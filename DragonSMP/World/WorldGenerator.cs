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
		}
	}

	public class FlatOreWorldGenerator : WorldGenerator
	{
		private World World;

		public FlatOreWorldGenerator(World World)
		{
			this.World = World;
		}

		public override void Generate(Chunk Chunk)
		{
			Random R = new Random();
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
								if (ty.Equals(31))
								{
									/*if (R.Next(x * 5).Equals(5) && R.Next(z * 5).Equals(5))
									{
										ChunkPart.Blocks[i] = 31;
									}*/
									if (R.Next(x * 2).Equals(5) && R.Next(z * 2).Equals(8))
									{
										//Generate the flower color based on a 50/50 probability
										if (R.NextDouble() > 0.7)
											ChunkPart.Blocks[i] = 37;
										else
											ChunkPart.Blocks[i] = 38;
									}
								}
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
									if (R.Next(ty * 8).Equals(0))
										ChunkPart.Blocks[i] = 16;
									else if (R.Next(ty * 13).Equals(0))
										ChunkPart.Blocks[i] = 15;
									else if (R.Next(ty * 35).Equals(0))
										ChunkPart.Blocks[i] = 14;
									else if (R.Next(ty * 50).Equals(0))
										ChunkPart.Blocks[i] = 21;
									else if (R.Next(ty * 70).Equals(0))
										ChunkPart.Blocks[i] = 73;
									else if (R.Next(ty * 110).Equals(0))
										ChunkPart.Blocks[i] = 56;
									else if (R.Next(ty * 190).Equals(0))
										ChunkPart.Blocks[i] = 129;
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