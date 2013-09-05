using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace DragonSpire
{
	public class HeightMap
	{
		public const byte Overlap = 16;

		private float[][] _heights;
		private int _size, _startX, _startY;

		#region Properties
		public int StartY
		{
			get { return _startY; }
			set { _startY = value; }
		}
		public int StartX
		{
			get { return _startX; }
			set { _startX = value; }
		}
		public float this[int x, int y]
		{
			get { return _heights[x + Overlap][y + Overlap]; }
			set { _heights[x + Overlap][y + Overlap] = value; }
		}
		public int Size
		{
			get { return _size; }
		}
		#endregion

		public HeightMap(int size, int startX = 0, int startY = 0)
		{
			if (size < 2 || (size & (size - 1)) != 0)
			{
				throw new ArgumentException("Size must be bigger than 1 and a power of 2.", "size");
			}

			int realSize = size + (Overlap * 2);

			_size = size;
			_startX = startX;
			_startY = startY;
			_heights = new float[realSize][];

			for (int i = 0; i < realSize; i++) _heights[i] = new float[realSize];
		}

		#region Public Methods

		public float[][] GetOverlappedHeights()
		{
			return _heights;
		}
		
		public void AlignEdges(HeightMap leftNeighbor, HeightMap rightNeighbor, HeightMap topNeighbor, HeightMap bottomNeighbor, int shift = 0)
		{
			int x, y, counter;
			float[][] nHeights;
			float value;
			int size = this.Size;

			if (leftNeighbor != null)
			{
				nHeights = leftNeighbor.GetOverlappedHeights();
				counter = 0;

				for (x = size + Overlap - shift; x < size + (Overlap * 2); x++)
				{
					for (y = 0; y < size; y++)
					{
						//value = (_heights[counter, y] + nHeights[x, y]) / 2f;
						//_heights[counter, y] = nHeights[x, y] = value;
						_heights[counter][y] = nHeights[x][y];
					}
					counter++;
				}
				
				x = size - 1;

				for (y = 0; y < size; y++)
				{
					//value = (this[0, y] + leftNeighbor[x, y]) / 2f;
					//this[0, y] = leftNeighbor[x, y] = value;
					this[0, y] = leftNeighbor[x, y];
				}
			}

			if (rightNeighbor != null)
			{
				nHeights = rightNeighbor.GetOverlappedHeights();
				counter = 0;

				for (x = size + Overlap - shift; x < size + (Overlap * 2); x++)
				{
					for (y = 0; y < size; y++)
					{
						//value = (_heights[x, y] + nHeights[counter, y]) / 2f;
						//_heights[x, y] = nHeights[counter, y] = value;
						_heights[x][y] = nHeights[counter][y];
					}
					counter++;
				}

				x = size - 1;

				for (y = 0; y < size; y++)
				{
					//value = (this[x, y] + rightNeighbor[0, y]) / 2f;
					//this[x, y] = rightNeighbor[0, y] = value;
					this[x, y] = rightNeighbor[0, y];
				}
			}

			if (topNeighbor != null)
			{
				nHeights = topNeighbor.GetOverlappedHeights();
				counter = 0;

				for (y = size + Overlap - shift; y < size + (Overlap * 2); y++)
				{
					for (x = 0; x < size; x++)
					{
						//value = (_heights[x, y] + nHeights[x, counter]) / 2f;
						//_heights[x, y] = nHeights[x, counter] = value;
						_heights[x][y] = nHeights[x][counter];
					}
					counter++;
				}

				y = size - 1;

				for (x = 0; x < size; x++)
				{
					//value = (this[x, y] + topNeighbor[x, 0]) / 2f;
					//this[x, y] = topNeighbor[x, 0] = value;
					this[x, y] = topNeighbor[x, 0];
				}
			}

			if (bottomNeighbor != null)
			{
				nHeights = bottomNeighbor.GetOverlappedHeights();
				counter = 0;

				for (y = size + Overlap - shift; y < size + (Overlap * 2); y++)
				{
					for (x = 0; x < size; x++)
					{
						//value = (_heights[x, counter] + nHeights[x, y]) / 2f;
						//_heights[x, counter] = nHeights[x, y] = value;
						_heights[x][counter] = nHeights[x][y];
					}
					counter++;
				}
				
				y = size - 1;

				for (x = 0; x < size; x++)
				{
					//value = (this[x, 0] + bottomNeighbor[x, y]) / 2f;
					//this[x, 0] = bottomNeighbor[x, y] = value;
					this[x, 0] = bottomNeighbor[x, y];
				}
			}
		}

		public void SetNoise(float frequency, byte octaves = 1, float persistence = 0.5f, float lacunarity = 2.0f, bool additive = false)
		{
			int size = _heights.GetLength(0);
			int startX = _startX - Overlap;
			int startY = _startY - Overlap;
			float fSize = (float)size,
				min = int.MinValue,
				max = int.MaxValue;

			Parallel.For(0, size, x =>
			{
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				byte currentOctave;
				float value, currentPersistence, signal;
				Vector2 coord;
				
				for (int y = 0; y < size; y++)
				{
					value = 0.0f;
					currentPersistence = 1.0f;
					/*coord = new Vector2(
						(x + startX) / fSize,
						(y + startY) / fSize);*/
					coord = new Vector2(
						((x + startX)),
						((y + startY)));
					//(_heights[x][y] - min) / (max - min)
					coord *= frequency;

					for (currentOctave = 0; currentOctave < octaves; currentOctave++)
					{
						signal = Perlin.Noise2(coord.X, coord.Y);
						value += signal * currentPersistence;
						coord *= lacunarity;
						currentPersistence *= persistence;
					}
					_heights[x][y] = (!additive) ? value : _heights[x][y] + value;
				}
			});
		}

		public void Perturb(float frequency, float depth)
		{
			int u, v, i, j;
			int size = _heights.GetLength(0);
			int startX = _startX - Overlap;
			int startY = _startY - Overlap;
			float[][] temp = new float[size][];
			float fSize = (float)size;
			Vector2 coord;

			for (i = 0; i < size; ++i)
			{
				temp[i] = new float[size];

				for (j = 0; j < size; ++j)
				{
					coord = new Vector2(
						(i + startX) / fSize,
						(j + startY) / fSize);

					coord *= frequency;

					u = i + (int)(Perlin.Noise3(coord.X, coord.Y, 0.0f) * depth);
					v = j + (int)(Perlin.Noise3(coord.X, coord.Y, 1.0f) * depth);

					if (u < 0) u = 0;
					if (u >= size) u = size - 1;
					if (v < 0) v = 0;
					if (v >= size) v = size - 1;

					temp[i][j] = _heights[u][v];
				}
			}

			_heights = temp;
		}

		public void Erode(float smoothness)
		{
			int size = _heights.GetLength(0);

			Parallel.For(1, size - 1, i =>
			{
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;

				int u, v;
				float d_max, d_i, d_h;
				int[] match;

				for (int j = 1; j < size - 1; j++)
				{
					d_max = 0.0f;
					match = new[] { 0, 0 };

					for (u = -1; u <= 1; u++)
					{
						for (v = -1; v <= 1; v++)
						{
							if (Math.Abs(u) + Math.Abs(v) > 0)
							{
								d_i = _heights[i][j] - _heights[i + u][j + v];

								if (d_i > d_max)
								{
									d_max = d_i;
									match[0] = u;
									match[1] = v;
								}
							}
						}
					}

					if (0 < d_max && d_max <= (smoothness / (float)size))
					{
						d_h = 0.5f * d_max;

						_heights[i][j] -= d_h;
						_heights[i + match[0]][j + match[1]] += d_h;
					}
				}
			});
		}

		public void Smoothen()
		{
			int i, j, u, v;
			int size = _heights.GetLength(0);
			float total;

			for (i = 1; i < size - 1; ++i)
			{
				for (j = 1; j < size - 1; ++j)
				{
					total = 0.0f;

					for (u = -1; u <= 1; u++)
					{
						for (v = -1; v <= 1; v++)
						{
							total += _heights[i + u][j + v];
						}
					}

					_heights[i][j] = total / 9.0f;
				}
			}
		}

		public void Normalize()
		{
			int size = _heights.GetLength(0);
			float min = -1f, max = 1f;

			/*for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					if (_heights[x, y] > max)
					{
						max = _heights[x, y];
					}
					else if (_heights[x, y] < min)
					{
						min = _heights[x, y];
					}
				}
			}

			if (min >= max) throw new Exception("Hmm...");*/

			Parallel.For(0, size, x =>
			{
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;

				for (int y = 0; y < size; y++)
				{
					_heights[x][y] = (_heights[x][y] - min) / (max - min);
				}
			});
		}

		public void MakeFlat(float height = 0.0f)
		{
			int x, y;
			int size = _heights.GetLength(0);

			for (x = 0; x < size; x++)
			{
				for (y = 0; y < size; y++)
				{
					_heights[x][y] = height;
				}
			}
		}

		public void Multiply(float amount)
		{
			int x, y;
			int size = _heights.GetLength(0);

			for (x = 0; x < size; x++)
			{
				for (y = 0; y < size; y++)
				{
					_heights[x][y] *= amount;
				}
			}
		}

		public void ForEach(Func<float, float> body)
		{
			int size = _heights.GetLength(0);

			Parallel.For(0, size, x =>
			{
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;

				for (int y = 0; y < size; y++)
				{
					_heights[x][y] = body(_heights[x][y]);
				}
			});
		}

		/*public void ForEach(Func<float, float, float, float> body)
		{
			int x, y;
			int size = _heights.GetLength(0);
		
			for (x = 0; x < size; x++)
			{
				for (y = 0; y < size; y++)
				{
					_heights[x, y] = body(x, y, _heights[x, y]);
				}
			}
		}*/
		
		#endregion
	}
}