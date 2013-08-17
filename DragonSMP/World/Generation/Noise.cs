using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire
{
	public class NoiseUtils
	{
		/// <summary>
		/// A faster version of Math.Floor
		/// </summary>
		/// <param name="Value">The number to floor</param>
		/// <returns>The floor of Value</returns>
		public static int FastFloor(double Value)
		{
			return Value > 0 ? (int)Value : (int)Value - 1;
		}

		public static double Normalize(double Value)
		{
			if (Value > 1.0)
				return 1.0;
			if (Value < 0.0)
				return 0.0;
			return Value;
		}
	}

	/// <summary>
	/// An implementation of the "classic" perlin noise class.
	/// Based on the example implementation provided by the creator of Perlin Noise, Ken Perlin.
	/// </summary>
	public class PerlinNoise
	{
		static double OffsetX;
		static double OffsetY;
		static double OffsetZ;
		private static int[] Permutations = new int[512];
		#region P
		static int[] P = {
						151, 160, 137, 91, 90, 15, 131, 13, 201,
						95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37,
						240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
						94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56,
						87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139,
						48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133,
						230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25,
						63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200,
						196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3,
						64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255,
						82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
						223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
						101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79,
						113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242,
						193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249,
						14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204,
						176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222,
						114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
					  };
		#endregion

		public PerlinNoise()
		{
			for (int I = 0; I < 512; I++)
			{
				Permutations[I] = P[I & 255];
			}
		}

		public PerlinNoise(Random R)
		{
			for (int I = 0; I < 256; I++)
			{
				Permutations[I] = R.Next(256);
			}

			for (int I = 0; I < 256; I++)
			{
				int U = R.Next(256 - I) + I;
				int BU = Permutations[I];

				Permutations[I] = Permutations[U];
				Permutations[U] = BU;
				Permutations[I + 256] = Permutations[I];
			}

			OffsetX = R.NextDouble() * 256;
			OffsetY = R.NextDouble() * 256;
			OffsetZ = R.NextDouble() * 256;
		}

		private static double Fade(double T)
		{
			return T * T * T * (T * (T * 6 - 15) + 10);
		}

		private static double Lerp(double T, double A, double B)
		{
			return A + T * (B - A);
		}

		private static double Gradient(int Hash, double X, double Y, double Z)
		{
			int H = Hash & 15;
			double U = Hash < 8 ? X : Y;
			double V = Hash < 4 ? Y : Hash.Equals(12) || Hash.Equals(14) ? X : Z;

			return ((H & 1).Equals(0) ? U : -U) + ((H & 2).Equals(0) ? V : -V);
		}

		/// <summary>
		/// 1D Perlin Noise Function
		/// </summary>
		public double Noise(double X)
		{
			return Noise(X, 0, 0);
		}

		/// <summary>
		/// 2D Perlin Noise Function
		/// </summary>
		public double Noise(double X, double Y)
		{
			return Noise(X, Y, 0);
		}

		/// <summary>
		/// 3D Perlin Noise Function
		/// </summary>
		public double Noise(double X, double Y, double Z)
		{
			X += OffsetX;
			Y += OffsetY;
			Z += OffsetZ;

			int XFloored = NoiseUtils.FastFloor(X);
			int YFloored = NoiseUtils.FastFloor(Y);
			int ZFloored = NoiseUtils.FastFloor(Z);

			int _XFloored = XFloored & 255;
			int _YFloored = YFloored & 255;
			int _ZFloored = ZFloored & 255;

			double FX = Fade(X);
			double FY = Fade(Y);
			double FZ = Fade(Z);

			int A = Permutations[_XFloored] + _YFloored;
			int AA = Permutations[A] + _ZFloored;
			int AB = Permutations[A + 1] + _ZFloored;
			int B = Permutations[_XFloored + 1] + _YFloored;
			int BA = Permutations[B] + _ZFloored;
			int BB = Permutations[B + 1] + _ZFloored;

			return Lerp(FZ, Lerp(FY, Lerp(FX, Gradient(Permutations[AA], _XFloored, _YFloored, _ZFloored),
											  Gradient(Permutations[BA], _XFloored - 1, _YFloored, _ZFloored)),
									 Lerp(FY, Gradient(Permutations[AB], _XFloored, _YFloored - 1, _ZFloored),
											  Gradient(Permutations[BB], _XFloored - 1, _YFloored - 1, _ZFloored))),
							Lerp(FY, Lerp(FX, Gradient(Permutations[AA + 1], _XFloored, _YFloored, _ZFloored - 1),
											  Gradient(Permutations[BA + 1], _XFloored - 1, _YFloored, _ZFloored - 1)),
									 Lerp(FY, Gradient(Permutations[AB + 1], _XFloored, _YFloored - 1, _ZFloored - 1),
											  Gradient(Permutations[BB + 1], _XFloored - 1, _YFloored - 1, _ZFloored - 1))));
		}

		/// <summary>
		/// 2D Noise with Frequency, Amplitude, and Octaves Options
		/// </summary>
		public double Noise(double X, double Y, double Frequency, double Amplitude, double Scale, int Octaves = 1)
		{
			return Noise(X, Y, 0, Frequency, Amplitude, Scale, Octaves);
		}

		/// <summary>
		/// 3D Noise with Frequency, Amplitude, and Octaves Options
		/// </summary>
		public double Noise(double X, double Y, double Z, double Frequency, double Amplitude, double Scale, int Octaves = 1)
		{
			double Result = 0;
			double _Frequency = 1;
			double _Amplitude = 1;

			X *= Scale;
			Y *= Scale;
			Z *= Scale;

			for (int I = 0; I < Octaves; I++)
			{
				Result += Noise(X * _Frequency, Y * _Frequency, Z * _Frequency) * _Amplitude;
				_Frequency *= Frequency;
				_Amplitude *= Amplitude;
			}

			if (Result < 0)
			{
				Result *= -1;
			}
			return Result;
		}
	}

	/// <summary>
	/// Implementation of Simplex Perlin Noise, which is faster than the "classic" Perlin implementation.
	/// Based on the example implementation provided by the creator of Simplex Perlin Noise, Sjef van Leeuwen.
	/// </summary>
	public class SimplexNoise
	{
		#region Grad3

		private static int[][] Grad3 = { 
                                           new int[]{1,1,0}, 
                                           new int[]{-1,1,0}, 
                                           new int[]{1,-1,0}, 
                                           new int[]{-1,-1,0}, 
                                           new int[]{1,0,1}, 
                                           new int[]{-1,0,1}, 
                                           new int[]{1,0,-1}, 
                                           new int[]{-1,0,-1}, 
                                           new int[]{0,1,1}, 
                                           new int[]{0,-1,1}, 
                                           new int[]{0,1,-1}, 
                                           new int[]{0,-1,-1} 
                                       };

		#endregion

		#region Initizalize Grad4

		private static int[][] Grad4 = { 
                                           new int[]{0,1,1,1}, 
                                           new int[]{0,1,1,-1}, 
                                           new int[]{0,1,-1,1}, 
                                           new int[]{0,1,-1,-1}, 
                                           new int[]{0,-1,1,1}, 
                                           new int[]{0,-1,1,-1}, 
                                           new int[]{0,-1,-1,1}, 
                                           new int[]{0,-1,-1,-1}, 
                                           new int[]{1,0,1,1}, 
                                           new int[]{1,0,1,-1}, 
                                           new int[]{1,0,-1,1}, 
                                           new int[]{1,0,-1,-1}, 
                                           new int[]{-1,0,1,1}, 
                                           new int[]{-1,0,1,-1}, 
                                           new int[]{-1,0,-1,1}, 
                                           new int[]{-1,0,-1,-1}, 
                                           new int[]{1,1,0,1}, 
                                           new int[]{1,1,0,-1}, 
                                           new int[]{1,-1,0,1}, 
                                           new int[]{1,-1,0,-1}, 
                                           new int[]{-1,1,0,1}, 
                                           new int[]{-1,1,0,-1}, 
                                           new int[]{-1,-1,0,1}, 
                                           new int[]{-1,-1,0,-1}, 
                                           new int[]{1,1,1,0}, 
                                           new int[]{1,1,-1,0}, 
                                           new int[]{1,-1,1,0}, 
                                           new int[]{1,-1,-1,0}, 
                                           new int[]{-1,1,1,0}, 
                                           new int[]{-1,1,-1,0}, 
                                           new int[]{-1,-1,1,0}, 
                                           new int[]{-1,-1,-1,0} 
                                       };

		#endregion

		#region Simplex

		private static int[][] Simplex = { 
											new int[]{0,1,2,3},
											new int[]{0,1,3,2},
											new int[]{0,0,0,0},
											new int[]{0,2,3,1},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{1,2,3,0}, 
											new int[]{0,2,1,3},
											new int[]{0,0,0,0},
											new int[]{0,3,1,2},
											new int[]{0,3,2,1},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{1,3,2,0}, 
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0}, 
											new int[]{1,2,0,3},
											new int[]{0,0,0,0},
											new int[]{1,3,0,2},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{2,3,0,1},
											new int[]{2,3,1,0}, 
											new int[]{1,0,2,3},
											new int[]{1,0,3,2},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{2,0,3,1},
											new int[]{0,0,0,0},
											new int[]{2,1,3,0}, 
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0}, 
											new int[]{2,0,1,3},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{3,0,1,2},
											new int[]{3,0,2,1},
											new int[]{0,0,0,0},
											new int[]{3,1,2,0}, 
											new int[]{2,1,0,3},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{0,0,0,0},
											new int[]{3,1,0,2},
											new int[]{0,0,0,0},
											new int[]{3,2,0,1},
											new int[]{3,2,1,0} 
                                         };

		#endregion

		#region P
		static int[] P = {
						151, 160, 137, 91, 90, 15, 131, 13, 201,
						95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37,
						240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
						94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56,
						87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139,
						48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133,
						230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25,
						63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200,
						196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3,
						64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255,
						82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
						223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
						101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79,
						113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242,
						193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249,
						14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204,
						176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222,
						114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
					  };
		#endregion

		static float OffsetX;
		static float OffsetY;
		static float OffsetZ;
		private static int[] Permutations = new int[512];

		public SimplexNoise(Random R)
		{
			for (int i = 0; i < 512; i++) 
				Permutations[i] = P[i & 255];

			OffsetX = (float)R.NextDouble() * 256;
			OffsetY = (float)R.NextDouble() * 256;
			OffsetZ = (float)R.NextDouble() * 256;
		}

		private static float Dot(int[] G, float X, float Y)
		{
			return G[0] * X + G[1] * Y;
		}

		private static float Dot(int[] G, float X, float Y, float Z)
		{
			return G[0] * X + G[1] * Y + G[2] * Z;
		}

		private static float Dot(int[] G, float X, float Y, float Z, float W)
		{
			return G[0] * X + G[1] * Y + G[2] * Z + G[3] * W;
		}

		/// <summary>
		/// 2D Simplex Noise
		/// </summary>
		public static float Noise(float xin, float yin)
		{
			xin += OffsetX;
			yin += OffsetY;

			float n0, n1, n2; // Noise contributions from the three corners 
			// Skew the input space to determine which simplex cell we're in 
			float F2 = (float)(0.5 * (Math.Sqrt(3.0) - 1.0));
			float s = (xin + yin) * F2; // Hairy factor for 2D 
			int i = NoiseUtils.FastFloor(xin + s);
			int j = NoiseUtils.FastFloor(yin + s);
			float g2 = (float)((3.0 - Math.Sqrt(3.0)) / 6.0);
			float t = (i + j) * g2;
			float X0 = i - t; // Unskew the cell origin back to (x,y) space 
			float Y0 = j - t;
			float x0 = xin - X0; // The x,y distances from the cell origin 
			float y0 = yin - Y0;
			// For the 2D case, the simplex shape is an equilateral triangle. 
			// Determine which simplex we are in. 
			int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords 
			if (x0 > y0)
			{
				i1 = 1; j1 = 0;
			} // lower triangle, XY order: (0,0)->(1,0)->(1,1) 
			else
			{
				i1 = 0; j1 = 1;
			} // upper triangle, YX order: (0,0)->(0,1)->(1,1) 
			// A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and 
			// a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where 
			// c = (3-sqrt(3))/6 
			float x1 = x0 - i1 + g2; // Offsets for middle corner in (x,y) unskewed coords 
			float y1 = y0 - j1 + g2;
			float x2 = x0 - 1.0f + 2.0f * g2; // Offsets for last corner in (x,y) unskewed coords 
			float y2 = y0 - 1.0f + 2.0f * g2;
			// Work out the hashed gradient indices of the three simplex corners 
			int ii = i & 255;
			int jj = j & 255;
			int gi0 = Permutations[ii + Permutations[jj]] % 12;
			int gi1 = Permutations[ii + i1 + Permutations[jj + j1]] % 12;
			int gi2 = Permutations[ii + 1 + Permutations[jj + 1]] % 12;
			// Calculate the contribution from the three corners 
			float t0 = 0.5f - x0 * x0 - y0 * y0;
			if (t0 < 0)
				n0 = 0.0f;
			else
			{
				t0 *= t0;
				n0 = t0 * t0 * Dot(Grad3[gi0], x0, y0); // (x,y) of Grad3 used for 2D gradient 
			}
			float t1 = 0.5f - x1 * x1 - y1 * y1;
			if (t1 < 0)
				n1 = 0.0f;
			else
			{
				t1 *= t1;
				n1 = t1 * t1 * Dot(Grad3[gi1], x1, y1);
			}
			float t2 = 0.5f - x2 * x2 - y2 * y2;
			if (t2 < 0)
				n2 = 0.0f;
			else
			{
				t2 *= t2;
				n2 = t2 * t2 * Dot(Grad3[gi2], x2, y2);
			}
			// Add contributions from each corner to get the final noise value. 
			// The result is scaled to return values in the interval [-1,1]. 
			float returnNoise = 70.0f * (n0 + n1 + n2);
			// make it range from 0 to 1;
			return (returnNoise + 1.0f) * 0.5f;
		}

		/// <summary> 
		/// 3D Simplex Noise. 
		/// </summary> 
		public static float Noise(float xin, float yin, float zin)
		{
			xin += OffsetX;
			yin += OffsetY;
			zin += OffsetZ;

			float n0, n1, n2, n3; // Noise contributions from the four corners 
			// Skew the input space to determine which simplex cell we're in 
			float F3 = 1.0f / 3.0f;
			float s = (xin + yin + zin) * F3; // Very nice and simple skew factor for 3D 
			int i = NoiseUtils.FastFloor(xin + s);
			int j = NoiseUtils.FastFloor(yin + s);
			int k = NoiseUtils.FastFloor(zin + s);
			float G3 = 1.0f / 6.0f; // Very nice and simple unskew factor, too 
			float t = (i + j + k) * G3;
			float X0 = i - t; // Unskew the cell origin back to (x,y,z) space 
			float Y0 = j - t;
			float Z0 = k - t;
			float x0 = xin - X0; // The x,y,z distances from the cell origin 
			float y0 = yin - Y0;
			float z0 = zin - Z0;
			// For the 3D case, the simplex shape is a slightly irregular tetrahedron. 
			// Determine which simplex we are in. 
			int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords 
			int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords 
			if (x0 >= y0)
			{
				if (y0 >= z0)
				{
					i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 1; k2 = 0;
				} // X Y Z order 
				else if (x0 >= z0)
				{
					i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 0; k2 = 1;
				} // X Z Y order 
				else
				{
					i1 = 0; j1 = 0; k1 = 1; i2 = 1; j2 = 0; k2 = 1;
				} // Z X Y order 
			}
			else
			{ // x0<y0 
				if (y0 < z0)
				{
					i1 = 0; j1 = 0; k1 = 1; i2 = 0; j2 = 1; k2 = 1;
				} // Z Y X order 
				else if (x0 < z0)
				{
					i1 = 0; j1 = 1; k1 = 0; i2 = 0; j2 = 1; k2 = 1;
				} // Y Z X order 
				else
				{
					i1 = 0; j1 = 1; k1 = 0; i2 = 1; j2 = 1; k2 = 0;
				} // Y X Z order 
			}
			// A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z), 
			// a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and 
			// a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where 
			// c = 1/6. 
			float x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords 
			float y1 = y0 - j1 + G3;
			float z1 = z0 - k1 + G3;
			float x2 = x0 - i2 + 2.0f * G3; // Offsets for third corner in (x,y,z) coords 
			float y2 = y0 - j2 + 2.0f * G3;
			float z2 = z0 - k2 + 2.0f * G3;
			float x3 = x0 - 1.0f + 3.0f * G3; // Offsets for last corner in (x,y,z) coords 
			float y3 = y0 - 1.0f + 3.0f * G3;
			float z3 = z0 - 1.0f + 3.0f * G3;
			// Work out the hashed gradient indices of the four simplex corners 
			int ii = i & 255;
			int jj = j & 255;
			int kk = k & 255;
			int gi0 = Permutations[ii + Permutations[jj + Permutations[kk]]] % 12;
			int gi1 = Permutations[ii + i1 + Permutations[jj + j1 + Permutations[kk + k1]]] % 12;
			int gi2 = Permutations[ii + i2 + Permutations[jj + j2 + Permutations[kk + k2]]] % 12;
			int gi3 = Permutations[ii + 1 + Permutations[jj + 1 + Permutations[kk + 1]]] % 12;
			// Calculate the contribution from the four corners 
			float t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
			if (t0 < 0) n0 = 0.0f;
			else
			{
				t0 *= t0;
				n0 = t0 * t0 * Dot(Grad3[gi0], x0, y0, z0);
			}
			float t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
			if (t1 < 0) n1 = 0.0f;
			else
			{
				t1 *= t1;
				n1 = t1 * t1 * Dot(Grad3[gi1], x1, y1, z1);
			}
			float t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
			if (t2 < 0) n2 = 0.0f;
			else
			{
				t2 *= t2;
				n2 = t2 * t2 * Dot(Grad3[gi2], x2, y2, z2);
			}
			float t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
			if (t3 < 0) n3 = 0.0f;
			else
			{
				t3 *= t3;
				n3 = t3 * t3 * Dot(Grad3[gi3], x3, y3, z3);
			}
			// Add contributions from each corner to get the final noise value. 
			// The result is scaled to stay just inside [-1,1] 
			return 32.0f * (n0 + n1 + n2 + n3);
		}
	}
}