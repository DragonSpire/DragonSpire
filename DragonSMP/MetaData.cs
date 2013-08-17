using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire
{
	#region Block MetaData
	public enum WoodPlanks : byte
	{
		Oak = 0,
		Spruce = 1,
		Birch = 2,
		Jungle = 3
	}
	public enum Saplings : byte
	{
		Oak = 0,
		Spruce = 1,
		Birch = 2,
		Jungle = 3
	}
	public enum LiquidFlow : byte
	{
		Highest = 0x0,
		Flow1 = 0x1,
		Flow2 = 0x2,
		Flow3 = 0x3,
		Flow4 = 0x4,
		Flow5 = 0x5,
		Flow6 = 0x6,
		Lowest = 0x7,
		Falling = 0x8
	}
	public enum Logs : byte
	{
		OakUpDown = 0,
		SpruceUpDown = 1,
		BirchUpDown = 2,
		JungleUpDown = 3,
		OakEastWest = 4,
		SpruceEastWest = 5,
		BirchEastWest = 6,
		JungleEastWest = 7,
		OakNorthSouth = 8,
		SpruceNorthSouth = 9,
		BirchNorthSouth = 10,
		JungleNorthSouth = 11,
		OakEastBark = 12,
		SpruceEastBark = 13,
		BirchEastBark = 14,
		JungleEastBark = 15
	}
	public enum LeavesData : byte
	{
		Oak = 0,
		Spruce = 1,
		Birch = 2,
		Jungle = 3
	}
	public enum WoolData : byte
	{
		White = 0,
		Orange = 1,
		Magenta = 2,
		LightBlue = 3,
		Yellow = 4,
		Lime = 5,
		Pink = 6,
		Gray = 7,
		LightGray = 8,
		Cyan = 9,
		Purple = 10,
		Blue = 11,
		Brown = 12,
		Green = 13,
		Red = 14,
		Black = 15
	}
	public enum Dye : byte
	{
		InkSac = 0,
		RoseRed = 1,
		CactusGreen = 2,
		CocoaBeans = 3,
		LapisLazuli = 4,
		PurpleDye = 5,
		CyanDye = 6,
		LightGrayDye = 7,
		GrayDye = 8,
		PinkDye = 9,
		LimeDye = 10,
		DandelionYellow = 11,
		LightBlueDye = 12,
		MagentaDye = 13,
		OrangeDye = 14,
		BoneMeal = 15
	}
	public enum GoldenApple : byte
	{
		Regular = 0,
		Enchanted = 1
	}
	public enum CauldronLevel : byte
	{
		Empty = 0,
		OneThirds = 1,
		TwoThirds = 2,
		Full = 3
	}
	public enum CobblestoneWallData : byte
	{
		Regular = 0,
		Mossy = 1
	}
	public enum FlowerPots : byte
	{
		Empty = 0,
		Rose = 1,
		Dandelion = 2,
		Oak = 3,
		Spruce = 4,
		Birch = 5,
		Jungle = 6,
		RedMushroom = 7,
		BrownMushroom = 8,
		Cactus = 9,
		Shrub = 10,
		Fern = 11
	}
	public enum SkullPlacement : byte
	{
		Floor = 0x1,
		WallNorth = 0x2,
		WallSouth = 0x3,
		WallEast = 0x4,
		WallWest = 0x5
	}
	public enum Skulls : byte
	{
		Skeleton = 0,
		WitherSkeleton = 1,
		Zombie = 2,
		Human = 3,
		Creeper = 4
	}
	public enum QuartzBlocks : byte
	{
		Regular = 0,
		Chiseled = 1,
		PillarVertical = 2,
		PillarNorthSouth = 3,
		PillarEastWest = 4
	}
	public enum PotionEffectTier : byte
	{
		Tier1 = 0,
		Tier2 = 32
	}
	public enum PotionEffectDuration : byte
	{
		Base = 0,
		Extended = 64
	}
	public enum PotionType : ushort
	{
		Regular = 0,
		Splash = 16384
	}
	public enum TorchOrientation : byte
	{
		East = 0x1,
		West = 0x2,
		South = 0x3,
		North = 0x4,
		OnFloor = 0x5,
		InGround = 0x6
	}
	public enum Slabs : byte
	{
		Stone = 0x0,
		Sandstone = 0x1,
		Wooden = 0x2,
		Cobble = 0x3,
		Brick = 0x4,
		StoneBrick = 0x5,
		NetherBrick = 0x6,
		Quartz = 0x7,
		StoneDouble = 0x8,
		SandstoneDouble = 0x9,
		QuartzDouble = 0xF
	}
	public enum WoodenSlabs : byte
	{
		Oak = 0x0,
		Spruce = 0x1,
		Birch = 0x2,
		Jungle = 0x3
	}
	public enum FireData : byte
	{
		Spreading = 0x0,
		Placed = 0x0,
		NoUpdates = 0xF
	}
	public enum SandstoneData : byte
	{
		Normal = 0x0,
		Chiseled = 0x1,
		Smooth = 0x2
	}
	public enum BedDirection : byte
	{
		South = 0x0,
		West = 0x1,
		North = 0x2,
		East = 0x3
	}
	public enum TallGrassData : byte
	{
		Shrub = 0x0,
		Grass = 0x1,
		Fern = 0x2
	}
	public enum PistonDirection : byte
	{
		Down = 0,
		Up = 1,
		North = 2,
		South = 3,
		West = 4,
		East = 5
	}
	public enum StairsOrientation : byte
	{
		East = 0x0,
		West = 0x1,
		South = 0x2,
		North = 0x3,

		Regular = 0,
		UpsideDown = 1
	}
	public enum Door : byte
	{
		NorthWest = 0x0,
		NorthEast = 0x1,
		SouthEast = 0x2,
		SouthWest = 0x3
	}
	public enum PumpkinMelon : byte
	{
		South = 0x0,
		West = 0x1,
		North = 0x2,
		East = 0x3,
		None = 0x4
	}
	public enum Cake : byte
	{
		//Pieces eaten
		Zero = 0x0,
		One = 0x1,
		Two = 0x2,
		Three = 0x3,
		Four = 0x4,
		Five = 0x5
	}
	public enum TrapdoorData : byte
	{
		South = 0x0,
		North = 0x1,
		East = 0x2,
		West = 0x3
	}
	public enum MonsterStoneData : byte
	{
		Stone = 0,
		Cobblestone = 1,
		StoneBrick = 2
	}
	public enum StoneBrickData : byte
	{
		Normal = 0x0,
		Mossy = 0x1,
		Cracked = 0x2,
		Chiseled = 0x3
	}
	public enum VineData : byte
	{
		South = 1,
		West = 2,
		North = 4,
		East = 8
	}
	public enum Gates : byte
	{
		South = 0,
		West = 1,
		North = 2,
		East = 4
	}
	public enum BrewingStandData : byte
	{
		East = 0x1,
		SouthWest = 0x2,
		NorthWest = 0x4
	}
	public enum CropStages : byte
	{
		Starting = 0,
		Stage1 = 1,
		Stage2 = 2,
		Stage3 = 3,
		Stage4 = 4,
		Stage5 = 5,
		Stage6 = 6,
		Grown = 7
	}
	public enum Coal : byte
	{
		Coal = 0,
		Charcoal = 1
	}
	public enum SignPostData : byte
	{
		South = 0x0,
		South_SouthWest = 0x1,
		SouthWest = 0x2,
		West_SouthWest = 0x3,
		West = 0x4,
		West_NorthWest = 0x5,
		NorthWest = 0x6,
		North_NorthWest = 0x7,
		North = 0x8,
		North_NorthEast = 0x9,
		NorthEast = 0xA,
		East_NorthEast = 0xB,
		East = 0xC,
		East_SouthEast = 0xD,
		SouthEast = 0xE,
		South_SouthEast = 0xF
	}
	public enum RailData : byte
	{
		NorthSouth = 0x0,
		WestEast = 0x1,
		AscendingEast = 0x2,
		AscendingWest = 0x3,
		AscendingNorth = 0x4,
		AscendingSouth = 0x5,
		NorthWestCorner = 0x6,
		NorthEastCorner = 0x7,
		SouthEastCorner = 0x8,
		SouthWestCorner = 0x9
	}
	public enum LSFC : byte
	{
		North = 0x2,
		South = 0x3,
		West = 0x4,
		East = 0x5
	}
	public enum DDH : byte
	{
		Down = 0x0,
		Up = 0x1
	}
	public enum Button : byte
	{
		East = 0x1,
		West = 0x2,
		South = 0x3,
		North = 0x4
	}
	public enum JukeboxData : byte
	{
		NoDisc = 0,
		Disc13 = 1,
		DiscCat = 2,
		DiscBlocks = 3,
		DiscChirp = 4,
		DiscFar = 5,
		DiscMall = 6,
		DiscMellohi = 7,
		DiscStal = 8,
		DiscStrad = 9,
		DiscWard = 10,
		Disc11 = 11,
		DiscWait = 12
	}
	#endregion

	#region Entity MetaData
	public enum EntityMetaDataTypes
	{
		Byte = 0, //No bits of upper 3
		Short = 32, //First bit of upper 3
		Int = 64, //Second Bit
		Float = 96, //First two bits
		String16 = 128, //Third bit
		Slot = 160, //3 and 1
		LocationInts = 192, //Not currently used, but the ability remains in the code (vanilla) //Bits 3 and 2
		NULL = 224, //All three upper bits, not used (this should never be used as of 1.6.2)
	}
	public enum EntityStatus
	{
		EntityHurt = 2,
		EntityDead = 3,
		WolfTaming = 6,
		WolfTames = 7,
		WolfShakingWater = 8,
		Eating = 9,
		SheepEatingGrass = 10,
		IronGolemHandingRose = 11,
		SpawnHeartParticlesNearVillager = 12,
		SpawnAngryParticlesNearVillager = 13,
		SpawnHappyParticlesNearVillager = 14,
		SpawnMagicParticlesNearWitch = 15,
		ZombieConvertingToVillagerByShakingViolently = 16,
		FireWorkExploding = 17,
	}
	#endregion

	#region Object MetaData

	#endregion
}