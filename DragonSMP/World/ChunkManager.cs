using System;
using System.Collections.Generic;

namespace DragonSpire
{
	class ChunkManager
	{
		internal static byte RegionSize = 3;
		internal static byte RegionSizeOffset = (byte)((RegionSize - 1) / 2);
		internal static byte RegionViewDistanceOffset = 1;

		Dictionary<ChunkLocation, Chunk> Chunks = new Dictionary<ChunkLocation, Chunk>();
		List<RegionLocation> LoadedRegions = new List<RegionLocation>();

		World world;
		WorldGenerator Generator;

		internal ChunkManager(World w, WorldGeneratorType type)
		{
			world = w;

			switch (type)
			{
				case WorldGeneratorType.Flat:
					Generator = new FlatWorldGenerator(world);
					break;
				case WorldGeneratorType.FlatOre:
					Generator = new FlatOreWorldGenerator(world);
					break;
			}

			LoadStartingChunks();
		}

		internal void LoadStartingChunks()
		{
			LoadRegion(-1, -1);
			LoadRegion(-1, 0);
			LoadRegion(-1, 1);
			LoadRegion(0, -1);
			LoadRegion(0, 0);
			LoadRegion(0, 1);
			LoadRegion(1, -1);
			LoadRegion(1, 0);
			LoadRegion(1, 1);
		}

		//TODO redo this to check for generated chunks and only generate chunks that are not generated D:
		//Since we currently do not unload them this is fine, but when we DO unload them we want to 
		//be able to reload them, not just generate new ones :)
		void LoadChunk(ChunkLocation l, bool Persistant)
		{
			if (!Chunks.ContainsKey(l))
			{
				Chunk chunk = new Chunk(world, l);
				chunk.isPersistant = Persistant;
				Generator.Generate(chunk);
				chunk.isLoaded = true;
				if (!Chunks.ContainsKey(l))
					Chunks.Add(l, chunk);
			}
		}
		void LoadChunk(int x, int z, bool Persistant)
		{
			LoadChunk(new ChunkLocation(x, z, world), Persistant);
		}

		void LoadRegion(int x, int z) //The location of the region (center of region)
		{
			int start_x = (x * 3) - RegionSizeOffset;
			int end_x = (x * 3) + RegionSizeOffset;
			int start_z = (z * 3) - RegionSizeOffset;
			int end_z = (z * 3) + RegionSizeOffset;

			for (int lx = start_x; lx <= end_x; lx++)
			{
				for (int lz = start_z; lz <= end_z; lz++)
				{
					LoadChunk(lx, lz, false);
				}
			}
		}
		void LoadRegion(RegionLocation rl)
		{
			LoadRegion(rl.X, rl.Z);
		}
		void LoadRegionContainingChunk(ChunkLocation cl)
		{
			LoadRegion(cl.regionLocation);
		}

		internal Chunk GetChunkAt(ChunkLocation Cl)
		{
			if (!Chunks.ContainsKey(Cl))
				LoadRegion(Cl.regionLocation); //load the entire region
			return Chunks[Cl];
		}

		internal List<Chunk> GetRegionChunks(RegionLocation RL)
		{
			List<Chunk> chunks = new List<Chunk>();

			int x = RL.X * RegionSize;
			int z = RL.Z * RegionSize;

			var C = new ChunkLocation(x, z, world);

			if (C.regionLocation != RL) Server.Log("Region location mismatch!", LogTypesEnum.Critical);
			//else Server.Log("Region Location Checks out!", LogTypesEnum.System);

			int start_x = (x) - RegionSizeOffset;
			int start_z = (z) - RegionSizeOffset;
			int end_x = (x) + RegionSizeOffset;
			int end_z = (z) + RegionSizeOffset;

			for (int lx = start_x; lx <= end_x; lx++)
			{
				for (int lz = start_z; lz <= end_z; lz++)
				{
					var CL = new ChunkLocation(lx, lz, world);
					//Console.WriteLine("Adding Chunk: " + CL.ToString());
					chunks.Add(GetChunkAt(CL));
				}
			}

			return chunks;
		}
		internal List<Chunk> GetRegionChunks(Chunk c)
		{
			return GetRegionChunks(c.CL.regionLocation);
		}

		internal List<Player> GetVisiblePlayers(RegionLocation RL)
		{
			List<Player> Players = new List<Player>();

			for (int x = 0 - RegionViewDistanceOffset; x <= RegionViewDistanceOffset; x++)
			{
				for (int z = 0 - RegionViewDistanceOffset; z <= RegionViewDistanceOffset; z++)
				{
					int Loop_X = RL.X + x;
					int Loop_Z = RL.Z + z;

					RegionLocation CRL = new RegionLocation(Loop_X, Loop_Z, world);

					foreach (Chunk c in GetRegionChunks(CRL))
					{
						Players.AddRange(c.Players.Values);
					}
				}
			}

			return Players;
		}
		internal List<Mob> GetVisibleMobs(RegionLocation RL)
		{
			List<Mob> Mobs = new List<Mob>();

			for (int x = 0 - RegionViewDistanceOffset; x <= RegionViewDistanceOffset; x++)
			{
				for (int z = 0 - RegionViewDistanceOffset; z <= RegionViewDistanceOffset; z++)
				{
					int Loop_X = RL.X + x;
					int Loop_Z = RL.X + z;

					RegionLocation CRL = new RegionLocation(Loop_X, Loop_Z, world);

					foreach (Chunk c in GetRegionChunks(RL))
					{
						Mobs.AddRange(c.Mobs.Values);
					}
				}
			}

			return Mobs;
		}
		internal List<ObjectEntity> GetVisibleObjects(RegionLocation RL)
		{
			List<ObjectEntity> Objects = new List<ObjectEntity>();

			for (int x = 0 - RegionViewDistanceOffset; x <= RegionViewDistanceOffset; x++)
			{
				for (int z = 0 - RegionViewDistanceOffset; z <= RegionViewDistanceOffset; z++)
				{
					int Loop_X = RL.X + x;
					int Loop_Z = RL.X + z;

					RegionLocation CRL = new RegionLocation(Loop_X, Loop_Z, world);

					foreach (Chunk c in GetRegionChunks(RL))
					{
						Objects.AddRange(c.Objects.Values);
					}
				}
			}

			return Objects;
		}

		internal List<Player> GetRegionPlayers(RegionLocation RL)
		{
			List<Player> Players = new List<Player>();

			foreach (Chunk c in GetRegionChunks(RL))
			{
				Players.AddRange(c.Players.Values);
			}

			return Players;
		}
		internal List<Mob> GetRegionMobs(RegionLocation RL)
		{
			List<Mob> Mobs = new List<Mob>();

			foreach (Chunk c in GetRegionChunks(RL))
			{
				Mobs.AddRange(c.Mobs.Values);
			}

			return Mobs;
		}
		internal List<ObjectEntity> GetRegionObjects(RegionLocation RL)
		{
			List<ObjectEntity> Objects = new List<ObjectEntity>();

			foreach (Chunk c in GetRegionChunks(RL))
			{
				Objects.AddRange(c.Objects.Values);
			}

			return Objects;
		}
	}
}