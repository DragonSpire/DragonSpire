using System;

namespace DragonSpire
{
	public class ChunkSection
	{
		internal int Y;
		internal byte[] Blocks = new byte[4096];
		internal byte[] MetaData = new byte[2048];
		internal byte[] BlockLight = new byte[2048];
		internal byte[] SkyLight = new byte[2048];
		internal byte[] AddData = new byte[2048];
		internal bool isEmpty = false; //set to true initially //TODO use this?

		public ChunkSection(int Y)
		{
			this.Y = Y;
			for (int i = 0; i < 2048; i++)
			{
				MetaData[i] = 0;
				BlockLight[i] = 255;
				SkyLight[i] = 255;
			}
			//TODO Light System
		}

		internal void SetBlock(int x, int y, int z, short id, byte meta)
		{
			if (id > 255) throw (new IndexOutOfRangeException("Block value > 255!"));

			Blocks[POStoINT(x, y, z)] = (byte)id;

			//TODO metadata
		}
		internal Block GetBlock(int x, int y, int z)
		{
			return (Block)MaterialManager.Materials[Blocks[POStoINT(x, y, z)]];
		}

		int POStoINT(int x, int y, int z)
		{
			if (x < 0 || y < 0 || z < 0 || x > 15 || y > 15 || z > 15)
				throw(new IndexOutOfRangeException("Values not within bounds of CHUNK SECTION (16^3) " + x + " " + y + " " + z));

			return (y * 256) + (z * 16) + x;
		}
	}
}