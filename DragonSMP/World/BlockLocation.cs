using System;

namespace DragonSpire
{
	public struct BlockLocation
	{
		private int _x;
		private byte _y;
		private int _z;
		private World _world;
		//private ChunkSection _chunkSection;
		private ChunkLocation _chunkLocation;
		private EntityLocation _playerLocation;

		public int X
		{
			get
			{
				return _x;
			}
		}
		public byte Y
		{
			get
			{
				return _y;
			}
		}
		public int Z
		{
			get
			{
				return _z;
			}
		}
		public World world
		{
			get { return _world; }
		}
		//internal Chunk chunk
		//{
		//    get { return _chunkLocation.chunk; }
		//}
		//internal ChunkSection chunkSection
		//{
		//    get { return _chunkSection; }
		//}
		public ChunkLocation chunkLocation
		{
			get { return _chunkLocation; }
		}
		public EntityLocation playerLocation
		{
			get
			{
				if (_playerLocation == null)
				{
					_playerLocation = new EntityLocation(_x, _y, _z, _world);
				}
				return _playerLocation;
			}
		}

		internal BlockLocation(World w)
		{
			_x = 0;
			_y = 0;
			_z = 0;
			_world = w;
			//_chunk = null;
			//_chunkSection = null;
			_chunkLocation = new ChunkLocation(w);
			_playerLocation = new EntityLocation(w);

			CalculateAdditionalData();
		}
		internal BlockLocation(int x, byte y, int z, World w)
		{
			_x = x;
			_y = y;
			_z = z;
			_world = w;
			//_chunk = null;
			//_chunkSection = null;
			_chunkLocation = new ChunkLocation(w);
			_playerLocation = new EntityLocation(w);

			CalculateAdditionalData();
		}

		void CalculateAdditionalData()
		{
			int cx = (int)Math.Floor((double)_x / 16); //Get hte x/z in chunk coordinates, converting via math.floor to make sure were getting lower bounds
			int cz = (int)Math.Floor((double)_z / 16);
			//if (_x < 0) cx--; //If BL_X or BL_Z are negative then we need to go DOWN one more chunk (because 0 0 is a positive only chunk)
			//if (_z < 0) cz--;
			_chunkLocation = _chunkLocation.MoveTo(cx, cz, world); //Get the chunk based on the cx and cz variables defined above
			
			_playerLocation = _playerLocation.MoveTo(X, Y, Y, Z, true);
		}

		public BlockLocation OffsetBy(BlockLocation L) //Add both location together to get the resulting location
		{
			_x = L.X + _x;
			_y = (byte)(L.Y + _y);
			_z = L.Z + _z;

			CalculateAdditionalData();

			return this;
		}
		public BlockLocation GetOffset(BlockLocation L) //Subract primary from secondary to get the offset of the two location (directional distance)
		{
			return new BlockLocation(L.X - X, (byte)(L.Y - Y), L.Z - Z, world);
		}
		public float GetDistance(BlockLocation L) //TODO Verify this is correct, should it be sqrt or cubed-rt?
		{
			BlockLocation Delta = GetOffset(L);

			return (float)Math.Sqrt(Delta.X * Delta.X + Delta.Y * Delta.Y + Delta.Z * Delta.Z);
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			BlockLocation L = (BlockLocation)obj;
			return (X == L.X && Y == L.Y && Z == L.Z && world == L.world); //Return the result of comparison
		}
		public static bool operator ==(BlockLocation BL, BlockLocation BL2)
		{
			return (BL.X == BL2.X && BL.Y == BL2.Y && BL.Z == BL2.Z);
		}
		public static bool operator !=(BlockLocation BL, BlockLocation BL2)
		{
			return (BL.X != BL2.X || BL.Y != BL2.Y || BL.Z != BL2.Z);
		}
		public override string ToString()
		{
			return _x + " " + _y + " " + _z + " " + _world.Name;
		}
	}
}