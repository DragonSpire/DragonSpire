using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire
{
	public class WorldGeneratorUtils
	{
		public static void GenerateTree(Chunk C, int X, byte Y, int Z, byte TrunkHeight, short TrunkType, short LeavesType)
		{
			byte Radius = (byte)new Random().Next(2, 4);
			GenerateTower(TrunkType, 0, C, X, Y, Z, TrunkHeight);
			GenerateSphere(LeavesType, 0, C, X, (byte)((Y + TrunkHeight) + 2), Z, Radius);
		}

		public static void GenerateOre(BaseDecorator Decorator, Random R, Chunk C, int VeinsPerChunk, int MinY, int MaxY)
		{
			for (int I = 0; I < VeinsPerChunk; I++)
			{
				int X = R.Next(16);
				byte Y = (byte)R.Next(MinY, MaxY);
				int Z = R.Next(16);
				Decorator.Decorate(C, X, Y, Z);
			}
		}

		public static void GenerateFlower(BaseDecorator Decorator, Random R, Chunk C, int FlowersPerChunk)
		{
			for (int I = 0; I < FlowersPerChunk; I++)
			{
				int X = C.CL.X + R.Next(16) + 8;
				byte Y = (byte)R.Next(128);
				int Z = C.CL.Z + R.Next(16) + 8;
				Decorator.Decorate(C, X, Y, Z);
			}
		}

		public static void GenerateTower(short ID, byte Meta, Chunk C, int X, byte Y, int Z, byte Height)
		{
			for (int I = Y; I < Y + Height; I++)
			{
				C.SetBlock(new BlockLocation(X, (byte)I, Z, C.World), MaterialManager.GetMaterial(ID), Meta);
			}
		}

		public static void GenerateHalfCircle(short ID, byte Meta, Chunk C, int X, byte Y, int Z, byte Radius)
		{
			for (int x = X - Radius; x <= X + Radius; x++)
			{
				for (int y = Y - Radius; y <= Y + Radius; y++)
				{
					if (new BlockLocation(X, Y, Z, C.World).GetDistance(new BlockLocation(x, (byte)y, Z, C.World)) <= Radius)
					{
						C.SetBlock(new BlockLocation(x, (byte)y, Z, C.World), MaterialManager.GetMaterial(ID), Meta);
					}
				}
			}
		}

		public static void GenerateSphere(short ID, byte Meta, Chunk C, int X, byte Y, int Z, byte Radius)
		{
			for (int j = Convert.ToInt16(-Radius); j <= Radius; j = (short)(j + 1))
			{
				if ((X + j) < 0 || (X + j) > 16) continue;
				for (int k = Convert.ToInt16(-Radius); k <= Radius; k = (short)(k + 1))
				{
					if ((Y + k) < 0 || (Y + k) > 255) continue;
					for (int m = Convert.ToInt16(-Radius); m <= Radius; m = (short)(m + 1))
					{
						if ((Z + m) < 0 || (Z + m) > 15) continue;
						int maXValue = (short)Math.Sqrt((double)(((j * j) + (k * k)) + (m * m)));
						if ((maXValue < (Radius + 1)))
						{
							try
							{
								int X2 = X + j;
								int Y2 = Y + k;
								int Z2 = Z + m;
								if (X2 <= 15 && Y2 <= 255 && Z2 <= 15)
								{
									if (!C.GetBlock(new BlockLocation(X + j, (byte)(Y + k), Z + m, C.World)).Equals(MaterialManager.GetBlock(7)) && !C.GetBlock(new BlockLocation(X + j, (byte)(Y + k), Z + m, C.World)).Equals(MaterialManager.GetBlock(17)))
									{
										C.SetBlock(new BlockLocation(X + j, (byte)(Y + k), Z + m, C.World), MaterialManager.GetMaterial(ID), 0);
									}
								}
							}
							catch { }
						}
					}
				}
			}
		}
	}
}