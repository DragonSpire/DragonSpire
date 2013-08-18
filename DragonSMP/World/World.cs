using System;
using System.Linq;

namespace DragonSpire
{
	public class World
	{
		internal ChunkManager chunkManager; //This manages all the chunks in a given level
		
		internal string Name = "";
		internal int Seed; //temporarily an int
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

			C.SetBlock(BL, mat, meta);

			foreach (Player p in chunkManager.GetVisiblePlayers(C.CL.regionLocation))
			{
				p.client.SendBlockChange(BL, mat, meta);
			}
			//TODO MetaData
		}
		internal Block GetBlock(BlockLocation BL)
		{
			Chunk C = chunkManager.GetChunkAt(BL.chunkLocation); //Get the chunk based off the blocks ChunkLocation variable

			return C.GetBlock(BL);
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