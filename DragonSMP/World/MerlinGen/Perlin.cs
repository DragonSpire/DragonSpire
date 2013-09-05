using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire
{
	public static class Perlin
	{
		private static byte[] _randomPermutations = new byte[512];
		private static byte[] _selectedPermutations = new byte[512];
		private static float[] _gradientTable = new float[512];

		private static int _seed;
		public static int Seed
		{
			get { return _seed; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Seed must be positive.");
				}

				_seed = value;

				// Generate new random permutations with this seed.
				Random random = new Random(_seed);
				random.NextBytes(_randomPermutations);

				for (int i = 0; i < 256; i++)
					_selectedPermutations[256 + i] = _selectedPermutations[i] = _randomPermutations[i];

				// Generate a new gradient table
				float[] kkf = new float[256];

				for (int i = 0; i < 256; i++)
					kkf[i] = -1.0f + 2.0f * ((float)i / 255.0f);

				for (int i = 0; i < 256; i++)
					_gradientTable[i] = kkf[_selectedPermutations[i]];

				for (int i = 256; i < 512; i++)
					_gradientTable[i] = _gradientTable[i & 255];
			}
		}

		static Perlin() { Seed = (int)Math.Abs(Environment.TickCount); }

		public static float Noise3(float x, float y, float z)
		{
			int x0 = (x > 0.0f ? (int)x : (int)x - 1);
			int y0 = (y > 0.0f ? (int)y : (int)y - 1);
			int z0 = (z > 0.0f ? (int)z : (int)z - 1);

			int X = x0 & 255;
			int Y = y0 & 255;
			int Z = z0 & 255;

			// Lower Quality
			//float u = (x - x0),
			//  	v = (y - y0),
			//  	w = (z - z0);

			// Normal Quality
			float u = NoiseMath.SCurve3(x - x0),
				v = NoiseMath.SCurve3(y - y0),
				w = NoiseMath.SCurve3(z - z0);

			// Higher Quality
			//float u = NoiseMath.SCurve5(x - x0),
			//  	v = NoiseMath.SCurve5(y - y0),
			//  	w = NoiseMath.SCurve5(z - z0);

			int A = _selectedPermutations[X] + Y, AA = _selectedPermutations[A] + Z, AB = _selectedPermutations[A + 1] + Z,
				B = _selectedPermutations[X + 1] + Y, BA = _selectedPermutations[B] + Z, BB = _selectedPermutations[B + 1] + Z;

			float a = NoiseMath.LinearInterpolate(_gradientTable[AA], _gradientTable[BA], u);
			float b = NoiseMath.LinearInterpolate(_gradientTable[AB], _gradientTable[BB], u);
			float c = NoiseMath.LinearInterpolate(a, b, v);
			float d = NoiseMath.LinearInterpolate(_gradientTable[AA + 1], _gradientTable[BA + 1], u);
			float e = NoiseMath.LinearInterpolate(_gradientTable[AB + 1], _gradientTable[BB + 1], u);
			float f = NoiseMath.LinearInterpolate(d, e, v);
			
			return NoiseMath.LinearInterpolate(c, f, w);
		}
		public static float Noise2(float x, float y)
		{
			int x0 = (x > 0.0f ? (int)x : (int)x - 1);
			int y0 = (y > 0.0f ? (int)y : (int)y - 1);

			int X = x0 & 255;
			int Y = y0 & 255;

			// Lower Quality
			//float u = (x - x0),
			//  	v = (y - y0);

			// Normal Quality
			float u = NoiseMath.SCurve3(x - x0),
				v = NoiseMath.SCurve3(y - y0);

			// Higher Quality
			//float u = NoiseMath.SCurve5(x - x0),
			//		v = NoiseMath.SCurve5(y - y0);

			int A = _selectedPermutations[X] + Y, AA = _selectedPermutations[A], AB = _selectedPermutations[A + 1],
				B = _selectedPermutations[X + 1] + Y, BA = _selectedPermutations[B], BB = _selectedPermutations[B + 1];

			float a = NoiseMath.LinearInterpolate(_gradientTable[AA], _gradientTable[BA], u);
			float b = NoiseMath.LinearInterpolate(_gradientTable[AB], _gradientTable[BB], u);
			float c = NoiseMath.LinearInterpolate(a, b, v);
			float d = NoiseMath.LinearInterpolate(_gradientTable[AA + 1], _gradientTable[BA + 1], u);
			float e = NoiseMath.LinearInterpolate(_gradientTable[AB + 1], _gradientTable[BB + 1], u);
			float f = NoiseMath.LinearInterpolate(d, e, v);
			
			return NoiseMath.LinearInterpolate(c, f, 0);
		}
	}

	internal static class NoiseMath
	{
		public static float CubicInterpolate(float n0, float n1, float n2, float n3, float a)
		{
			float p = (n3 - n2) - (n0 - n1);
			float q = (n0 - n1) - p;
			float r = n2 - n0;
			float s = n1;

			return p * a * a * a + q * a * a + r * a + s;
		}

		public static float GetMin(float a, float b)
		{
			return (a < b ? a : b);
		}
		public static float GetMax(float a, float b)
		{
			return (a > b ? a : b);
		}

		public static void SwapValues(ref float a, ref float b)
		{
			float c = a;

			a = b;
			b = c;

		}

		public static float LinearInterpolate(float n0, float n1, float a)
		{
			return ((1.0f - a) * n0) + (a * n1);
		}

		public static double MakeInt32Range(double n)
		{
			if (n >= 1073741824.0)
			{
				return ((2.0 * Math.IEEERemainder(n, 1073741824.0)) - 1073741824.0);
			}
			else if (n <= -1073741824.0)
			{
				return ((2.0 * Math.IEEERemainder(n, 1073741824.0)) + 1073741824.0);
			}
			else
			{
				return n;
			}
		}

		public static float SCurve3(float a)
		{
			return (a * a * (3.0f - 2.0f * a));
		}
		public static float SCurve5(float a)
		{
			float a3 = a * a * a;
			float a4 = a3 * a;
			float a5 = a4 * a;

			return (6.0f * a5) - (15.0f * a4) + (10.0f * a3);
		}
	}
}