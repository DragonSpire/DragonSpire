using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DragonSpire
{
	static class TickTimer
	{
		static long LastTimeUpdate = 0;
		static int TimeTickUpdateOffset = 1;

		static long LastPosUpdate = 0;
		static int PosTickUpdateOffset = 2;

		static long LastPlayerListUpdate = 0;
		static int PlayerListTickUpdateOffset = 20;

		static Player[] players;
		static Player[] removeFromPlayerList;

		internal static void Initialize()
		{
			new Thread(StartMainTicking).Start();
			//new Thread(StartOnMoveTicking).Start();
			//new Thread(StartAITicking).Start();

		}

		static void StartMainTicking()
		{
			while (!Server.shouldShutdown)
			{
				//Server.TimeTicks++;
				Server.TimeTicks += 1;

				lock (Player.players) { players = Player.players.Values.ToArray(); }
				lock (Player.ToRemoveFromPlayerList) { removeFromPlayerList = Player.ToRemoveFromPlayerList.ToArray(); }

				try
				{
					UpdateTime();
					SendKeepAlives();
					UpdatePlayerPositions();
					UpdatePlayerList();
				}
				catch (Exception e)
				{
					Server.Log(e.Message, LogTypesEnum.Error);
					Server.Log(e.StackTrace, LogTypesEnum.Error);
				}

				players = new Player[0];
				removeFromPlayerList = new Player[0];
				Thread.Sleep(50);
			}
		}
		static void StartOnMoveTicking()
		{
			while (!Server.shouldShutdown)
			{

			}
		}

		static void UpdateTime()
		{
			if ((Server.TimeTicks - LastTimeUpdate) < TimeTickUpdateOffset) return;
			LastTimeUpdate = Server.TimeTicks;

			for (int i = 0; i < players.Length; i++)
			{
				Player p = players[i];
				if (!p.client.loggedin || p.client.isDisconnected) continue; //Continue if this player is not ready!
				p.client.SendTimeUpdate(); //Send the worlds time!
			}
		}
		static void SendKeepAlives()
		{
			for (int i = 0; i < players.Length; i++)
			{
				Player p = players[i];
				if (!p.client.loggedin || p.client.isDisconnected) continue; //Continue if this player is not ready!
				p.client.SendKeepAlive();
			}
		}
		static void UpdatePlayerPositions()
		{
			if ((Server.TimeTicks - LastPosUpdate) < PosTickUpdateOffset) return;
			LastPosUpdate = Server.TimeTicks;

			Entity[] EntityList;

			for (int i = 0; i < players.Length; i++)
			{
				Player p = players[i];
				if (!p.client.loggedin || p.client.isDisconnected) continue; //Continue if this player is not ready!

				//Console.WriteLine("This player is online and has " + p.HasSpawned.Count + " entities spawned");

				lock (p.HasSpawned) { EntityList = p.HasSpawned.ToArray(); } //Get a copy of the players visible entities

				for (int j = 0; j < EntityList.Length; j++) //Loop through visible list
				{
					Entity e = EntityList[j];
					if (e.EId == p.EId) continue; //This checks to see if we have the same player!

					p.client.SendEntityTeleport(e); //Send entity position
					p.client.SendEntityHeadLook(e);
				}

				p.client.OnMove();
			}
		}
		static void UpdatePlayerList()
		{
			if ((Server.TimeTicks - LastPlayerListUpdate) < PlayerListTickUpdateOffset) return;
			LastPlayerListUpdate = Server.TimeTicks;

			byte[] PlayerListData = GeneratePlayerListData();

			for (int i = 0; i < players.Length; i++)
			{
				Player p = players[i];
				if (!p.client.loggedin || p.client.isDisconnected) continue; //Continue if this player is not ready!

				p.client.SendRawPacket(PacketType.PlayerListItem, PlayerListData);
			}
		}

		static byte[] GeneratePlayerListData()
		{
			var bytes = new List<byte>(); //List to hold our data

			for (int i = 0; i < players.Length; i++) //Loop through online players
			{
				Player p = players[i]; //Set the local player variable

				if (p.client.loggedin && !p.client.isDisconnected) //Check if the player is logged in and is not disconnected!
				{
					if (bytes.Count > 0) bytes.Add((byte)PacketType.PlayerListItem); //Add player data! (only add id if bytes has data already!)
					bytes.AddRange(DBC.GetBytes(p.name));
					bytes.AddRange(DBC.GetBytes(true)); //This player is to be added to / remain in the player list!
					bytes.AddRange(DBC.GetBytes(p.client.ping));
				}
			}

			for (int i = 0; i < removeFromPlayerList.Length; i++) //Loop through the list of players to REMOVE from the player list
			{
				Player p = removeFromPlayerList[i]; //Set local player variable

				if (bytes.Count > 0) bytes.Add((byte)PacketType.PlayerListItem); //Add player data! (only add id if bytes has data already!)
				bytes.AddRange(DBC.GetBytes(p.name));
				bytes.AddRange(DBC.GetBytes(false)); //This player should be REMOVED from the player list
				bytes.AddRange(DBC.GetBytes((short)0));
			}

			return bytes.ToArray(); //Return our data
		}
	}
}
