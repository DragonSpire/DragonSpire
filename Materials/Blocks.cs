namespace DragonSpire.Blocks
{
	public class Air : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Air"; } }
		public override short ID { get { return 0; } }

		public override bool isSolid { get { return false; } }
		public override byte Opacity { get { return 0x0; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return -1; } }
	}
	public class Stone : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Stone"; } }
		public override short ID { get { return 1; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 1.5F; } }

		public override bool OnBreak(BlockLocation block, World w)
		{
			w.GlobalSpawnEntityObject(new ItemStack(new SLOT(MaterialManager.Materials[MaterialEnum.Cobblestone], 1, 0, new byte[0]), block.playerLocation));
			return true; //Return that YES this block should be broken in the world
		}
		public override bool OnBreak(BlockLocation block, Player p, World w)
		{
			w.GlobalSpawnEntityObject(
				new ItemStack
					(
					new SLOT
						(MaterialManager.Materials[MaterialEnum.Cobblestone], 1, 0, new byte[0]),
						block.playerLocation
					)
				);
			return true; //Return that YES this block should be broken in the world
		}
	}
	public class Grass : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Grass"; } }
		public override short ID { get { return 2; } }

		public override float BlastResistance { get { return 3F; } }
		public override float MiningHardness { get { return 0.6F; } }

		public override bool OnBreak(BlockLocation block, World w)
		{
			w.GlobalSpawnEntityObject(new ItemStack(new SLOT(MaterialManager.Materials[MaterialEnum.Dirt], 1, 0, new byte[0]), block.playerLocation));
			return true; //Return that YES this block should be broken in the world
		}
		public override bool OnBreak(BlockLocation block, Player p, World w)
		{
			w.GlobalSpawnEntityObject(
				new ItemStack
					(
					new SLOT
						(MaterialManager.Materials[MaterialEnum.Dirt], 1, 0, new byte[0]), 
						block.playerLocation
					)
				);
			return true; //Return that YES this block should be broken in the world
		}
	}
	public class Dirt : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Dirt"; } }
		public override short ID { get { return 3; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }

		public override bool OnBreak(BlockLocation block, World w)
		{
			w.GlobalSpawnEntityObject(new ItemStack(new SLOT(MaterialManager.Materials[MaterialEnum.Dirt], 1, 0, new byte[0]), block.playerLocation));
			return true; //Return that YES this block should be broken in the world
		}
		public override bool OnBreak(BlockLocation block, Player p, World w)
		{
			w.GlobalSpawnEntityObject(
				new ItemStack
					(
					new SLOT
						(MaterialManager.Materials[MaterialEnum.Dirt], 1, 0, new byte[0]),
						block.playerLocation
					)
				);
			return true; //Return that YES this block should be broken in the world
		}
	}
	public class CobbleStone : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "CobbleStone"; } }
		public override short ID { get { return 4; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 2F; } }

		public override bool OnBreak(BlockLocation block, World w)
		{
			w.GlobalSpawnEntityObject(new ItemStack(new SLOT(MaterialManager.Materials[MaterialEnum.Cobblestone], 1, 0, new byte[0]), block.playerLocation));
			return true; //Return that YES this block should be broken in the world
		}
		public override bool OnBreak(BlockLocation block, Player p, World w)
		{
			w.GlobalSpawnEntityObject(
				new ItemStack
					(
					new SLOT
						(MaterialManager.Materials[MaterialEnum.Cobblestone], 1, 0, new byte[0]),
						block.playerLocation
					)
				);
			return true; //Return that YES this block should be broken in the world
		}
	}
	public class WoodenPlanks : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "WoodenPlanks"; } }
		public override short ID { get { return 5; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class Sapling : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Sapling"; } }
		public override short ID { get { return 6; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class Bedrock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Bedrock"; } }
		public override short ID { get { return 7; } }

		public override float BlastResistance { get { return 18000000F; } }
		public override float MiningHardness { get { return -1; } }
	}
	public class FlowingWater : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "FlowingWater"; } }
		public override short ID { get { return 8; } }

		public override float BlastResistance { get { return 500F; } }
		public override float MiningHardness { get { return -1; } }
	}
	public class Water : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Water"; } }
		public override short ID { get { return 9; } }

		public override float BlastResistance { get { return 500F; } }
		public override float MiningHardness { get { return -1; } }
	}
	public class FlowingLava : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "FlowingLava"; } }
		public override short ID { get { return 10; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return -1; } }
	}
	public class Lava : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Lava"; } }
		public override short ID { get { return 11; } }

		public override float BlastResistance { get { return 500F; } }
		public override float MiningHardness { get { return -1; } }
	}
	public class Sand : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Sand"; } }
		public override short ID { get { return 12; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class Gravel : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Gravel"; } }
		public override short ID { get { return 13; } }

		public override float BlastResistance { get { return 3F; } }
		public override float MiningHardness { get { return 0.6F; } }
	}
	public class GoldOre : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "GoldOre"; } }
		public override short ID { get { return 14; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class IronOre : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "IronOre"; } }
		public override short ID { get { return 15; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class CoalOre : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "CoalOre"; } }
		public override short ID { get { return 16; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class Log : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Log"; } }
		public override short ID { get { return 17; } }

		public override float BlastResistance { get { return 10F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class Leaves : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Leaves"; } }
		public override short ID { get { return 18; } }

		public override float BlastResistance { get { return 1F; } }
		public override float MiningHardness { get { return 0.2F; } }
	}
	public class Sponge : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Sponge"; } }
		public override short ID { get { return 19; } }

		public override float BlastResistance { get { return 3F; } }
		public override float MiningHardness { get { return 0.6F; } }
	}
	public class Glass : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Glass"; } }
		public override short ID { get { return 20; } }

		public override float BlastResistance { get { return 1.5F; } }
		public override float MiningHardness { get { return 0.3F; } }
	}
	public class LapisLazuliOre : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "LapisLazuliOre"; } }
		public override short ID { get { return 21; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class LapisLazuliBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "LapisLazuliBlock"; } }
		public override short ID { get { return 22; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class Dispenser : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Dispenser"; } }
		public override short ID { get { return 23; } }

		public override float BlastResistance { get { return 17.5F; } }
		public override float MiningHardness { get { return 3.5F; } }
	}
	public class Sandstone : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Sandstone"; } }
		public override short ID { get { return 24; } }

		public override float BlastResistance { get { return 4F; } }
		public override float MiningHardness { get { return 0.8F; } }
	}
	public class NoteBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "NoteBlock"; } }
		public override short ID { get { return 25; } }

		public override float BlastResistance { get { return 4F; } }
		public override float MiningHardness { get { return 0.8F; } }
	}
	public class Bed : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Bed"; } }
		public override short ID { get { return 26; } }

		public override float BlastResistance { get { return 1F; } }
		public override float MiningHardness { get { return 0.2F; } }
	}
	public class PoweredRail : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "PoweredRail"; } }
		public override short ID { get { return 27; } }

		public override float BlastResistance { get { return 3.5F; } }
		public override float MiningHardness { get { return 0.7F; } }
	}
	public class DetectorRail : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "DetectorRail"; } }
		public override short ID { get { return 28; } }

		public override float BlastResistance { get { return 3.5F; } }
		public override float MiningHardness { get { return 0.7F; } }
	}
	public class StickyPiston : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "StickyPiston"; } }
		public override short ID { get { return 29; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class Cobweb : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Cobweb"; } }
		public override short ID { get { return 30; } }

		public override float BlastResistance { get { return 20F; } }
		public override float MiningHardness { get { return 4F; } }
	}
	public class TallGrass : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "TallGrass"; } }
		public override short ID { get { return 31; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class DeadBush : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "DeadBush"; } }
		public override short ID { get { return 32; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class Piston : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Piston"; } }
		public override short ID { get { return 33; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class PistonExtension : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "PistonExtension"; } }
		public override short ID { get { return 34; } }

		//Couldn't location hardness of this block.
		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return -1; } }
	}
	public class Wool : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Wool"; } }
		public override short ID { get { return 35; } }

		public override float BlastResistance { get { return 4F; } }
		public override float MiningHardness { get { return 0.8F; } }
	}
	public class BlockMovedByPiston : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "BlockMovedByPiston"; } }
		public override short ID { get { return 36; } }

		//Couldn't location hardness of this block.
		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return -1; } }
	}
	public class Dandelion : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Dandelion"; } }
		public override short ID { get { return 37; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class Rose : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Rose"; } }
		public override short ID { get { return 38; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class BrownMushroom : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "BrownMushroom"; } }
		public override short ID { get { return 39; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class RedMushroom : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "RedMushroom"; } }
		public override short ID { get { return 40; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class GoldBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "GoldBlock"; } }
		public override short ID { get { return 41; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class IronBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "IronBlock"; } }
		public override short ID { get { return 42; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 5F; } }
	}
	public class DoubleSlabs : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "DoubleSlabs"; } }
		public override short ID { get { return 43; } }

		//Double check this.
		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class Slab : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Slab"; } }
		public override short ID { get { return 44; } }

		//Double check this.
		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class Bricks : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Bricks"; } }
		public override short ID { get { return 45; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class TNT : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "TNT"; } }
		public override short ID { get { return 46; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class Bookshelf : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Bookshelf"; } }
		public override short ID { get { return 47; } }

		public override float BlastResistance { get { return 7.5F; } }
		public override float MiningHardness { get { return 1.5F; } }
	}
	public class MossStone : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "MossStone"; } }
		public override short ID { get { return 48; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class Obsidian : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Obsidian"; } }
		public override short ID { get { return 49; } }

		public override float BlastResistance { get { return 6000F; } }
		public override float MiningHardness { get { return 50F; } }
	}
	public class Torch : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Torch"; } }
		public override short ID { get { return 50; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class Fire : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Fire"; } }
		public override short ID { get { return 51; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class MonsterSpawner : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "MonsterSpawner"; } }
		public override short ID { get { return 52; } }

		public override float BlastResistance { get { return 25F; } }
		public override float MiningHardness { get { return 5F; } }
	}
	public class WoodStairs : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "WoodStairs"; } }
		public override short ID { get { return 53; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class Chest : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Chest"; } }
		public override short ID { get { return 54; } }

		public override float BlastResistance { get { return 12.5F; } }
		public override float MiningHardness { get { return 2.5F; } }
	}
	public class RedstoneWire : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "RedstoneWire"; } }
		public override short ID { get { return 55; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class DiamondOre : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "DiamondOre"; } }
		public override short ID { get { return 56; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class DiamondBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "DiamondBlock"; } }
		public override short ID { get { return 57; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 5F; } }
	}
	public class CraftingTable : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "CraftingTable"; } }
		public override short ID { get { return 58; } }

		public override float BlastResistance { get { return 12.5F; } }
		public override float MiningHardness { get { return 2.5F; } }
	}
	public class WheatStalks : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "WheatStalks"; } }
		public override short ID { get { return 59; } }

		//Double check this.
		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class Farmland : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Farmland"; } }
		public override short ID { get { return 60; } }

		public override float BlastResistance { get { return 3F; } }
		public override float MiningHardness { get { return 0.6F; } }
	}
	public class Furnace : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Furnace"; } }
		public override short ID { get { return 61; } }

		public override float BlastResistance { get { return 17.5F; } }
		public override float MiningHardness { get { return 3.5F; } }
	}
	public class BurningFurnace : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "BurningFurnace"; } }
		public override short ID { get { return 62; } }

		public override float BlastResistance { get { return 17.5F; } }
		public override float MiningHardness { get { return 3.5F; } }
	}
	public class SignPost : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "SignPost"; } }
		public override short ID { get { return 63; } }

		public override float BlastResistance { get { return 5F; } }
		public override float MiningHardness { get { return 1F; } }
	}
	public class WoodenDoor : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "WoodenDoor"; } }
		public override short ID { get { return 64; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class Ladders : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Ladders"; } }
		public override short ID { get { return 65; } }

		public override float BlastResistance { get { return 2F; } }
		public override float MiningHardness { get { return 0.4F; } }
	}
	public class Rail : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Rail"; } }
		public override short ID { get { return 66; } }

		public override float BlastResistance { get { return 3.5F; } }
		public override float MiningHardness { get { return 0.7F; } }
	}
	public class CobbleStairs : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "CobbleStairs"; } }
		public override short ID { get { return 67; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class WallSign : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "WallSign"; } }
		public override short ID { get { return 68; } }

		public override float BlastResistance { get { return 5F; } }
		public override float MiningHardness { get { return 1F; } }
	}
	public class Lever : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Lever"; } }
		public override short ID { get { return 69; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class StonePressurePlate : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "StonePressurePlate"; } }
		public override short ID { get { return 70; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class IronDoor : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "IronDoor"; } }
		public override short ID { get { return 71; } }

		public override float BlastResistance { get { return 25F; } }
		public override float MiningHardness { get { return 5F; } }
	}
	public class WoodenPressurePlate : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "WoodenPressurePlate"; } }
		public override short ID { get { return 72; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class RedstoneOre : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "RedstoneOre"; } }
		public override short ID { get { return 73; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class GlowingRedstoneOre : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "GlowingRedstoneOre"; } }
		public override short ID { get { return 74; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class InactiveRedstoneTorch : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "InactiveRedstoneTorch"; } }
		public override short ID { get { return 75; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class RedstoneTorch : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "RedstoneTorch"; } }
		public override short ID { get { return 76; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class StoneButton : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "StoneButton"; } }
		public override short ID { get { return 77; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class Snow : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Snow"; } }
		public override short ID { get { return 78; } }

		public override float BlastResistance { get { return 0.5F; } }
		public override float MiningHardness { get { return 0.1F; } }
	}
	public class Ice : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Ice"; } }
		public override short ID { get { return 79; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class SnowBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "SnowBlock"; } }
		public override short ID { get { return 80; } }

		public override float BlastResistance { get { return 1F; } }
		public override float MiningHardness { get { return 0.2F; } }
	}
	public class Cactus : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Cactus"; } }
		public override short ID { get { return 81; } }

		public override float BlastResistance { get { return 2F; } }
		public override float MiningHardness { get { return 0.4F; } }
	}
	public class ClayBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "ClayBlock"; } }
		public override short ID { get { return 82; } }

		public override float BlastResistance { get { return 3F; } }
		public override float MiningHardness { get { return 0.6F; } }
	}
	public class SugarCane : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "SugarCane"; } }
		public override short ID { get { return 83; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class Jukebox : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Jukebox"; } }
		public override short ID { get { return 84; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class Fence : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Fence"; } }
		public override short ID { get { return 85; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class Pumpkin : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Pumpkin"; } }
		public override short ID { get { return 86; } }

		public override float BlastResistance { get { return 5F; } }
		public override float MiningHardness { get { return 1F; } }
	}
	public class Netherrack : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Netherrack"; } }
		public override short ID { get { return 87; } }

		public override float BlastResistance { get { return 2F; } }
		public override float MiningHardness { get { return 0.4F; } }
	}
	public class SoulSand : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "SoulSand"; } }
		public override short ID { get { return 88; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class GlowstoneBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "GlowstoneBlock"; } }
		public override short ID { get { return 89; } }

		public override float BlastResistance { get { return 1.5F; } }
		public override float MiningHardness { get { return 0.3F; } }
	}
	public class NetherPortal : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "NetherPortal"; } }
		public override short ID { get { return 90; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return -1; } }
	}
	public class JackOLantern : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "JackOLantern"; } }
		public override short ID { get { return 91; } }

		public override float BlastResistance { get { return 5F; } }
		public override float MiningHardness { get { return 1F; } }
	}
	public class CakeBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "CakeBlock"; } }
		public override short ID { get { return 92; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class InactiveRedstoneRepeater : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "InactiveRedstoneRepeater"; } }
		public override short ID { get { return 93; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class ActiveRedstoneRepeater : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "ActiveRedstoneRepeater"; } }
		public override short ID { get { return 94; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class LockedChest : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "LockedChest"; } }
		public override short ID { get { return 95; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 2.5F; } }
	}
	public class Trapdoor : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Trapdoor"; } }
		public override short ID { get { return 96; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class MonsterEggStone : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "MonsterEggStone"; } }
		public override short ID { get { return 97; } }

		public override float BlastResistance { get { return 3.75F; } }
		public override float MiningHardness { get { return 0.75F; } }
	}
	public class StoneBrick : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "StoneBrick"; } }
		public override short ID { get { return 98; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 1.5F; } }
	}
	public class HugeBrownMushroom : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "HugeBrownMushroom"; } }
		public override short ID { get { return 99; } }

		public override float BlastResistance { get { return 1F; } }
		public override float MiningHardness { get { return 0.2F; } }
	}
	public class HugeRedMushroom : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "HugeRedMushroom"; } }
		public override short ID { get { return 100; } }

		public override float BlastResistance { get { return 1F; } }
		public override float MiningHardness { get { return 0.2F; } }
	}
	public class IronBars : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "IronBars"; } }
		public override short ID { get { return 101; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 5F; } }
	}
	public class GlassPane : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "GlassPane"; } }
		public override short ID { get { return 102; } }

		public override float BlastResistance { get { return 1.5F; } }
		public override float MiningHardness { get { return 0.3F; } }
	}
	public class Melon : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Melon"; } }
		public override short ID { get { return 103; } }

		public override float BlastResistance { get { return 5F; } }
		public override float MiningHardness { get { return 1F; } }
	}
	public class PumpkinStem : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "PumpkinStem"; } }
		public override short ID { get { return 104; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class MelonStem : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "MelonStem"; } }
		public override short ID { get { return 105; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class Vines : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Vines"; } }
		public override short ID { get { return 106; } }

		public override float BlastResistance { get { return 1F; } }
		public override float MiningHardness { get { return 0.2F; } }
	}
	public class FenceGate : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "FenceGate"; } }
		public override short ID { get { return 107; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class BrickStairs : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "BrickStairs"; } }
		public override short ID { get { return 108; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class StoneBrickStairs : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "StoneBrickStairs"; } }
		public override short ID { get { return 109; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 1.5F; } }
	}
	public class Mycelium : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Mycelium"; } }
		public override short ID { get { return 110; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.6F; } }
	}
	public class LilyPad : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "LilyPad"; } }
		public override short ID { get { return 111; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class NetherBrick : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "NetherBrick"; } }
		public override short ID { get { return 112; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class NetherBrickFence : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "NetherBrickFence"; } }
		public override short ID { get { return 113; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class NetherBrickStairs : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "NetherBrickStairs"; } }
		public override short ID { get { return 114; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class NetherWart : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "NetherWart"; } }
		public override short ID { get { return 115; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class EnchantmentTable : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "EnchantmentTable"; } }
		public override short ID { get { return 116; } }

		public override float BlastResistance { get { return 6000F; } }
		public override float MiningHardness { get { return 5F; } }
	}
	public class BrewingStand : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "BrewingStand"; } }
		public override short ID { get { return 117; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class Cauldron : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Cauldron"; } }
		public override short ID { get { return 118; } }

		public override float BlastResistance { get { return 10F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class EndPortal : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "EndPortal"; } }
		public override short ID { get { return 119; } }

		public override float BlastResistance { get { return 18000000F; } }
		public override float MiningHardness { get { return -1; } }
	}
	public class EndPortalFrame : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "EndPortalFrame"; } }
		public override short ID { get { return 120; } }

		public override float BlastResistance { get { return 18000000F; } }
		public override float MiningHardness { get { return -1; } }
	}
	public class EndStone : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "EndStone"; } }
		public override short ID { get { return 121; } }

		public override float BlastResistance { get { return 45F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class DragonEgg : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "DragonEgg"; } }
		public override short ID { get { return 122; } }

		public override float BlastResistance { get { return 45F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class InactiveRedstoneLamp : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "InactiveRedstoneLamp"; } }
		public override short ID { get { return 123; } }

		public override float BlastResistance { get { return 1.5F; } }
		public override float MiningHardness { get { return 0.3F; } }
	}
	public class ActiveRedstoneLamp : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "ActiveRedstoneLamp"; } }
		public override short ID { get { return 124; } }

		public override float BlastResistance { get { return 1.5F; } }
		public override float MiningHardness { get { return 0.3F; } }
	}
	public class WoodenDoubleSlabs : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "WoodenDoubleSlabs"; } }
		public override short ID { get { return 125; } }

		//Double check this.
		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class WoodenSlab : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "WoodenSlab"; } }
		public override short ID { get { return 126; } }

		//Double check this.
		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class CocoaPod : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "CocoaPod"; } }
		public override short ID { get { return 127; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 0.2F; } }
	}
	public class SandstoneStairs : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "SandstoneStairs"; } }
		public override short ID { get { return 128; } }

		public override float BlastResistance { get { return 4F; } }
		public override float MiningHardness { get { return 0.8F; } }
	}
	public class EmeraldOre : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "EmeraldOre"; } }
		public override short ID { get { return 129; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class EnderChest : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "EnderChest"; } }
		public override short ID { get { return 130; } }

		public override float BlastResistance { get { return 3000F; } }
		public override float MiningHardness { get { return 22.5F; } }
	}
	public class TripWireHook : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "TripWireHook"; } }
		public override short ID { get { return 131; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class TripWire : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "TripWire"; } }
		public override short ID { get { return 132; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class EmeraldBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "EmeraldBlock"; } }
		public override short ID { get { return 133; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 5F; } }
	}
	public class SpruceWoodStairs : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "SpruceWoodStairs"; } }
		public override short ID { get { return 134; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class BirchWoodStairs : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "BirchWoodStairs"; } }
		public override short ID { get { return 135; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class JungleWoodStairs : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "JungleWoodStairs"; } }
		public override short ID { get { return 136; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class CommandBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "CommandBlock"; } }
		public override short ID { get { return 137; } }

		public override float BlastResistance { get { return 18000000F; } }
		public override float MiningHardness { get { return -1; } }
	}
	public class Beacon : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Beacon"; } }
		public override short ID { get { return 138; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class CobblestoneWall : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "CobblestoneWall"; } }
		public override short ID { get { return 139; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 2F; } }
	}
	public class FlowerPot : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "FlowerPot"; } }
		public override short ID { get { return 140; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class Carrots : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Carrots"; } }
		public override short ID { get { return 141; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class Potatoes : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Potatoes"; } }
		public override short ID { get { return 142; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class WoodenButton : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "WoodenButton"; } }
		public override short ID { get { return 143; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class Head : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Head"; } }
		public override short ID { get { return 144; } }

		public override float BlastResistance { get { return 5F; } }
		public override float MiningHardness { get { return 1F; } }
	}
	public class Anvil : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Anvil"; } }
		public override short ID { get { return 145; } }

		public override float BlastResistance { get { return 6000F; } }
		public override float MiningHardness { get { return 5F; } }
	}
	public class TrappedChest : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "TrappedChest"; } }
		public override short ID { get { return 146; } }

		public override float BlastResistance { get { return 12.5F; } }
		public override float MiningHardness { get { return 2.5F; } }
	}
	public class LightWeightedPressurePlate : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "LightWeightedPressurePlate"; } }
		public override short ID { get { return 147; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class HeavyWeightedPressurePlate : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "HeavyWeightedPressurePlate"; } }
		public override short ID { get { return 148; } }

		public override float BlastResistance { get { return 2.5F; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class InactiveRedstoneComparator : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "InactiveRedstoneComparator"; } }
		public override short ID { get { return 149; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class ActiveRedstoneComparator : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "ActiveRedstoneComparator"; } }
		public override short ID { get { return 150; } }

		public override float BlastResistance { get { return 0F; } }
		public override float MiningHardness { get { return 0F; } }
	}
	public class DaylightSensor : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "DaylightSensor"; } }
		public override short ID { get { return 151; } }

		public override float BlastResistance { get { return 1F; } }
		public override float MiningHardness { get { return 0.2F; } }
	}
	public class RedstoneBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "RedstoneBlock"; } }
		public override short ID { get { return 152; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 5F; } }
	}
	public class NetherQuartzOre : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "NetherQuartzOre"; } }
		public override short ID { get { return 153; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class Hopper : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Hopper"; } }
		public override short ID { get { return 154; } }

		public override float BlastResistance { get { return 15F; } }
		public override float MiningHardness { get { return 3F; } }
	}
	public class QuartzBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "QuartzBlock"; } }
		public override short ID { get { return 155; } }

		public override float BlastResistance { get { return 4F; } }
		public override float MiningHardness { get { return 0.8F; } }
	}
	public class QuartzStairs : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "QuartzStairs"; } }
		public override short ID { get { return 156; } }

		public override float BlastResistance { get { return 4F; } }
		public override float MiningHardness { get { return 0.8F; } }
	}
	public class ActivatorRail : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "ActivatorRail"; } }
		public override short ID { get { return 157; } }

		public override float BlastResistance { get { return 3.5F; } }
		public override float MiningHardness { get { return 0.7F; } }
	}
	public class Dropper : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Dropper"; } }
		public override short ID { get { return 158; } }

		public override float BlastResistance { get { return 17.5F; } }
		public override float MiningHardness { get { return 3.5F; } }
	}
	public class StainedClay : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "StainedClay"; } }
		public override short ID { get { return 159; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 1.25F; } }
	}
	public class HayBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "HayBlock"; } }
		public override short ID { get { return 170; } }

		//Couldn't locate resistance for this block.
		public override float BlastResistance { get { return -1; } }
		public override float MiningHardness { get { return 0.5F; } }
	}
	public class Carpet : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Carpet"; } }
		public override short ID { get { return 171; } }

		//Couldn't locate resistance for this block.
		public override float BlastResistance { get { return -1; } }
		public override float MiningHardness { get { return 0.1F; } }
	}
	public class HardenedClay : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "HardenedClay"; } }
		public override short ID { get { return 172; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 1.25F; } }
	}
	public class CoalBlock : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "CoalBlock"; } }
		public override short ID { get { return 173; } }

		public override float BlastResistance { get { return 30F; } }
		public override float MiningHardness { get { return 5F; } }
	}
	public class Nothing : Block
	{
		public override dynamic DirectAccess
		{
			get { return this; }
		}

		public override string Name { get { return "Nothing"; } }
		public override short ID { get { return -1; } }

		public override float BlastResistance { get { return -1; } }
		public override float MiningHardness { get { return -1; } }
	}
}