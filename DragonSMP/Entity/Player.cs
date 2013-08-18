using System.Collections.Generic;
using System.Linq;

namespace DragonSpire
{
	public class Player : LivingEntity
	{
		internal static List<Player> ToRemoveFromPlayerList = new List<Player>();

		internal static Dictionary<int, Player> players = new Dictionary<int, Player>();
		internal List<RegionLocation> RegionsLoaded = new List<RegionLocation>();

		internal ChunkManager chunkManager
		{
			get
			{
				return world.chunkManager;
			}
		}
		internal EntityLocation location
		{
			get
			{
				return physics.Location;
			}
			set
			{
				physics.Location = value;
			}
		}

		/// <summary>
		/// This is a list of players this player can see
		/// </summary>
		internal List<Entity> HasSpawned = new List<Entity>();

		public override string name
		{
			get { return username; }
		}

		/// <summary>
		/// The networking side of the player class
		/// </summary>
		internal Client client;
		internal string username;

		internal float ExpBar = 0;
		internal short Level = 0;
		internal short TotalExp = 0;

        public bool isInCreative { get { return _isInCreative; } set { client.ChangeGameState(3, (byte)(value ? 1 : 0)); _isInCreative = value; } }
        private bool _isInCreative = false;
        internal bool isCrouched = false;
		internal bool isSprinting = false;
		internal bool isEating = false; //Eating Drinking Blocking
		internal bool isInvisible = false;

		public PlayerWindowManager WindowManager;

		internal Player(Client c, World w)
		{
			world = w;
			physics = new Physics(new EntityLocation(world.Spawn.playerLocation), this, PhysicsType.Gravity);
			client = c;
			WindowManager = new PlayerWindowManager(this);
		}

		public override void PhysicsCall()
		{

		}

		public override byte[] GenerateMetaData()
		{
			List<byte> bytes = new List<byte>();
			
			bytes.Add((byte)EntityMetaDataTypes.Float | (byte)0x06); //Set the flag for FLOAT | HEALTH
			bytes.AddRange(DBC.GetBytes(Health)); //Were adding the HEALTH float 
			bytes.Add(127); //End of METADATA

			return bytes.ToArray();
		}

		public void SendMessage(string s)
		{
			client.SendNonFormattedMessage(s);
		}

		public static void Announcement(string Sender, string s)
		{
			Client.GlobalAnnouncement(Sender, s);
		}

        /// <summary>
        /// Returns the number of logged in players.
        /// </summary>
        internal static int GetPlayerCount(bool ShowHiddenPlayers = true)
        {
            return players.Values.Count(p => { return p.client.loggedin; });
        }

        /// <summary>
        /// Returns the first result matching partially or exactly the input string.
        /// </summary>
        public static Player Find(string name)
        {
            name.ToLower();
            return Player.players.Values.ToList().Find(pl => { return pl.username.ToLower().IndexOf(name) != -1; });
        }

        /// <summary>
        /// Returns a list contianing all partial or exact matches from the input string.
        /// </summary>
        public static Player FindExact(string name)
        {
            name.ToLower();
            return Player.players.Values.ToList().Find(pl => { return pl.username.ToLower() == name; });
        }
        public static Player FindExact(string name, bool casesensitive)
        {
            if (!casesensitive) return FindExact(name);
            return Player.players.Values.ToList().Find(pl => { return pl.username == name; });
        }

        /// <summary>
        /// Returns a list contianing all partial or exact matches from the input string.
        /// </summary>
        public static List<Player> FindAll(string name)
        {
            name.ToLower();
            return Player.players.Values.ToList().FindAll(pl => { return pl.username.ToLower().IndexOf(name) != -1; });
        }

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			Entity e = obj as Entity;
			if (e == null) return false;

			if (e.EId == EId) return true;

			return false;
		}
	}
}
