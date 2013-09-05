using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire
{
	static class HeightMapGenerator
	{
		internal static void GenerateHeightMap(Chunk c)
		{
			HeightMap HM = new HeightMap(16, c.CL.X, c.CL.Z);

			var heights = HM.GetOverlappedHeights();
			Console.WriteLine(heights.Length);
		}
	}
}
