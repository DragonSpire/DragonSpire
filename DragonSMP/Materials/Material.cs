using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DragonSpire
{
	public static class MaterialManager
	{
		/// <summary>
		/// This dictionary contains links to all blocks/items in the server, including custom ones
		/// </summary>
		public static Dictionary<short, Material> Materials = new Dictionary<short, Material>();

		/// <summary>
		/// This will load all the materials included in the server (note that this will dump the entire materials list first!
		/// </summary>
		internal static void LoadMaterials()
		{
			Server.Log("Loading Materials...", LogTypesEnum.System);
			
			Materials.Clear();

			LoadBlocks();
			LoadItems();

			Server.Log(MaterialManager.Materials.Count + " Materials loaded!", LogTypesEnum.System);

			#region Unneeded Material Initializers
			//new Blocks.Air();
			//new Blocks.Stone();
			//new Blocks.Grass();
			//new Blocks.Dirt();
			//new Blocks.CobbleStone();
			//new Blocks.WoodenPlanks();
			//new Blocks.Sapling();
			//new Blocks.Bedrock();
			//new Blocks.FlowingWater();
			//new Blocks.Water();
			//new Blocks.FlowingLava();
			//new Blocks.Lava();
			//new Blocks.Sand();
			//new Blocks.Gravel();
			//new Blocks.GoldOre();
			//new Blocks.IronOre();
			//new Blocks.CoalOre();
			//new Blocks.Log();
			//new Blocks.Leaves();
			//new Blocks.Sponge();
			//new Blocks.Glass();
			//new Blocks.LapisLazuliOre();
			//new Blocks.LapisLazuliBlock();
			//new Blocks.Dispenser();
			//new Blocks.Sandstone();
			//new Blocks.NoteBlock();
			//new Blocks.Bed();
			//new Blocks.PoweredRail();
			//new Blocks.DetectorRail();
			//new Blocks.StickyPiston();
			//new Blocks.Cobweb();
			//new Blocks.TallGrass();
			//new Blocks.DeadBush();
			//new Blocks.Piston();
			//new Blocks.PistonExtension();
			//new Blocks.Wool();
			//new Blocks.BlockMovedByPiston();
			//new Blocks.Dandelion();
			//new Blocks.Rose();
			//new Blocks.BrownMushroom();
			//new Blocks.RedMushroom();
			//new Blocks.GoldBlock();
			//new Blocks.IronBlock();
			//new Blocks.DoubleSlabs();
			//new Blocks.Slab();
			//new Blocks.Bricks();
			//new Blocks.TNT();
			//new Blocks.Bookshelf();
			//new Blocks.MossStone();
			//new Blocks.Obsidian();
			//new Blocks.Torch();
			//new Blocks.Fire();
			//new Blocks.MonsterSpawner();
			//new Blocks.WoodStairs();
			//new Blocks.Chest();
			//new Blocks.RedstoneWire();
			//new Blocks.DiamondOre();
			//new Blocks.DiamondBlock();
			//new Blocks.CraftingTable();
			//new Blocks.WheatStalks();
			//new Blocks.Farmland();
			//new Blocks.Furnace();
			//new Blocks.BurningFurnace();
			//new Blocks.SignPost();
			//new Blocks.WoodenDoor();
			//new Blocks.Ladders();
			//new Blocks.Rail();
			//new Blocks.CobbleStairs();
			//new Blocks.WallSign();
			//new Blocks.Lever();
			//new Blocks.StonePressurePlate();
			//new Blocks.IronDoor();
			//new Blocks.WoodenPressurePlate();
			//new Blocks.RedstoneOre();
			//new Blocks.GlowingRedstoneOre();
			//new Blocks.InactiveRedstoneTorch();
			//new Blocks.RedstoneTorch();
			//new Blocks.StoneButton();
			//new Blocks.Snow();
			//new Blocks.Ice();
			//new Blocks.SnowBlock();
			//new Blocks.Cactus();
			//new Blocks.ClayBlock();
			//new Blocks.SugarCane();
			//new Blocks.Jukebox();
			//new Blocks.Fence();
			//new Blocks.Pumpkin();
			//new Blocks.Netherrack();
			//new Blocks.SoulSand();
			//new Blocks.GlowstoneBlock();
			//new Blocks.NetherPortal();
			//new Blocks.JackOLantern();
			//new Blocks.CakeBlock();
			//new Blocks.InactiveRedstoneRepeater();
			//new Blocks.ActiveRedstoneRepeater();
			//new Blocks.LockedChest();
			//new Blocks.Trapdoor();
			//new Blocks.MonsterEggStone();
			//new Blocks.StoneBrick();
			//new Blocks.HugeBrownMushroom();
			//new Blocks.HugeRedMushroom();
			//new Blocks.IronBars();
			//new Blocks.GlassPane();
			//new Blocks.Melon();
			//new Blocks.PumpkinStem();
			//new Blocks.MelonStem();
			//new Blocks.Vines();
			//new Blocks.FenceGate();
			//new Blocks.BrickStairs();
			//new Blocks.StoneBrickStairs();
			//new Blocks.Mycelium();
			//new Blocks.LilyPad();
			//new Blocks.NetherBrick();
			//new Blocks.NetherBrickFence();
			//new Blocks.NetherBrickStairs();
			//new Blocks.NetherWart();
			//new Blocks.EnchantmentTable();
			//new Blocks.BrewingStand();
			//new Blocks.Cauldron();
			//new Blocks.EndPortal();
			//new Blocks.EndPortalFrame();
			//new Blocks.EndStone();
			//new Blocks.DragonEgg();
			//new Blocks.InactiveRedstoneLamp();
			//new Blocks.ActiveRedstoneLamp();
			//new Blocks.WoodenDoubleSlabs();
			//new Blocks.WoodenSlab();
			//new Blocks.CocoaPod();
			//new Blocks.SandstoneStairs();
			//new Blocks.EmeraldOre();
			//new Blocks.EnderChest();
			//new Blocks.TripWireHook();
			//new Blocks.TripWire();
			//new Blocks.EmeraldBlock();
			//new Blocks.SpruceWoodStairs();
			//new Blocks.BirchWoodStairs();
			//new Blocks.JungleWoodStairs();
			//new Blocks.CommandBlock();
			//new Blocks.Beacon();
			//new Blocks.CobblestoneWall();
			//new Blocks.FlowerPot();
			//new Blocks.Carrots();
			//new Blocks.Potatoes();
			//new Blocks.WoodenButton();
			//new Blocks.Head();
			//new Blocks.Anvil();
			//new Blocks.TrappedChest();
			//new Blocks.LightWeightedPressurePlate();
			//new Blocks.HeavyWeightedPressurePlate();
			//new Blocks.InactiveRedstoneComparator();
			//new Blocks.ActiveRedstoneComparator();
			//new Blocks.DaylightSensor();
			//new Blocks.RedstoneBlock();
			//new Blocks.NetherQuartzOre();
			//new Blocks.Hopper();
			//new Blocks.QuartzBlock();
			//new Blocks.QuartzStairs();
			//new Blocks.ActivatorRail();
			//new Blocks.Dropper();
			//new Blocks.StainedClay();
			//new Blocks.HayBlock();
			//new Blocks.Carpet();
			//new Blocks.HardenedClay();
			//new Blocks.CoalBlock();
			//new Blocks.Nothing();
			

			//new Items.IronShovel();
			//new Items.IronPickaxe();
			//new Items.IronAxe();
			//new Items.FlintSteel();
			//new Items.Apple();
			//new Items.Bow();
			//new Items.Arrow();
			//new Items.Coal();
			//new Items.Diamond();
			//new Items.IronIngot();
			//new Items.GoldIngot();
			//new Items.IronSword();
			//new Items.WoodenSword();
			//new Items.WoodenShovel();
			//new Items.WoodenPickaxe();
			//new Items.WoodenAxe();
			//new Items.StoneSword();
			//new Items.StoneShovel();
			//new Items.StonePickaxe();
			//new Items.StoneAxe();
			//new Items.DiamondSword();
			//new Items.DiamondShovel();
			//new Items.DiamondPickaxe();
			//new Items.DiamondAxe();
			//new Items.Stick();
			//new Items.Bowl();
			//new Items.MushroomStew();
			//new Items.GoldenSword();
			//new Items.GoldenShovel();
			//new Items.GoldenPickaxe();
			//new Items.GoldenAxe();
			//new Items.String();
			//new Items.Feather();
			//new Items.Gunpowder();
			//new Items.WoodenHoe();
			//new Items.StoneHoe();
			//new Items.IronHoe();
			//new Items.DiamondHoe();
			//new Items.GoldenHoe();
			//new Items.Seeds();
			//new Items.Wheat();
			//new Items.Bread();
			//new Items.LeatherCap();
			//new Items.LeatherTunic();
			//new Items.LeatherPants();
			//new Items.LeatherBoots();
			//new Items.ChainHelmet();
			//new Items.ChainChestplate();
			//new Items.ChainLeggings();
			//new Items.ChainBoots();
			//new Items.IronHelmet();
			//new Items.IronChestplate();
			//new Items.IronLeggings();
			//new Items.IronBoots();
			//new Items.DiamondHelmet();
			//new Items.DiamondChestplate();
			//new Items.DiamondLeggings();
			//new Items.DiamondBoots();
			//new Items.GoldenHelmet();
			//new Items.GoldenChestplate();
			//new Items.GoldenLeggings();
			//new Items.GoldenBoots();
			//new Items.Flint();
			//new Items.RawPorkchop();
			//new Items.CookedPorkchop();
			//new Items.Painting();
			//new Items.GoldenApple();
			//new Items.Sign();
			//new Items.WoodenDoor();
			//new Items.Bucket();
			//new Items.WaterBucket();
			//new Items.LavaBucket();
			//new Items.Minecart();
			//new Items.Saddle();
			//new Items.IronDoor();
			//new Items.Redstone();
			//new Items.Snowball();
			//new Items.Boat();
			//new Items.Leather();
			//new Items.Milk();
			//new Items.Brick();
			//new Items.Clay();
			//new Items.SugarCane();
			//new Items.Paper();
			//new Items.Book();
			//new Items.Slimeball();
			//new Items.MinecartChest();
			//new Items.MinecartFurnace();
			//new Items.Egg();
			//new Items.Compass();
			//new Items.FishingRod();
			//new Items.Clock();
			//new Items.GlowstoneDust();
			//new Items.RawFish();
			//new Items.CookedFish();
			//new Items.Dye();
			//new Items.Bone();
			//new Items.Sugar();
			//new Items.Cake();
			//new Items.Bed();
			//new Items.RedstoneRepeater();
			//new Items.Cookie();
			//new Items.Map();
			//new Items.Shears();
			//new Items.Melon();
			//new Items.PumpkinSeeds();
			//new Items.MelonSeeds();
			//new Items.RawBeef();
			//new Items.Steak();
			//new Items.RawChicken();
			//new Items.CookedChicken();
			//new Items.RottenFlesh();
			//new Items.EnderPearl();
			//new Items.BlazeRod();
			//new Items.GhastTear();
			//new Items.GoldNugget();
			//new Items.NetherWart();
			//new Items.Potion();
			//new Items.GlassBottle();
			//new Items.SpiderEye();
			//new Items.FermentedSpiderEye();
			//new Items.BlazePowder();
			//new Items.MagmaCream();
			//new Items.BrewingStand();
			//new Items.Cauldron();
			//new Items.EnderEye();
			//new Items.GlisteningMelon();
			//new Items.SpawnEgg();
			//new Items.EnchantingBottle();
			//new Items.FireCharge();
			//new Items.BookQuill();
			//new Items.WrittenBook();
			//new Items.Emerald();
			//new Items.ItemFrame();
			//new Items.FlowerPot();
			//new Items.Carrot();
			//new Items.Potato();
			//new Items.BakedPotato();
			//new Items.PoisonousPotato();
			//new Items.EmptyMap();
			//new Items.GoldenCarrot();
			//new Items.MobHead();
			//new Items.CarrotStick();
			//new Items.NetherStar();
			//new Items.PumpkinPie();
			//new Items.FireworkRocket();
			//new Items.FireworkStar();
			//new Items.EnchantedBook();
			//new Items.RedstoneComparator();
			//new Items.NetherBrick();
			//new Items.NetherQuartz();
			//new Items.MinecartTNT();
			//new Items.MinecartHopper();
			//new Items.IronHorseArmour();
			//new Items.GoldenHorseArmour();
			//new Items.DiamondHorseArmour();
			//new Items.Lead();
			//new Items.NameTag();
			//new Items.Disc13();
			//new Items.DiscCat();
			//new Items.DiscBlocks();
			//new Items.DiscChirp();
			//new Items.DiscFar();
			//new Items.DiscMall();
			//new Items.DiscMellohi();
			//new Items.DiscStal();
			//new Items.DiscStrad();
			//new Items.DiscWard();
			//new Items.Disc11();
			//new Items.DiscWait();
			#endregion
		}
		
		//TODO load OUR dll's first, then load dll's from the Plugins folder, secure our dll's with (maybe) hashing?

		internal static void LoadBlocks()
		{
			foreach (string fileOn in Directory.GetFiles(Directory.GetCurrentDirectory()))
			{
				FileInfo file = new FileInfo(fileOn);

				//Preliminary check, must be .dll
				if (file.Extension.Equals(".dll") || file.Extension.Equals(".exe"))
				{
					//Create a new assembly from the plugin file we're adding..
					Assembly pluginAssembly = Assembly.LoadFrom(file.Name);

					//Next we'll loop through all the Types found in the assembly
					foreach (Type pluginType in pluginAssembly.GetTypes())
					{
						if (pluginType.IsSubclassOf(typeof(Block)) && pluginType.IsPublic && !pluginType.IsAbstract)
						{
							var block = (Block)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));
							//Server.Log("BLOCK: '" + block.Name + "' : '" + block.ID + "' in " + file.Name, LogTypesEnum.Info);
						}
					}
				}
			}
		}
		internal static void LoadItems()
		{
			foreach (string fileOn in Directory.GetFiles(Directory.GetCurrentDirectory()))
			{
				FileInfo file = new FileInfo(fileOn);

				//Preliminary check, must be .dll
				if (file.Extension.Equals(".dll") || file.Extension.Equals(".exe"))
				{
					//Create a new assembly from the plugin file we're adding..
					Assembly pluginAssembly = Assembly.LoadFrom(file.Name);

					//Next we'll loop through all the Types found in the assembly
					foreach (Type pluginType in pluginAssembly.GetTypes())
					{
						if (pluginType.IsSubclassOf(typeof(Item)) && pluginType.IsPublic && !pluginType.IsAbstract)
						{
							var item = (Item)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));
							//Server.Log("ITEM: '" + item.Name + "' : '" + item.ID + "' in " + file.Name, LogTypesEnum.Info);
						}
					}
				}
			}
		}

		public static Material GetMaterial(short s)
		{
			return Materials[s];
		}
		public static Block GetBlock(short s)
		{
			if (s > 255) return (Block)Materials[255];
			return (Block)Materials[s];
		}
		public static Material GetItem(short s)
		{
			return (Item)Materials[s];
		}
	}

	/// <summary>
	/// A material is any item/block that is used ingame
	/// </summary>
	public abstract class Material
	{
		public abstract dynamic DirectAccess { get; }

		/// <summary>
		/// This is the ID of the material
		/// </summary>
		public abstract short ID { get; }
		/// <summary>
		/// This is the ID that is send to the player (IE What it will look like to players)
		/// This is useful for creating custom materials, where we have a new id in the SERVER but there is no corresponding ID on the client side
		/// 
		/// Note that you do not have to override this value unless you NEED it, it defaults to the ID of this material
		/// </summary>
		public virtual short sendID { get { return ID; } }

		/// <summary>
		/// The name of the material as it will appear to players
		/// </summary>
		public abstract string Name { get; }
		/// <summary>
		/// Here you can add alias to material names
		/// </summary>
		public virtual string[] Alias { get { return new string[0]; } }

		/// <summary>
		/// Automatically set based on material ID
		/// </summary>
		public bool isBlock { get { return ID < 256; } }

		public Material()
		{
			if (MaterialManager.Materials.ContainsKey(ID))
				MaterialManager.Materials.Remove(ID);
			MaterialManager.Materials.Add(ID, this);
		}
	}
}