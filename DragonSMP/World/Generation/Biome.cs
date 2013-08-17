namespace DragonSpire
{
	#region Biome IDs
	public enum VanillaBiomeType : byte
	{
		Ocean = 0,
		Plains = 1,
		Desert = 2,
		ExtremeHills = 3,
		Forest = 4,
		Taiga = 5,
		Swampland = 6,
		River = 7,
		Hell = 8,
		Sky = 9,
		FrozenOcean = 10,
		FrozenRiver = 11,
		IcePlains = 12,
		IceMountains = 13,
		MushroomIsland = 14,
		MushroomIslandShore = 15,
		Beach = 16,
		DesertHills = 17,
		ForestHills = 18,
		TaigaHills = 19,
		ExtremeHillsEdge = 20,
		Jungle = 21,
		JungleHills = 22,
		Uncalculated = 255
	}
	#endregion

	public abstract class Biome
	{
		/// <summary>
		/// The id of this biome.
		/// </summary>
		public abstract byte ID { get; }

		/// <summary>
		/// The name of this biome.
		/// </summary>
		public abstract string Name { get; }

		public abstract float Temperature { get; }

		public abstract float Precipitation { get; }

		public abstract byte TopBlock { get; }

		public abstract byte FillerBlock { get; }
	}
}