using System;
using System.Linq;

namespace DragonSpire
{
	public class World
	{
		internal ChunkManager chunkManager; //This manages all the chunks in a given level
		
		internal string Name = "";
		//private long Seed;
		internal BlockLocation Spawn;
		internal Difficulty Difficulty = Difficulty.Normal;
		internal GameMode GameMode = GameMode.Survival;
		internal Dimension Dimension = Dimension.Overworld;

		internal long TimeCreated;
		internal long CurrentTime
		{
			get
			{
				return Server.TimeTicks - TimeCreated;
			}
		}
		internal long age = 6549; //TODO implement world age

		internal World(String Name, WorldGeneratorType type)
		{
			TimeCreated = Server.TimeTicks;

			this.Name = Name;
			chunkManager = new ChunkManager(this, type);
			Spawn = new BlockLocation(0, 34, 0, this);

			Server.worlds.Add(this);
		}

		internal Chunk GetChunkAt(ChunkLocation CL)
		{
			return chunkManager.GetChunkAt(CL);
		}

		internal void SetBlock(BlockLocation BL, Material mat, byte meta = 0)
		{
			Chunk C = chunkManager.GetChunkAt(BL.chunkLocation); //Get the chunk based off the blocks ChunkLocation variable
			ChunkSection CS = C.ChunkParts[(int)Math.Floor((double)BL.Y / 16)]; //get the ChunkSection based off the y location

			int x = BL.X % 16;
			int z = BL.Z % 16;

			if (x < 0) x += 16;
			if (z < 0) z += 16;

			int y = BL.Y % 16;

			CS.SetBlock(x, y, z, mat.ID, meta);

			foreach (Player p in chunkManager.GetVisiblePlayers(C.CL.regionLocation))
			{
				p.client.SendBlockChange(BL, mat, meta);
			}
			//TODO MetaData
		}
		internal Block GetBlock(BlockLocation BL)
		{
			Chunk C = chunkManager.GetChunkAt(BL.chunkLocation); //Get the chunk based off the blocks ChunkLocation variable
			ChunkSection CS = C.ChunkParts[(int)Math.Floor((double)BL.Y / 16)]; //get the ChunkSection based off the y location

			int x = BL.X % 16;
			int z = BL.Z % 16;

			if (x < 0) x += 16;
			if (z < 0) z += 16;

			int y = BL.Y % 16;

			return CS.GetBlock(x, y, z);
		}

		public void GlobalSpawnEntityObject(ObjectEntity OE)
		{
			foreach (Player p in chunkManager.GetVisiblePlayers(OE.currentChunk.CL.regionLocation))
			{
				p.client.SpawnObjectOrVehicle(OE);
			}
			OE.currentChunk.Objects.Add(OE.EId, OE);
		}
	}

	public enum WorldGeneratorType
	{
		Flat,
		FlatOre
	}
}