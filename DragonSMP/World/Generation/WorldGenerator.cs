using System;
namespace DragonSpire
{
	public abstract class WorldGenerator
	{
		/// <summary>
		/// Used to generate a chunk.
		/// </summary>
		/// <param name="Chunk">The chunk to generate.</param>
		public abstract void Generate(Chunk Chunk);
	}

	public class FlatWorldGenerator : WorldGenerator
	{
		private World World;

		public FlatWorldGenerator(World World)
		{
			this.World = World;
		}

		public override void Generate(Chunk Chunk)
		{
			for (int cs = 0; cs < 16; cs++)
			{
				ChunkSection ChunkPart = Chunk.ChunkParts[cs];
				int i = 0;
				for (int y = 0; y < 16; y++)
				{
					for (int z = 0; z < 16; z++)
					{
						for (int x = 0; x < 16; x++)
						{
							int ty = y + (16 * cs); //this is the totalY (spanned across chunk sections

							if (ty > 30)
							{
								ChunkPart.Blocks[i] = 0;
							}
							else
							{
								if (ty.Equals(30))
								{
									ChunkPart.Blocks[i] = 2;
								}
								else if (ty <= 29 && ty > 25)
								{
									ChunkPart.Blocks[i] = 3;
								}
								else if (ty.Equals(0))
								{
									ChunkPart.Blocks[i] = 7;
								}
								else
								{
									ChunkPart.Blocks[i] = 1;
								}
							}

							//tick our block number
							i++;
						}
					}
				}
			}
			WorldGeneratorUtils.GenerateTree(Chunk, 4, 31, 4, 3, 17, 18);
		}
	}

	public class FlatOreWorldGenerator : WorldGenerator
	{
		private World World;
		private Random R;

		public FlatOreWorldGenerator(World World)
		{
			this.World = World;
			R = new Random(World.Seed);
		}

		public override void Generate(Chunk Chunk)
		{
			for (int cs = 0; cs < 16; cs++)
			{
				ChunkSection ChunkPart = Chunk.ChunkParts[cs];
				int i = 0;
				for (int y = 0; y < 16; y++)
				{
					for (int z = 0; z < 16; z++)
					{
						for (int x = 0; x < 16; x++)
						{
							int ty = y + (16 * cs); //this is the totalY (spanned across chunk sections

							if (ty > 30)
							{
								ChunkPart.Blocks[i] = 0;
							}
							else
							{
								if (ty.Equals(30))
								{
									ChunkPart.Blocks[i] = 2;
								}
								else if (ty <= 29 && ty > 25)
								{
									ChunkPart.Blocks[i] = 3;
								}
								else if (ty.Equals(0))
								{
									ChunkPart.Blocks[i] = 7;
								}
								else
								{
									ChunkPart.Blocks[i] = 1;
								}
							}

							//tick our block number
							i++;
						}
					}
				}
			}
			if (new Random().NextDouble() > 0.7)
			{
				WorldGeneratorUtils.GenerateFlower(new FlowerDecorator(R, 37), R, Chunk, 8);
			}
			else
			{
				WorldGeneratorUtils.GenerateFlower(new FlowerDecorator(R, 38), R, Chunk, 8);
			}
			WorldGeneratorUtils.GenerateFlower(new StalkDecorator(R, 81), R, Chunk, 1);
			WorldGeneratorUtils.GenerateOre(new OreDecorator(R, 16, 16), R, Chunk, 20, 0, 128);
			WorldGeneratorUtils.GenerateOre(new OreDecorator(R, 15, 8), R, Chunk, 20, 0, 64);
			WorldGeneratorUtils.GenerateOre(new OreDecorator(R, 14, 8), R, Chunk, 2, 0, 32);
			WorldGeneratorUtils.GenerateOre(new OreDecorator(R, 73, 7), R, Chunk, 8, 0, 16);
			WorldGeneratorUtils.GenerateOre(new OreDecorator(R, 21, 6), R, Chunk, 1, 16, 16);
			WorldGeneratorUtils.GenerateOre(new OreDecorator(R, 56, 7), R, Chunk, 1, 0, 16);
		}
	}

	public class SquareWorldGenerator : WorldGenerator
	{
		private World World;
		private Random R;

		public SquareWorldGenerator(World World)
		{
			this.World = World;
			R = new Random(World.Seed);
		}

		public override void Generate(Chunk Chunk)
		{
			for (int cs = 0; cs < 16; cs++)
			{
				ChunkSection ChunkPart = Chunk.ChunkParts[cs];
				int i = 0;
				for (int y = 0; y < 16; y++)
				{
					for (int z = 0; z < 16; z++)
					{
						for (int x = 0; x < 16; x++)
						{
							int ty = y + (16 * cs); //this is the totalY (spanned across chunk sections

							if (ty > 30)
							{
								ChunkPart.Blocks[i] = 0;
							}
							else
							{
								if (ty.Equals(30))
								{
									if (z < 3 || x < 3)
									{
										//Randomly set redstone lamps
										if (z.Equals(1) && R.NextDouble() > .7 || x.Equals(1) && R.NextDouble() > .7)
										{
											ChunkPart.Blocks[i] = 124;
										}
										else
										{
											ChunkPart.Blocks[i] = 5;
										}
									}
									else
									{
										ChunkPart.Blocks[i] = 2;
									}
								}
								else if (ty <= 29 && ty > 25)
								{
									ChunkPart.Blocks[i] = 3;
								}
								else if (ty.Equals(0))
								{
									ChunkPart.Blocks[i] = 7;
								}
								else
								{
									ChunkPart.Blocks[i] = 1;
								}
							}

							//tick our block number
							i++;
						}
					}
				}
			}
		}
	}
}