using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;

namespace DragonSpire
{
	class Client
	{
		bool EnableDebug = false;
		bool debug
		{
			get
			{
				if (isDisconnected) return false;
				return EnableDebug;
			}
		}

		//TODO Finish Verification with Minecraft.net
		
		internal static string ServerID = "MerlinsTestSMPSVR"; //hack This should be randomly generated for security
		internal byte[] SharedSecret = new byte[16];
		internal byte[] SharedKey;
		
		internal bool loggedin = false;
        internal short ping = 0;

        internal int KeepAliveID = 0;
        internal DateTime sentKeepAlive;
		bool receivedKeepAliveResponse = true;

        internal static string MOTD = "Merlin's Dragon SMP";
		
		private const byte ProtocolVersion = 74;
		private static readonly ASCIIEncoding Asen = new ASCIIEncoding();
		static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

		internal static Random random = new Random();

		internal readonly TcpClient _tcpClient;
		internal Socket _socket;

		private MCStream stream
		{
			get
			{
				return PM.mcStream;
			}
		}
		private PacketManager PM;

		public string _ip;
		internal bool isDisconnected = false;
		bool AnnouncedDisconnect = false;
		internal bool moved = false;

		Player player; //Holds player information

		ChunkManager chunkManager
		{
			get
			{
				return player.world.chunkManager;
			}
		}

		internal Client(TcpClient client, World w)
		{
			player = new Player(this, w);
			loggedin = false;
			RandomNumberGenerator.Create().GetBytes(SharedSecret);

			_tcpClient = client;
			new Thread(Start).Start();
		}

		internal void Start()
		{
			try
			{
				_socket = _tcpClient.Client;
				PM = new PacketManager(this, _tcpClient.GetStream());
				_ip = _socket.RemoteEndPoint.ToString().Split(':')[0];
				BeginRead();
			}
			catch (Exception e)
			{
				Server.Log(e.Message, LogTypesEnum.Error);
				Server.Log(e.StackTrace, LogTypesEnum.Error);
			}
		}

		internal void BeginRead()
		{
			try
			{
				while (!isDisconnected)
				{
					if (Server.shouldShutdown) return;
					if (isDisconnected) return;
					
					while (!stream.isDataAvailable)
					{
						//Console.WriteLine("NO DATA");
						if (Server.shouldShutdown) return;
						if (isDisconnected) return;

						Thread.Sleep(100);
					}

					var packetType = (PacketType)stream.ReadByte();
					//if(debug) Console.WriteLine(packetType);

					switch (packetType)
					{
						case (PacketType.KeepAlive): ReceiveKeepAlive(); break;
						case (PacketType.LoginRequest): ReceiveLoginRequest(); return;
						case (PacketType.HandShake): ReceiveHandshake(); break;
						case (PacketType.ChatMessage): ReceiveChatMessage(); break;
						case (PacketType.UseEntity): ReceiveUseEntity(); break;
						case (PacketType.Player): ReceivePlayer(); break;
						case (PacketType.PlayerPosition): ReceivePlayerPosition(); break;
						case (PacketType.PlayerLook): ReceivePlayerLook(); break;
						case (PacketType.PlayerPositionAndLook): ReceivePlayerPositionAndLook(); break;
						case (PacketType.PlayerDigging): ReceivePlayerDigging(); break;
						case (PacketType.PlayerBlockPlacement): ReceivePlayerBlockPlacement(); break;
						case (PacketType.HeldItemChange): ReceiveHeldItemChange(); break;
						case (PacketType.Animation): ReceiveAnimation(); break;
						case (PacketType.EntityAction): ReceiveEntityAction(); break;
						case (PacketType.SteerVehicle): ReceiveSteerVehicle(); break;
						case (PacketType.CloseWindow): ReceiveCloseWindow(); break;
						case (PacketType.ClickWindow): ReceiveClickWindow(); break;
						case (PacketType.ConfirmTransaction): ReceiveConfirmTransaction(); break;
						case (PacketType.CreativeInventoryAction): ReceiveCreativeInventoryAction(); break;
						case (PacketType.EnchantItem): ReceiveEnchantItem(); break;
						case (PacketType.UpdateSign): ReceiveUpdateSign(); break;
						case (PacketType.PlayerAbilities): ReceivePlayerAbilities(); break;
						case (PacketType.TabComplete): ReceiveTabComplete(); break;
						case (PacketType.ClientSettings): ReceiveClientSettings(); break;
						case (PacketType.ClientStatuses): ReceiveClientStatuses(); break;
						case (PacketType.PluginMessage): ReceivePluginMessage(); break;
						case (PacketType.EncryptionKeyResponse): ReceiveEncryptionResponse(); break;
						case (PacketType.ServerListPing): ReceiveServerListPing(); break;
						case (PacketType.DisconnectOrKick): ReceiveDisconnectOrKick(); return;
						default:
							break;
					}
				}
				Disconnect();
			}
			catch(Exception ex)
			{
				if (!ex.Message.StartsWith("Unable to read", StringComparison.OrdinalIgnoreCase) && !ex.Message.StartsWith("Attempted to read", StringComparison.OrdinalIgnoreCase)) //These two messages mean we lost connection to the client, or they lost connection to us, so we dont need to write errors or send a disconnect statement
				{
				    Server.Log(ex.Message, LogTypesEnum.Debug);
				    Server.Log(ex.StackTrace, LogTypesEnum.Debug);

					if (!isDisconnected)
					{
						SendDisconnect("error in packets, or you are gone");
					}
					else
					{
						Disconnect();
					}
				}
			}
		}

		#region Receive Methods

