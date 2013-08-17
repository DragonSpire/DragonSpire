using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire
{
	public abstract class BaseDecorator
	{
		protected Random R;

		protected BaseDecorator(Random _R)
		{
			R = _R;
		}

		public virtual void Decorate(Chunk C, int X, byte Y, int Z)
		{
			throw new NotImplementedException();
		}

		public virtual void Decorate(Chunk C)
		{
			throw new NotImplementedException();
		}
	}

	public class HouseDecorator : BaseDecorator
	{
		public HouseDecorator(Random _R) : base(_R)
		{
		}

		public override void Decorate(Chunk C)
		{
			for (int y = 0; y < 256; y++)
			{
				for (int z = 0; z < 16; z++)
				{
					for (int x = 0; x < 16; x++)
					{
						if (y > 30)
						{
							if (y <= 38)
							{
								if (4 < x && x < 14 && z.Equals(5) || 4 < x && x < 14 && z.Equals(13) || 4 < z && z < 14 && x.Equals(5) || 4 < z && z < 14 && x.Equals(13))
								{
									C.SetBlock(new BlockLocation(x, (byte)y, z, C.World), MaterialManager.GetMaterial(98), 0);
								}

								if (y.Equals(34))
								{
									if (5 < x && x < 13 && 5 < z && z < 13)
									{
										C.SetBlock(new BlockLocation(x, (byte)y, z, C.World), MaterialManager.GetMaterial(5), 0);
									}
								}
								if (x.Equals(5) && z.Equals(9) && y < 33)
								{
									C.SetBlock(new BlockLocation(x, (byte)y, z, C.World), MaterialManager.GetMaterial(0), 0);
								}

								if (x.Equals(5) && 5 < z && z < 8 && 35 < y && y < 38 || x.Equals(5) && 10 < z && z < 13 && 35 < y && y < 38)
								{
									C.SetBlock(new BlockLocation(x, (byte)y, z, C.World), MaterialManager.GetMaterial(20), 0);
								}

								if (x.Equals(6) && z.Equals(12))
								{
									if (y < 36)
									{
										C.SetBlock(new BlockLocation(x, (byte)y, z, C.World), MaterialManager.GetMaterial(65), 0);
									}
								}
							}
							else if (y.Equals(39))
							{
								if (3 < x && x < 15 && 3 < z && z < 15)
								{
									C.SetBlock(new BlockLocation(x, (byte)y, z, C.World), MaterialManager.GetMaterial(17), 0);
								}
							}
							else if (y.Equals(40))
							{
								if (4 < x && x < 14 && 4 < z && z < 14)
								{
									C.SetBlock(new BlockLocation(x, (byte)y, z, C.World), MaterialManager.GetMaterial(17), 0);
								}
							}
							else if (y.Equals(41))
							{
								if (5 < x && x < 13 && 5 < z && z < 13)
								{
									C.SetBlock(new BlockLocation(x, (byte)y, z, C.World), MaterialManager.GetMaterial(17), 0);
								}
							}
							else if (y.Equals(42))
							{
								if (6 < x && x < 12 && 6 < z && z < 12)
								{
									C.SetBlock(new BlockLocation(x, (byte)y, z, C.World), MaterialManager.GetMaterial(17), 0);
								}
							}
						}
					}
				}
			}
		}
	}
	
	//Used for cactus and sugar cane
	public class StalkDecorator : BaseDecorator
	{
		byte ID;

		public StalkDecorator(Random _R, byte _ID) : base(_R)
		{
			ID = _ID;
		}

		public override void Decorate(Chunk C, int X, byte Y, int Z)
		{
			for (int I = 0; I < 10; I++)
			{
				int _X = X + R.Next(8) - R.Next(8);
				byte _Y = (byte)(Y + R.Next(4) - R.Next(4));
				int _Z = Z + R.Next(8) - R.Next(8);
				byte H = (byte)new Random().Next(3);

				if (ID.Equals(81) && C.GetBlock(new BlockLocation(_X, (byte)(_Y - 1), _Z, C.World)).Equals(MaterialManager.GetBlock(12)))
				{
					//Cactus
					WorldGeneratorUtils.GenerateTower(ID, 0, C, _X, _Y, _Z, H);
				}
			}
		}
	}

	public class FlowerDecorator : BaseDecorator
	{
		byte ID;
		byte Meta;

		public FlowerDecorator(Random _R, byte _ID) : base(_R)
		{
			ID = _ID;
			Meta = 0;
		}

		public FlowerDecorator(Random _R, byte _ID, byte _Meta) : base(_R)
		{
			ID = _ID;
			Meta = _Meta;
		}

		public override void Decorate(Chunk C, int X, byte Y, int Z)
		{
			for (int I = 0; I < 64; I++)
			{
				int _X = X + R.Next(8) - R.Next(8);
				byte _Y = (byte)(Y + R.Next(4) - R.Next(4));
				int _Z = Z + R.Next(8) - R.Next(8);

				if (C.GetBlock(new BlockLocation(_X, (byte)(_Y - 1), _Z, C.World)).Equals(MaterialManager.GetBlock(2)) && C.GetBlock(new BlockLocation(_X, _Y, _Z, C.World)).Equals(MaterialManager.GetBlock(0)))
				{
					C.SetBlock(new BlockLocation(_X, _Y, _Z, C.World), MaterialManager.GetMaterial(ID), Meta);
				}
			}
		}
	}

	public class OreDecorator : BaseDecorator
	{
		byte ID;
		int VeinSize;

		public OreDecorator(Random _R, byte _ID, int _VeinSize) : base(_R)
		{
			ID = _ID;
			VeinSize = _VeinSize;
		}

		public override void Decorate(Chunk C, int X, byte Y, int Z)
		{
			//TODO: Create a better Ore Decorator
			int _X = X;
			byte _Y = Y;
			int _Z = Z;
			for (int I = 0; I <= R.Next(VeinSize); I++)
			{
				if (R.NextDouble() > 0.5)
				{
					if (!I.Equals(0))
					{
						double Random = R.NextDouble();
						if (0.0 <= Random && Random <= 0.3 && _X - X < 2)
						{
							_X += 1;
						}
						else if (0.4 <= Random && Random <= 0.6 && _Y - Y < 2)
						{
							_Y += 1;
						}
						else if (0.7 <= Random && Random <= 1.0 && _Z - Z < 2)
						{
							_Z += 1;
						}
					}

					if (C.GetBlock(new BlockLocation(_X, _Y, _Z, C.World)).Equals(MaterialManager.GetBlock(1)))
					{
						C.SetBlock(new BlockLocation(_X, _Y, _Z, C.World), MaterialManager.GetMaterial(ID), 0);
					}
				}
			}
		}
	}

	public class ForestDecorator : BaseDecorator
	{
		public ForestDecorator(Random _R)
			: base(_R)
		{
		}
		public override void Decorate(Chunk C, int X, byte Y, int Z)
		{
			throw new NotImplementedException();
		}
	}
}