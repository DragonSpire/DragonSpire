/// <summary>
/// A file for all enums that do not have an counter-part.
/// </summary>
namespace DragonSpire
{
	public enum Dimension : sbyte
	{
		Nether = -1,
		Overworld = 0,
		End = 1
	}
	public enum Difficulty : int
	{
		Peaceful = 0,
		Easy = 1,
		Normal = 2,
		Hard = 3
	}
	public enum GameMode : int
	{
		Survival = 0,
		Creative = 1,
		Adventure = 2
	}
	public enum Direction
	{
		North,
		NorthEast,
		East,
		SouthEast,
		South,
		SouthWest,
		West,
		NorthWest
	}

	public enum PacketType
	{
		KeepAlive = 0x00,
		LoginRequest = 0x01,
		HandShake = 0x02,
		ChatMessage = 0x03,
		TimeUpdate = 0x04,
		EntityEquipment = 0x05,
		SpawnPosition = 0x06,
		UseEntity = 0x07,
		UpdateHealth = 0x08,
		Respawn = 0x09,
		Player = 0x0A,
		PlayerPosition = 0x0B,
		PlayerLook = 0x0C,
		PlayerPositionAndLook = 0x0D,
		PlayerDigging = 0x0E,
		PlayerBlockPlacement = 0x0F,
		HeldItemChange = 0x10,
		UseBed = 0x11,
		Animation = 0x12,
		EntityAction = 0x13,
		SpawnNamedEntity = 0x14,
		CollectItem = 0x16,
		SpawnObjectOrVehicle = 0x17,
		SpawnMob = 0x18,
		SpawnPainting = 0x19,
		SpawnExperienceOrb = 0x1A,
		SteerVehicle = 0x1B,
		EntityVelocity = 0x1C,
		DestroyEntity = 0x1D,
		Entity = 0x1E,
		EntityRelativeMove = 0x1F,
		EntityLook = 0x20,
		EntityLookAndRelativeMove = 0x21,
		EntityTeleport = 0x22,
		EntityHeadLook = 0x23,
		EntityStatus = 0x26,
		AttachEntity = 0x27,
		EntityMetaData = 0x28,
		EntityEffect = 0x29,
		RemoveEntityEffect = 0x2A,
		SetExperience = 0x2B,
		EntityProperties = 0x2C,
		ChunkData = 0x33,
		MultiBlockChange = 0x34,
		BlockChange = 0x35,
		BlockAction = 0x36,
		BlockBreakAnimation = 0x37,
		MapChunkBulk = 0x38,
		Explosion = 0x3C,
		SoundOrParticleEffect = 0x3D,
		NamedSoundEffect = 0x3E,
		Particle = 0x3F,
		ChangeGameState = 0x46,
		SpawnGlobalEntity = 0x47,
		OpenWindow = 0x64,
		CloseWindow = 0x65,
		ClickWindow = 0x66,
		SetSlot = 0x67,
		SetWindowItems = 0x68,
		UpdateWindowProperty = 0x69,
		ConfirmTransaction = 0x6A,
		CreativeInventoryAction = 0x6B,
		EnchantItem = 0x6C,
		UpdateSign = 0x82,
		ItemData = 0x83,
		UpdateTileEntity = 0x84,
		TileEditorOpen = 0x85,
		IncrementStatistic = 0xC8,
		PlayerListItem = 0xC9,
		PlayerAbilities = 0xCA,
		TabComplete = 0xCB,
		ClientSettings = 0xCC,
		ClientStatuses = 0xCD,
		ScoreboardObjective = 0xCE,
		UpdateScore = 0xCF,
		DisplayScoreboard = 0xD0,
		Teams = 0xD1,
		PluginMessage = 0xFA,
		EncryptionKeyResponse = 0xFC,
		EncryptionKeyRequest = 0xFD,
		ServerListPing = 0xFE,
		DisconnectOrKick = 0xFF
	}
	public enum DiggingStatus
	{
		StartedDigging = 0,
		CancelledDigging = 1,
		FinishedDigging = 2,
		DropItemStack = 3,
		DropItem = 4,
		ShootArrow = 5,
		FinishEating = 5,
	}
	public enum Face
	{
		negY = 0, //Bottom
		posY = 1, //Top
		negZ = 2, //-Z
		posZ = 3, //+Z
		negX = 4, //-X
		posX = 5, //+X
	}
	public enum PlayerAnimations
	{
		NoAnimation = 0,
		SwingArm = 1,
		DamageAnimation = 2,
		LeaveBed = 3,
		EatFood = 5,
		Unknown = 102,
		Crouch = 104,
		UnCrouch = 105,
	}

	public enum EntityObjectSpawnTypes
	{
		Boat = 1,
		ItemStack = 2,
		MineCart_Normal = 10,
		MineCart_Storage = 11,
		Minecart_Powered = 12,
		ActivatedTNT = 50,
		EnderCrystal = 51,
		Arrow = 60,
		Snowball = 61,
		Egg = 62,
		ThrownEnderpearl = 65,
		WhitherSkull = 66,
		FallingObjects = 70,
		ItemFrames = 71,
		EyeOfEnder = 72,
		ThrownPotion = 73,
		FallingDragonEgg = 74,
		ThrownExpBottle = 75,
		FishingFloat = 90,
	}
	public enum EntityMobSpawnTypes
	{
		Creeper = 50,
		Skeleton = 51,
		Spider = 52,
		GiantZombie = 53,
		Zombie = 54,
		Slime = 55,
		Ghast = 56,
		ZombiePigman = 57,
		Enderman = 58,
		CaveSpider = 59,
		Silverfish = 60,
		Blaze = 61,
		MagmaCube = 62,
		EnderDragon = 63,
		Wither = 64,
		Bat = 65,
		Witch = 66,
		Pig = 90,
		Sheep = 91,
		Cow = 92,
		Chicken = 93,
		Squid = 94,
		Wolf = 95,
		Mooshroom = 96,
		Snowman = 97,
		Ocelot = 98,
		IronGolem = 99,
		Horse = 100,
		Villager = 120,
	}


	

}