		/// <summary>
		/// Receive an int from the client, currently this integer does nothing except tell the server that the client is still alive, so we will reset the timeout and ignore the int
		/// </summary>
        void ReceiveKeepAlive()
        {
            if (stream.ReadInt() == KeepAliveID) //Read the integer from the stream and see if this is the keep alive we are waiting on!
            {
                TimeSpan ts = new TimeSpan();
                ts = DateTime.Now - sentKeepAlive;
                ping = (short)ts.Milliseconds;
            }

			receivedKeepAliveResponse = true; //We have received a keep alive and we are ready to send another one!
        }
		/// <summary>
		/// This is not normally sent to the server unless the client has "Minecraft forge" installed, since we don't handle that we will kick the client for now
		/// </summary>
		void ReceiveLoginRequest()
		{
			SendDisconnect("We do not support MinecraftForge (yet)");
		}
		/// <summary>
		/// The username of the player and the server/port which they are attempting to connect to
		/// </summary>
		void ReceiveHandshake()
		{
			stream.ReadByte(); //The version of the MC Protocol this client is using
			player.username = stream.ReadString(); //The clients username
			Server.Log(player.username + " is connecting", LogTypesEnum.Normal);
			stream.ReadString(); //The Server this client is attempting to connect to (should be our server)
			stream.ReadInt(); //The port the clietn is attempting to connect on, should be our port as well

			SendEncryptionRequest(); //This user wants to login, so we will send a request to initiate the login sequence
		}
		/// <summary>
		/// Here is where we will receive chat messages from the client
		/// </summary>
		void ReceiveChatMessage()
		{
			string IncomingMessage = stream.ReadString();
			if (IncomingMessage.Length > 1 && IncomingMessage[0] == '/' && IncomingMessage[1] != '/') //First is a / but second is NOT (ie you can type //test and it will NOT use the test command
			{
				IncomingMessage = IncomingMessage.Remove(0, 1);
				HandleCommand(IncomingMessage);
			}
			else
			{
				GlobalChat(player.username, IncomingMessage);
			}
		}
		/// <summary>
		/// This is where we handle player commands
		/// </summary>
		/// <param name="s">the full command sent by the player (excluding the leadig /)</param>
		void HandleCommand(string s)
		{
			string[] args = s.Split(' ');

			if (!CommandManager.ExecuteCommand(player, args[0], s))
			{
				SendNonFormattedMessage("Command does not exist!");
			}
		}
		/// <summary>
		/// This packet tells us when the player is attacking / right clicking an entity in the world
		/// </summary>
		void ReceiveUseEntity()
		{
			int User = stream.ReadInt(); //The entity ID of the player (we dont need this because the player should only send us THEIR id)
			int Target = stream.ReadInt(); //The entity this player is using (or attacking)
			bool isLeftClick = Convert.ToBoolean(stream.ReadByte()); //Whether or not the player LEFT clicked the entity (ie left clickign = attack/dig and right click is use/block)

			//TODO Handle UseEntity
		}
		/// <summary>
		/// This packet is simply whether or not this player is on the ground or not
		/// </summary>
		void ReceivePlayer()
		{
			//TODO (for anti-falldamage-avoidance we should calculate this ourselves eventually)
			bool OnGround = Convert.ToBoolean(stream.ReadByte());
		}
		/// <summary>
		/// This packet gives us the players current position (accurate) in the world, it does not give us the pitch/yaw
		/// </summary>
		void ReceivePlayerPosition()
		{
			double x = stream.ReadDouble(); //This players x position
			double y = stream.ReadDouble(); //This players y Position
			double stance = stream.ReadDouble(); //This players STANCE (used for stairs, crouching etc)
			double z = stream.ReadDouble(); //This players z position
			bool OnGound = Convert.ToBoolean(stream.ReadByte()); //Whether or not this player is on the ground

			//Console.WriteLine("LITERAL: " + x + " " + y + " " + z + " " + Server.TimeTicks);

			player.location = player.location.MoveTo(x, y, stance, z, OnGound);
		}
		/// <summary>
		/// This packet gives us the players look directiong (yaw/pitch)
		/// </summary>
		void ReceivePlayerLook()
		{
			float yaw = stream.ReadSingle(); //This players current YAW
			float pitch = stream.ReadSingle(); //This players current PITCH
			bool OnGound = Convert.ToBoolean(stream.ReadByte()); //Whether this player is on the ground or not

			player.location = player.location.MoveLook(yaw, pitch);
		}
		/// <summary>
		/// This packet tells us where the player is and where the player is currently looking at in the world
		/// </summary>
		void ReceivePlayerPositionAndLook()
		{
			double x = stream.ReadDouble(); //Players X Position
			double y = stream.ReadDouble(); //Player Y Postion
			double stance = stream.ReadDouble(); //Players stance (for stairs crouching etc)
			double z = stream.ReadDouble(); //Players Z position
			float yaw = stream.ReadSingle(); //Players look YAW
			float pitch = stream.ReadSingle(); //Players look Pitch
			bool OnGround = Convert.ToBoolean(stream.ReadByte()); //Whether or not this player is on the ground

			player.location = player.location.MoveTo(x, y, stance, z, OnGround);
			player.location = player.location.MoveLook(yaw, pitch);
		}
		/// <summary>
		/// This packet is send when a player mines (or attempts to mine) a block
		/// </summary>
		void ReceivePlayerDigging()
		{
			DiggingStatus status = (DiggingStatus)stream.ReadByte();
			int x = stream.ReadInt(); //The block this player is hitting (x)
			byte y = stream.ReadByte(); //The block this player is hitting (y) 
			int z = stream.ReadInt(); //The block this player is hitting (z)
			Face face = (Face)stream.ReadByte(); //The face of the block this player is hitting
			try
			{
				if (status == DiggingStatus.FinishedDigging || (player.isInCreative && status == DiggingStatus.StartedDigging))
				{
					BlockLocation BL = new BlockLocation(x, y, z, player.world);

					//Console.WriteLine("BLOCK: " + BL.ToString());
					
					Block b = player.world.GetBlock(BL);

					if (b.DirectAccess.OnBreak(BL, player, player.world))
					{
						player.world.SetBlock(BL, MaterialManager.GetBlock(MaterialEnum.Air));
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
			//TODO Handle Digging (properly)
		}
		/// <summary>
		/// This packet tells us when the client attempts to place a block
		/// </summary>
		void ReceivePlayerBlockPlacement()
		{
			int blockX = stream.ReadInt(); //The X coordinate where we are placing this block
			byte blockY = stream.ReadByte(); //The Y Coordinate where we are placing this block
			int blockZ = stream.ReadInt(); //The Z Coordinate where we are placing this block
			Direction direction = (Direction)stream.ReadByte(); //The Direcion this block should be facing
			SLOT HeldItem = stream.ReadSlot(); //Information on the held iteam
			byte CursorX = stream.ReadByte(); //The position of the cursor within this block (X)
			byte CursorY = stream.ReadByte(); //The position of the cursor within this block (Y)
			byte CursorZ = stream.ReadByte(); //The position of the cursor within this block (Z)
		}
		/// <summary>
		/// Received when the player changes the slot they have selected on their hotbar
		/// </summary>
		void ReceiveHeldItemChange()
		{
			short SlotID = stream.ReadShort();
		}
		/// <summary>
		/// Send when an entity should change animation
		/// </summary>
		void ReceiveAnimation()
		{
			int EntityID = stream.ReadInt();
			byte AnimationID = stream.ReadByte();
		}
		/// <summary>
		/// Send at least once while crouching, leaving a bed or sprinting.
		/// </summary>
		void ReceiveEntityAction()
		{
			int EntityID = stream.ReadInt(); //Players EID
			byte ActionID = stream.ReadByte(); //The action the player is or should be taking
			int JumpBoost = stream.ReadInt(); //Horse jump boost?
		}
		/// <summary>
		/// This is sent by the client to the server to steer vehicles
		/// </summary>
		void ReceiveSteerVehicle()
		{
			float Sideways = stream.ReadSingle(); //Positive to the left of the player
			float Forward = stream.ReadSingle(); //Positive when going forward
			bool Jump = stream.ReadBool();
			bool Unmount = stream.ReadBool();
		}
		/// <summary>
		/// Received when the player closes a window
		/// </summary>
		void ReceiveCloseWindow()
		{
			byte WindowID = stream.ReadByte();
		}
		/// <summary>
		/// Received when a player clicks on a window (IE to click an item)
		/// </summary>
		void ReceiveClickWindow()
		{
			byte WindowID = stream.ReadByte();
			short SlotID = stream.ReadShort();
			byte Button = stream.ReadByte(); //The button used in the click
			short ActionNumber = stream.ReadShort(); //Unique action number used for verifying transactions
			byte Mode = stream.ReadByte();
			SLOT ClickedItem = stream.ReadSlot();
		}
		/// <summary>
		/// Used to Confirm an inventory transaction (to help prevent item duping)
		/// </summary>
		void ReceiveConfirmTransaction()
		{
			byte WindowID = stream.ReadByte(); //The id of the window where the transaction occured
			short ActionNumber = stream.ReadShort(); //The action number of the transaction
			bool Accepted = stream.ReadBool(); //Whether or not the action was accepted
		}
		/// <summary>
		/// Used while the user has the inventory open in creative mode 
		/// </summary>
		void ReceiveCreativeInventoryAction()
		{
			short SlotID = stream.ReadShort();
			SLOT ClickedItem = stream.ReadSlot();
		}
		/// <summary>
		/// Packet sent when a client enchants an item
		/// </summary>
		void ReceiveEnchantItem()
		{
			byte WindowId = stream.ReadByte();
			byte Enchantment = stream.ReadByte();
		}
		/// <summary>
		/// Sent when a sign needs updated
		/// </summary>
		void ReceiveUpdateSign()
		{
			int x = stream.ReadInt();
			short y = stream.ReadShort();
			int z = stream.ReadInt();
			string FirstLine = stream.ReadString();
			string SecondLine = stream.ReadString();
			string ThirdLine = stream.ReadString();
			string FourthLine = stream.ReadString();
		}
		/// <summary>
		/// Received when the player starts/stop flying, should contain all the players special abilities (fly godmode and creative)
		/// </summary>
		void ReceivePlayerAbilities()
		{
			byte Flags = stream.ReadByte(); //bit values for different flags for powers
			float FlyingSpeed = stream.ReadSingle();
			float WalkingSpeed = stream.ReadSingle();
		}
		/// <summary>
		/// Used for auto completion in the text field
		/// </summary>
		void ReceiveTabComplete()
		{
			string StringToCheck = stream.ReadString();
		}
		/// <summary>
		/// Receive client settings (locale ViewDistance ChatFlags Difficulty and ShowCapes)
		/// </summary>
		void ReceiveClientSettings()
		{
			string locale = stream.ReadString();
			byte ViewDistance = stream.ReadByte();
			byte chatFlags = stream.ReadByte();
			byte difficulty = stream.ReadByte();
			byte ShowCape = stream.ReadByte();

			//Console.WriteLine("This clients locale is " + locale + " VD is " + ViewDistance + " chunks, difficulty is " + (Difficulty)difficulty + " and we should show capes is " + Convert.ToBoolean(ShowCape));
		}
		/// <summary>
		/// Receive client Statuses (Tells us when the player is ready to Spawn / Respawn)
		/// </summary>
		void ReceiveClientStatuses()
		{
			byte Status = stream.ReadByte();
			if (Status == 1)
			{
				//Respawn
				//Respawn();
			}
			else
			{
				//Initial Spawn
				InitializePlayer();
			}
		}
		/// <summary>
		/// Messages made for plugins!
		/// </summary>
		void ReceivePluginMessage()
		{
			string Channel = stream.ReadString();
			short Length = stream.ReadShort();
			byte[] Data = stream.ReadByteArray(Length);
		}
		/// <summary>
		/// Receiving the final Encryption response so we can  encrypt the stream!
		/// </summary>
		void ReceiveEncryptionResponse()
		{
			//Console.WriteLine("Receiving Encryption Response!");

			byte[] SharedSecret = stream.ReadByteArray(stream.ReadShort());
			byte[] VerifyTokenResponse = stream.ReadByteArray(stream.ReadShort());

			SharedKey = Server.CryptoServiceProvider.Decrypt(SharedSecret, false);

			//TODO [LP] Check encryption for Security Matches

			SendEncryptionResponse();

			//ENCRYPT STREAM
			stream.BeginEncryption(SharedKey);
			
		}
		/// <summary>
		/// This is the server list (ie server selection screen) ping
		/// </summary>
        void ReceiveServerListPing()
        {
            stream.ReadByte(); //Always 1 // also not sure about needing to read more bytes than this, but its a disconnect anyway so it'd only be useful for statys or mods.

            string ServerPing = String.Format("§1\0{0}\0{1}\0{2}\0{3}\0{4}",  // §1 is pinglist identifier. \0 is null separator.
                                    Client.ProtocolVersion, 
                                    "1.6.2", // not sure where this comes from? maybe a Client.ServerVersion?
                                    Client.MOTD, // name as appears on list.
                                    Player.GetPlayerCount(),
                                    Config.MaxPlayers);
             
            SendDisconnect(ServerPing); // and finally send it. :)
        }
		/// <summary>
		/// This is received when the player is leaving
		/// </summary>
		void ReceiveDisconnectOrKick()
		{
			string reason = stream.ReadString();
			Console.WriteLine(reason);
			//Disconnect();
			//GlobalAnnouncement("SERVER", player.name + " has left.");
		}

		#endregion
		#region Send Methods

		internal void SendKeepAlive()
		{
			if (!loggedin || !receivedKeepAliveResponse) return; //We dont send keepalives is either A. They are not logged in, or B. We are waiting on a response

			var PD = PM.GetFreshPacket(PacketType.KeepAlive);
			KeepAliveID = random.Next();
			PD.Add(KeepAliveID);
			sentKeepAlive = DateTime.Now;
			PM.Packets.Add(PD);

			receivedKeepAliveResponse = false; //We are waiting on a response
		}
		internal void SendRawPacket(PacketType type, byte[] data)
		{
			var PD = PM.GetFreshPacket(type); //DataType
			PD.data.AddRange(data); //Add Data
			PM.Packets.Add(PD); //Add to Packet List to Send
		}

		#region Chat
		void SendPreFormattedChatMessage(string s)
		{
			if (debug) Console.WriteLine("SendPreFormattedChatMessage");

			var PD = PM.GetFreshPacket(PacketType.ChatMessage);
			PD.Add(s);
			PM.Packets.Add(PD);
		}
		internal void SendNonFormattedMessage(string s)
		{
			SendPreFormattedChatMessage(Messager.Announcement("Server", s));
		}
		internal static void GlobalChat(string FROM, string s)
		{
			foreach (Player p in Player.players.Values.ToArray())
			{
				p.client.SendPreFormattedChatMessage(Messager.ChatMessage(FROM, s));
			}
		}
		internal static void GlobalAnnouncement(string FROM, string s)
		{
			foreach (Player p in Player.players.Values.ToArray())
			{
				p.client.SendPreFormattedChatMessage(Messager.Announcement(FROM, s));
			}
		}
		#endregion
		#region Login Disconnect and ServerList Methods
		void SendEncryptionRequest()
		{
			if (debug) Console.WriteLine("SendEncryptionRequest");

			var verifyToken = new byte[4];
			var csp = new RNGCryptoServiceProvider();
			csp.GetBytes(verifyToken);

			var encodedKey = PacketCryptography.AsnKeyBuilder.PublicKeyToX509(Server.ServerKey);

			var PD = PM.GetFreshPacket(PacketType.EncryptionKeyRequest);
			PD.Add(ServerID);
			PD.Add((short)encodedKey.Length);
			PD.Add(encodedKey.GetBytes());
			PD.Add((short)verifyToken.Length);
			PD.Add(verifyToken);
			PM.Packets.Add(PD);
		}
		void SendEncryptionResponse()
		{
			if (debug) Console.WriteLine("SendEncryptionResponse");

			byte[] test = new byte[0];

			var PD = PM.GetFreshPacket(PacketType.EncryptionKeyResponse);
			PD.Add((short)test.Length);
			PD.Add(test);
			PD.Add((short)test.Length);
			PD.Add(test);
			PM.Packets.Add(PD);

		} //TODO Implement Encryption request properly
		void SendLoginResponse()
		{
			if (debug) Console.WriteLine("SendLoginResponse");

			var PD = PM.GetFreshPacket(PacketType.LoginRequest);

			PD.Add(player.EId);
			PD.Add("Default");
			PD.Add((byte)GameMode.Survival); //Gamemode 
			PD.Add((byte)Dimension.Overworld); //Dimension
			PD.Add((byte)Difficulty.Normal); //Difficulty
			PD.Add((byte)0); //Not Used
			PD.Add((byte)8); //Max players

			PM.Packets.Add(PD);
		} //TODO pull variables from server settings and level
		void InitializePlayer()
		{
			if(debug) Console.WriteLine("Initializing player!");

			SendLoginResponse();

			player.oldChunk = player.currentChunk;
			player.oldRegion = player.currentRegion;

			SendSurroundingArea();

			SendSpawnPosition();
			SendPlayerPositionAndLook();



			loggedin = true;
			Server.Log(player.username + " has logged in!", LogTypesEnum.Normal);

			Player.players.Add(player.EId, player);
			player.currentChunk.Players.Add(player.EId, player);

			UpdateVisibleEntities();
            SendKeepAlive();
		}

		void SendDisconnect(string s)
		{
			if (debug) Console.WriteLine("SendDisconnect");

			var PD = PM.GetFreshPacket(PacketType.DisconnectOrKick);
			PD.Add(s);
			PM.Packets.Add(PD);

			Disconnect();
		}
		void Disconnect()
		{
			if (!AnnouncedDisconnect)
			{
				AnnouncedDisconnect = true;

				if (loggedin)
				{
					loggedin = false;

					if (player.currentChunk.Players.ContainsKey(player.EId))
						player.currentChunk.Players.Remove(player.EId);

					Console.WriteLine(player.username + " Left.");

					Player.ToRemoveFromPlayerList.Add(player);
				}

				if (Player.players.ContainsKey(player.EId))
				{
					Player.players.Remove(player.EId);
				}

				List<int> e = new List<int>();
				e.Add(player.EId);

				foreach (Player p in Player.players.Values.ToArray())
				{
					if (p.HasSpawned.Contains(player))
					{
						p.HasSpawned.Remove(player);
						p.client.SendDestroyEntity(e);
					}
				}
			}
		}
		#endregion
		#region World Methods
		internal void SendSurroundingArea()
		{
			List<RegionLocation> RegionsWeNeed = new List<RegionLocation>();
			List<RegionLocation> CurrentRegions = player.RegionsLoaded;
			List<RegionLocation> RegionsToUnLoad = new List<RegionLocation>();

			RegionLocation CurrentRegion = player.currentRegion;
			int CurrentRegion_X = CurrentRegion.X;
			int CurrentRegion_Z = CurrentRegion.Z;
			int ViewDistance = ChunkManager.RegionViewDistanceOffset;

			for (int x = 0-ViewDistance; x <= ViewDistance; x++) //from -3 to 3 for example (MUST BE <=)
			{
				for (int z = 0 - ViewDistance; z <= ViewDistance; z++) //from -3 to 3 for example (MUST BE <=)
				{
					int Loop_X = CurrentRegion_X + x; //Loop X = Region X + Current X
					int Loop_Z = CurrentRegion_Z + z; //Loop Z = Region Z + Current Z

					RegionsWeNeed.Add(new RegionLocation(Loop_X, Loop_Z, player.world)); //Create a list of the regions in viewable distance
				}
			}

			foreach (RegionLocation Rl in player.RegionsLoaded) //Loop through the players regions
			{
				if (!RegionsWeNeed.Contains(Rl)) //If we dont need this region
				{
					RegionsToUnLoad.Add(Rl); //Add it to the list of regions to unload
				}
			}

			foreach (RegionLocation Fr in RegionsToUnLoad) //Loop through the regions we no longer need
			{
				if (!player.RegionsLoaded.Contains(Fr)) continue; //Player can't see this region, no sense in unloading it!

				SendUnloadRegion(Fr); //Unload the regions we no longer need
			}
			foreach (RegionLocation Fr in RegionsWeNeed) //Loop through the regions we need
			{
				if(player.RegionsLoaded.Contains(Fr)) continue; //Player can already see this region

				SendRegion(Fr); //Send the regions we need
			}
		}
		internal void SendRegion(RegionLocation region)
		{
			player.RegionsLoaded.Add(region); //this player can now see this region

			List<Chunk> list = chunkManager.GetRegionChunks(region);
			SendMapChunkBulk(list);
		}
		internal void SendUnloadRegion(RegionLocation region)
		{
			player.RegionsLoaded.Remove(region); //This player can no longer see this region

			List<Chunk> list = chunkManager.GetRegionChunks(region);
			SendMapChunkBulkUNLOAD(list);
		}
		void SendChunkData(Chunk c)
		{
			if (debug) Console.WriteLine("SendChunkData");

			byte[] bytes = c.GetCompressedData(); //Array holding map data

			var PD = PM.GetFreshPacket(PacketType.ChunkData);
			PD.Add(c.CL.X);
			PD.Add(c.CL.Z);
			PD.Add(true);
			PD.Add(ushort.MaxValue);
			PD.Add((ushort)0);
			PD.Add(bytes.Length);
			PD.Add(bytes);
			PM.Packets.Add(PD);
		}
		void SendUnloadChunk(Chunk c)
		{
			if (debug) Console.WriteLine("SendUnloadChunk");

			var PD = PM.GetFreshPacket(PacketType.ChunkData);
			PD.Add(c.CL.X); //X
			PD.Add(c.CL.Z); //Z
			PD.Add(true); //Ground Up
			PD.Add((ushort)0);
			PD.Add((ushort)0);
			PD.Add(0);
			PM.Packets.Add(PD);
		}
		void SendMapChunkBulk(List<Chunk> chunks)
		{
			if (debug) Console.WriteLine("SendMapChunkBulk");

			byte[] Data = Chunk.GetCompressedData(chunks); //Array holding map data

			var PD = PM.GetFreshPacket(PacketType.MapChunkBulk);
			PD.Add((short)chunks.Count);
			PD.Add(Data.Length);
			PD.Add(true);
			PD.Add(Data);

			foreach (Chunk c in chunks)
			{
				PD.Add(c.CL.X);
				PD.Add(c.CL.Z);
				PD.Add(ushort.MaxValue);
				PD.Add((ushort)0);
			}

			PM.Packets.Add(PD);
		}
		void SendMapChunkBulkUNLOAD(List<Chunk> chunks)
		{
			if (debug) Console.WriteLine("SendMapChunkBulkUNLOAD");

			foreach (Chunk c in chunks)
			{
				SendUnloadChunk(c);
			}
		}
		void SendSpawnPosition()
		{
			SendSpawnPosition(player.world.Spawn);
		}
		void SendSpawnPosition(BlockLocation l)
		{
			if (debug) Console.WriteLine("SendSpawnPosition");

			var PD = PM.GetFreshPacket(PacketType.SpawnPosition);
			PD.Add(l.X);
			PD.Add((int)l.Y);
			PD.Add(l.Z);
			PM.Packets.Add(PD);
		}

		internal void SendTimeUpdate()
		{
			SendTimeUpdate(player.world.CurrentTime);
		}
		internal void SendTimeUpdate(long time)
		{
			if (debug) Console.WriteLine("SendTimeUpdate");

			var PD = PM.GetFreshPacket(PacketType.TimeUpdate);
			PD.Add(player.world.age);
			PD.Add(time);
			PM.Packets.Add(PD);
		}
				
		internal void SendBlockChange(BlockLocation bl, Material mat, byte Meta)
		{
			if (debug) Console.WriteLine("SendBlockChange");

			var PD = PM.GetFreshPacket(PacketType.BlockChange);
			PD.Add(bl.X);
			PD.Add(bl.Y);
			PD.Add(bl.Z);
			PD.Add(mat.sendID);
			PD.Add(Meta);
			PM.Packets.Add(PD);
		}
		void SendMultiBlockChange()
		{
			//TODO MultiBlock Change
		}
		void SendUpdateSign()
		{

		}
		void SendUpdateTileEntity()
		{

		}
		#endregion
		#region Player (self) Methods)
		internal void SendPlayerPositionAndLook()
		{
			SendPlayerPositionAndLook(player.physics.Location);
		}
		internal void SendPlayerPositionAndLook(EntityLocation PL)
		{
			if (debug) Console.WriteLine("SendPlayerPositionAndLook");

			var PD = PM.GetFreshPacket(PacketType.PlayerPositionAndLook);
			PD.Add(PL.X);
			PD.Add(PL.Y);
			PD.Add(PL.Stance);
			PD.Add(PL.Z);
			PD.Add(PL.Yaw);
			PD.Add(PL.Pitch);
			PD.Add(PL.OnGround);
			PM.Packets.Add(PD);
		}
		void SendUpdateHealth()
		{
			if (debug) Console.WriteLine("SendUpdateHealth");

			var PD = PM.GetFreshPacket(PacketType.UpdateHealth);
			PD.Add(player.Health);
			PD.Add(player.Hunger);
			PD.Add(player.FoodSaturation);
			PM.Packets.Add(PD);
		}
		void SendSetExperience()
		{
			if (debug) Console.WriteLine("SendSetExperience");

			var PD = PM.GetFreshPacket(PacketType.SetExperience);
			PD.Add(player.ExpBar);
			PD.Add(player.Level);
			PD.Add(player.TotalExp);
			PM.Packets.Add(PD);
		}
		void SendPlayerAbilities()
		{

		}
		void SendRespawn()
		{
			if (debug) Console.WriteLine("SendRespawn");

			var PD = PM.GetFreshPacket(PacketType.Respawn);
			PD.Add((int)Dimension.Overworld);
			PD.Add((byte)Difficulty.Normal);
			PD.Add((byte)GameMode.Survival);
			PD.Add((short)256);
			PD.Add("default");
			PM.Packets.Add(PD);
		} //TODO Pull info from level
		internal void ChangeGameState(byte reason, byte gamemode)
		{
			if (debug) Console.WriteLine("ChangeGameState");

			var PD = PM.GetFreshPacket(PacketType.ChangeGameState);
			PD.Add(reason);
			PD.Add(gamemode);
			PM.Packets.Add(PD);
		}
		#endregion
		#region Entity Methods
		void SendEntityEquipment(Entity e)
		{

		} //TODO all
		void SendHeldItemChange()
		{
			//SendHeldItemChange(player.inventory.SelectedSlot);
		}
		void SendHeldItemChange(short SlotID)
		{
			if (debug) Console.WriteLine("SendHeldItemChange");

			var PD = PM.GetFreshPacket(PacketType.HeldItemChange);
			PD.Add(SlotID);
			PM.Packets.Add(PD);
		}
		void SendUseBed(Entity e, BlockLocation BedLocation)
		{
			if (debug) Console.WriteLine("SendUseBed");

			var PD = PM.GetFreshPacket(PacketType.UseBed);
			PD.Add(e.EId);
			PD.Add((byte)0);
			PD.Add(BedLocation.X);
			PD.Add(BedLocation.Y);
			PD.Add(BedLocation.Z);
			PM.Packets.Add(PD);
		}
		internal void SpawnNamedEntity(Player p) //Change to entity
		{
			if (debug) Console.WriteLine("SpawnNamedEntity");

			if (p.EId == player.EId) return;

			var PD = PM.GetFreshPacket(PacketType.SpawnNamedEntity);
			PD.Add(p.EId);
			PD.Add(p.username);
			PD.Add((int)p.physics.Location.AX);
			PD.Add((int)p.physics.Location.AY);
			PD.Add((int)p.physics.Location.AZ);
			PD.Add(p.physics.Location.YawByte);
			PD.Add(p.physics.Location.PitchByte);
			PD.Add(0);
			PD.Add(p.GenerateMetaData());
			PM.Packets.Add(PD);

			player.HasSpawned.Add(p);
		}
		void SendCollectItem(Entity Collector, Entity ItemBeingCollected)
		{
			if (debug) Console.WriteLine("SendCollectItem");

			var PD = PM.GetFreshPacket(PacketType.CollectItem);
			PD.Add(ItemBeingCollected.EId);
			PD.Add(Collector.EId);
			PM.Packets.Add(PD);
		}
		internal void SpawnObjectOrVehicle(ObjectEntity e)
		{
			if (debug) Console.WriteLine("SpawnObjectOrVehicle");

			var PD = PM.GetFreshPacket(PacketType.SpawnObjectOrVehicle);
			PD.Add(e.EId);
			PD.Add((byte)e.ObjectType);
			PD.Add((int)e.physics.Location.AX);
			PD.Add((int)e.physics.Location.AY);
			PD.Add((int)e.physics.Location.AZ);
			PD.Add(e.physics.Location.YawByte);
			PD.Add(e.physics.Location.PitchByte);
			PD.Add(e.Data);
			PD.Add(e.GenerateObjectData());

			//Were throwing this on the end of the object's data to get it to the client at the same time
			PD.Add((byte)PacketType.EntityMetaData);
			PD.Add(e.EId);
			PD.Add(e.GenerateMetaData());

			PM.Packets.Add(PD);

			//SendMetaData(e);
		}
		void SpawnMob(Mob e)
		{
			if (debug) Console.WriteLine("SpawnMob");

			var PD = PM.GetFreshPacket(PacketType.SpawnMob);
			PD.Add(e.EId);
			PD.Add((byte)e.MobType);
			PD.Add((int)e.physics.Location.AX);
			PD.Add((int)e.physics.Location.AY);
			PD.Add((int)e.physics.Location.AZ);
			PD.Add(e.physics.Location.PitchByte);
			PD.Add(e.physics.Location.PitchByte);
			PD.Add(e.physics.Location.YawByte);
			PD.Add((short)e.physics.Velocity.X);
			PD.Add((short)e.physics.Velocity.Y);
			PD.Add((short)e.physics.Velocity.Z);
			PD.Add(e.GenerateMetaData());
			PM.Packets.Add(PD);
		}
		void SpawnPainting()
		{
			//NYI
		}
		void SpawnExperianceOrb()
		{

		}
		void SendEntityVelocity(Entity e)
		{
			if (debug) Console.WriteLine("SendEntityVelocity");

			var PD = PM.GetFreshPacket(PacketType.EntityVelocity);
			PD.Add(e.EId);
			PD.Add((short)e.physics.Velocity.X);
			PD.Add((short)e.physics.Velocity.Y);
			PD.Add((short)e.physics.Velocity.Z);
			PM.Packets.Add(PD);
		}
		void SendDestroyEntity(List<int> entities)
		{
			if (debug) Console.WriteLine("SendDestroyEntity");

			var PD = PM.GetFreshPacket(PacketType.DestroyEntity);
			PD.Add((byte)entities.Count);
			PD.Add(entities.ToArray());
			PM.Packets.Add(PD);
		}
		void SendEntity(Entity e)
		{
			if (debug) Console.WriteLine("SendEntity");

			var PD = PM.GetFreshPacket(PacketType.Entity);
			PD.Add(e.EId);
			PM.Packets.Add(PD);
		}
		void SendRelativeMove(Entity e)
		{

		} //TODO Calculate short movement
		void SendEntityLook(Entity e)
		{
			if (debug) Console.WriteLine("SendEntityLook");

			var PD = PM.GetFreshPacket(PacketType.EntityLook);
			PD.Add(e.EId);
			PD.Add(e.physics.Location.YawByte);
			PD.Add(e.physics.Location.PitchByte);
			PM.Packets.Add(PD);
		}
		void SendEntityLookAndRelativeMove(Entity e)
		{

		} //TODO Calculate short movement
		internal void SendEntityTeleport(Entity e)
		{
			////if (debug) Console.WriteLine("SendEntityTeleport");
			if (e.EId == player.EId) return;

			var PD = PM.GetFreshPacket(PacketType.EntityTeleport);
			PD.Add(e.EId);
			PD.Add((int)e.physics.Location.X * 32);
			PD.Add((int)e.physics.Location.Y * 32);
			PD.Add((int)e.physics.Location.Z * 32);
			PD.Add(e.physics.Location.YawByte);
			PD.Add(e.physics.Location.PitchByte);
			PM.Packets.Add(PD);
		}
		internal void SendEntityHeadLook(Entity e)
		{
			//if (debug) Console.WriteLine("SendEntityHeadLook");
			if (e.EId == player.EId) return;

			var PD = PM.GetFreshPacket(PacketType.EntityHeadLook);
			PD.Add(e.EId);
			PD.Add(e.physics.Location.YawByte);
			PM.Packets.Add(PD);
		}
		void SendEntityStatus(Entity e)
		{
			if (debug) Console.WriteLine("SendEntityStatus");

			var PD = PM.GetFreshPacket(PacketType.EntityStatus);
			PD.Add(e.EId);
			PD.Add(0); //TODO Entity Status ?
			PM.Packets.Add(PD);
		}
		void SendAttachEntity(Entity e, Entity vehicle, bool leashed)
		{
			if (debug) Console.WriteLine("SendAttachEntity");

			var PD = PM.GetFreshPacket(PacketType.AttachEntity);
			PD.Add(e.EId);
			PD.Add(vehicle.EId);
			PD.Add(leashed); //Supposed to be Unsigned Byte (test this)
			PM.Packets.Add(PD);
		}
		void SendMetaData(Entity e)
		{
			if (debug) Console.WriteLine("SendMetaData");

			var PD = PM.GetFreshPacket(PacketType.EntityMetaData);
			PD.Add(e.EId);
			PD.Add(e.GenerateMetaData());
			PM.Packets.Add(PD);
		}
		void SendEntityEffect(Entity e)
		{
			throw new NotImplementedException("SendEntityEffect is NYI");

			var PD = PM.GetFreshPacket(PacketType.EntityEffect);
			PD.Add(e.EId);
			PM.Packets.Add(PD);

			//TODO Entity Effect
			//TODO Effect Amplifier
			//TODO Effect Duration
		} //TODO Effect info
		void SendRemoveEntityEffect(Entity e)
		{
			throw new NotImplementedException("SendRemoveEntityEffect is NYI");

			var PD = PM.GetFreshPacket(PacketType.RemoveEntityEffect);
			PD.Add(e.EId);
			PM.Packets.Add(PD);

			//TODO Effect number
		} //TODO Effect Number
		void SendEntityProperties(Entity e)
		{
			//This
			//Packet
			//Sucks
		}
		void SpawnGlobalEntity() //Lightning
		{
			//TODI SpawnGlobalEntity (IE Lightning)
		}
		#endregion
		#region Window and Inventory
		internal void SendOpenWindow(byte WindowID, Window w)
		{
			var PD = PM.GetFreshPacket(PacketType.OpenWindow);
			PD.Add(WindowID); //The players id for this window (this is player specific)
			PD.Add(w.WindowType.InventoryType); //The inventory type (workbench, chest, furnace etc)
			PD.Add(w.Title); //The window title
			PD.Add(w.WindowType.SlotCount); //The number of slots (NOT including player inventory
			PD.Add(true); //We will always use the title we send to the client
			if(w.WindowType.InventoryType == 11 && w.isEntityWindow) PD.Add(w.e.EId); //We only send this if were using an animal chest (inventory type = 11) (horses silly!)
			PM.Packets.Add(PD);
		}
		internal void SendForceCloseWindow(byte WindowID)
		{
			var PD = PM.GetFreshPacket(PacketType.CloseWindow);
			PD.Add(WindowID);
			PM.Packets.Add(PD);
		}
		internal void SendSetSlot(byte WindowID, short Slot, SLOT data)
		{
			var PD = PM.GetFreshPacket(PacketType.SetSlot);
			PD.Add(WindowID);
			PD.Add(Slot);
			PD.Add(data.GeneratePacketSlotData());
			PM.Packets.Add(PD);
		}
		void SendSetWindowItems(byte WindowID, Window w)
		{
			var PD = PM.GetFreshPacket(PacketType.SetWindowItems);
			PD.Add(WindowID); //Window ID
			PD.Add(w.WindowType.SlotCount); //Number of slots
			PD.Add(w.container.GetAllSlotData()); //Slot Data
			PM.Packets.Add(PD);
		}
		void SendUpdateWindowProperty(byte WindowID, short Property, short Value)
		{
			var PD = PM.GetFreshPacket(PacketType.UpdateWindowProperty);
			PD.Add(WindowID); //Window ID
			PD.Add(Property); //Property to update (arrow or fuel)
			PD.Add(Value); //The value of the property
			PM.Packets.Add(PD);
		}
		void SendConfirmTransaction(byte WindowID, short ActionNumber, bool isAccepted)
		{
			var PD = PM.GetFreshPacket(PacketType.UpdateWindowProperty);
			PD.Add(WindowID); //Window ID
			PD.Add(ActionNumber); //The action number we are referancing
			PD.Add(isAccepted); //Whether or not we accept this transaction
			PM.Packets.Add(PD);
		}
		void SendCreativeInventoryAction()
		{
			//Not really sure how this works, or if we are going to use it...
		}
		void SendTileEditorOpen()
		{

		}
		void SendItemData()
		{
			//This
			//Packet
			//Also
			//Sucks
			//Ass
		}
		#endregion
		#region Effects
		void SendExplosion(EntityLocation location)
		{
			throw new NotImplementedException("Explosions are NYI");

			var PD = PM.GetFreshPacket(PacketType.Explosion);
			PD.Add(location.AX);
			PD.Add(location.AY);
			PD.Add(location.AZ);
			PD.Add((float)0);
			PM.Packets.Add(PD);

			//TODO send 'record' and 'records' and 'force velocity' of explosion

		} //TODO Pull information from some sort of explosion class or table or randomization
		void SendSoundOrParticleEffect()
		{
			//TODO Particle or Sound
		}
		void SendNamedSoundEffect()
		{
			//TODO Named sound
		}
		void SendParticle()
		{
			//TODO Particle
		}
		void SendBlockAction()
		{
			//TODO Send Block Action
		}
		void SendBlockBreakAnimation()
		{
			//TOOD Block BreakAnimation
		}
		void SendAnimation(Entity e, PlayerAnimations Animation)
		{
			if (debug) Console.WriteLine("SendAnimation");

			var PD = PM.GetFreshPacket(PacketType.Animation);
			PD.Add(e.EId);
			PD.Add((byte)Animation);
			PM.Packets.Add(PD);
		} //done
		#endregion
		#region PlayerList, Tab, Stats, Plugins and ScoreBoard
		void SendScoreboardObjective()
		{

		}
		void SendUpdateScore()
		{

		}
		void SendDisplayScoreboard()
		{

		}
		void SendTeams()
		{

		}
		void SendPluginMessage()
		{

		}
		void SendIncrementStatistic()
		{

		}
		internal void SendPlayerListRemoveItem(Player p)
		{
			var PD = PM.GetFreshPacket(PacketType.PlayerListItem);
			PD.Add(p.name);
			PD.Add(false);
			PD.Add(p.client.ping);
			p.client.PM.Packets.Add(PD);
		}
		void SendTabComplete()
		{

		}
		#endregion

		#endregion

		#region Movement Methods
		internal void OnMove()
		{
			if (player.GetCurrentChunk != player.oldChunk)
			{
				//Console.WriteLine(player.name + " Chunk Changed!");
				//Console.WriteLine("Player " + player.location.ToString());
				//Console.WriteLine("PChunk " + player.currentChunk.CL.ToString());
				OnChunkMove();
			}
		}
		void OnChunkMove()
		{
			player.oldChunk.Players.Remove(player.EId); //Remove player from old chunk
			player.oldChunk = player.currentChunk; //Set the current chunk as the old chunk

			if (!player.currentChunk.Players.ContainsKey(player.EId)) player.currentChunk.Players.Add(player.EId, player); //Add the player to the new chunk

			if (player.currentRegion != player.oldRegion) OnRegionMove(); //Check if we need to update region
		}
		void OnRegionMove()
		{
			player.oldRegion = player.currentRegion; //Set the old region to the current region
		
			SendSurroundingArea(); //Send Map Data
			UpdateVisibleEntities(); //Send Entity Data
		}

		internal void UpdateVisibleEntities()
		{
			var players = chunkManager.GetVisiblePlayers(player.currentRegion);
			var mobs = chunkManager.GetVisibleMobs(player.currentRegion);
			var objects = chunkManager.GetVisibleObjects(player.currentRegion);

			var EntitiesToDestroy = new List<int>(); //A list of entities to destroy via the 0x1D packet (Destroy Entities)

			if (players.Contains(player)) players.Remove(player); //remove this player from list

			foreach (Entity e in player.HasSpawned)
			{
				if (!players.Contains(e) && !mobs.Contains(e) && !objects.Contains(e))
				{
					Console.WriteLine("Destroying entity!");
					EntitiesToDestroy.Add(e.EId);
				}
			}

			if (EntitiesToDestroy.Contains(player.EId)) EntitiesToDestroy.Remove(player.EId);
			SendDestroyEntity(EntitiesToDestroy);

			foreach (Player p in players)
			{
				if (!player.HasSpawned.Contains(p))
				{
					if (p == player) continue;
					SpawnNamedEntity(p);
					if(!p.HasSpawned.Contains(player)) p.client.SpawnNamedEntity(player);
				}
			}
			foreach (Mob mob in mobs)
			{
				if (!player.HasSpawned.Contains(mob))
				{
					SpawnMob(mob);
				}
			}
			foreach (ObjectEntity OE in objects)
			{
				if (!player.HasSpawned.Contains(OE))
				{
					SpawnObjectOrVehicle(OE);
				}
			}
		}
		#endregion
	}
	
}
