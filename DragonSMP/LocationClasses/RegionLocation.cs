using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire
{

	public struct RegionLocation
	{
		public int X;
		public int Z;
		public World world;

		internal RegionLocation(int x, int z, World w)
		{
			X = x;
			Z = z;
			world = w;
		}

		/// <summary>
		/// This method will return the low chunk bounds for this region
		/// </summary>
		/// <returns>ChunkLocation that marks the low bounds of this region</returns>
		public ChunkLocation ChunkStartingLocation()
		{
			return new ChunkLocation((X * ChunkManager.RegionSize) - 1, (Z * ChunkManager.RegionSize) - 1, world);
		}
		/// <summary>
		/// This method returns the high chunk bounds for this region
		/// </summary>
		/// <returns>ChunkLocation that marks the high bounds of this region</returns>
		public ChunkLocation ChunkEndLocation()
		{
			return new ChunkLocation((X * ChunkManager.RegionSize) - 1, (Z * ChunkManager.RegionSize) - 1, world);
		}



		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			RegionLocation RL = (RegionLocation)obj;
			return (X == RL.X && Z == RL.Z);
		}
		public static bool operator ==(RegionLocation RL, RegionLocation RL2)
		{
			return (RL.X == RL2.X && RL.Z == RL2.Z);
		}
		public static bool operator !=(RegionLocation RL, RegionLocation RL2)
		{
			return (RL.X != RL2.X || RL.Z != RL2.Z);
		}

		public override string ToString()
		{
			return X + " " + Z;
		}
	}
}
