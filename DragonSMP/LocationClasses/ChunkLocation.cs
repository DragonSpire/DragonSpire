using System;

namespace DragonSpire
{
	public struct ChunkLocation
	{
		private World _world;
		private int _x;
		private int _z;

		public World world
		{
			get
			{
				return _world;
			}
		}
		public int X
		{
			get
			{
				return _x;
			}
		}
		public int Z
		{
			get
			{
				return _z;
			}
		}

		public RegionLocation regionLocation
		{
			get
			{
				int rX = X;
				int rZ = Z;
				if (X > 0) rX += 1;
				else rX -= 1;
				if (Z > 0) rZ += 1;
				else rZ -= 1;
				return new RegionLocation(MathHelper.RTZ(rX / ChunkManager.RegionSize), MathHelper.RTZ(rZ / ChunkManager.RegionSize), world);
			}
		}

		internal ChunkLocation(World w)
		{
			_x = 0;
			_z = 0;
			_world = w;
		}
		internal ChunkLocation(int X, int Z, World w)
		{
			_x = X;
			_z = Z;
			_world = w;
		}

		internal ChunkLocation MoveTo(int x, int z, World w)
		{
			_x = x;
			_z = z;
			_world = w;

			return this;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			ChunkLocation CL = (ChunkLocation)obj;

			return (X == CL.X && Z == CL.Z);
		}
		public bool Equals(ChunkLocation CL)
		{
			return (X == CL.X && Z == CL.Z);
		}
		public static bool operator ==(ChunkLocation CL, ChunkLocation CL2)
		{
			return (CL.X == CL2.X && CL.Z == CL2.Z);
		}
		public static bool operator !=(ChunkLocation CL, ChunkLocation CL2)
		{
			return (CL.X != CL2.X || CL.Z != CL2.Z);
		}

		public override string ToString()
		{
			return X + " " + Z + " " + world.Name;
		}
	}
}