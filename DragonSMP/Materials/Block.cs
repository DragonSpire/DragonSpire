namespace DragonSpire
{
	public abstract class Block : Material
	{
		#region Non-Required Parameters
		/// <summary>
		/// Whether or not this block is solid. (IE if false players are allowed to be IN it)
		/// </summary>
		public virtual bool isSolid { get { return true; } } //Can be defaulted and does not NEED to be overriden (most blocks will be solid)
		/// <summary>
		/// Whether or not this block is not translucent.
		/// </summary>
		public bool isOpaque { get { return (Opacity == 0xF); } } //Is dynamically set, cannot be overriden
		/// <summary>
		/// The opacity of this block.
		/// 0x0 - Translucent (See Through)
		/// 0xF - Opaque (Cannot See Through)
		/// Note that this will not affect things client side, but will affect how much light passes through this object
		/// </summary>
		public virtual byte Opacity { get { return 0xF; } } //Does not NEED to be overriden, defaults to opaque.
		/// <summary>
		/// Whether or not grass, flowers, etc grow on this block.
		/// </summary>
		public virtual bool isFertile { get { return false; } } //Does not NEED to be overriden, defaults to false (non-fertile)
		/// <summary>
		/// Whether or not this block can give off power.
		/// </summary>
		public virtual bool canPower { get { return false; } } //Does not NEED to be overriden, defaults to false (no poweR)
		/// <summary>
		/// Whether or not this block can burn.
		/// We don't need to make this abstract since we dynamically set the value based upon BurnRate.
		/// </summary>
		public bool canBurn { get { return (BurnRate > 0); } } //Is dynamically set, cannot be overriden
		/// <summary>
		/// How fast this block burns in ticks.
		/// 20 seconds = 1 tick
		/// 0 = Non-Burnable
		/// </summary>
		public virtual byte BurnRate { get { return 0; } } //Does not NEED to be overriden, defaults to 0 (non burnable)
		/// <summary>
		/// Whether this block gives off light, Dynamically set
		/// </summary>
		public bool isLuminescent { get { return Luminance > 0; } } //Is dynamically set, cannot be overriden
		/// <summary>
		/// How much light this block gives off.
		/// 0 - 15
		/// </summary>
		public virtual byte Luminance { get { return 0; } } //Does not NEED to be overriden, defaults to 0 (none)
		/// <summary>
		/// How slippery this block is(how much velocity it lets you keep).
		/// </summary>
		public virtual float Slipperyness { get { return 0; } } //this does not NEED to be set, defaults to full friction (not slippery)
		/// <summary>
		/// How much experience this block drops when it's destroyed.
		/// </summary>
		public virtual short ExperienceDropped { get { return 0; } } //this does not need to be set, most blocks will nto drop experiance
		#endregion
		#region Required Parameters
		/// <summary>
		/// How resistant this block is to explosions.
		/// -1 means it will not block ANY of the explosion (explosion will still weaken by distance)
		/// </summary>
		public abstract float BlastResistance { get; } //This DOES need to be set, since all blocks will vary
		/// <summary>
		/// How many hits it takes to mine this block.
		/// -1 = indestructible (also used for blocks that cant be mined, air and water for example)
		/// </summary>
		public abstract float MiningHardness { get; } //This DOES need to be set, all blocks will vary
		#endregion

		#region Methods
		/// <summary>
		/// This holds the physics of this a particular block
		/// </summary>
		/// <param name="block">The location of this block</param>
		public virtual void OnPhysics(BlockLocation block) { }

		/// <summary>
		/// This method is called when a player right clicks on this block
		/// </summary>
		/// <param name="block">This blocks position</param>
		/// <param name="p">The player that has right clicked this block</param>
		/// <returns>Returns a bool representing whether this right click should override a block placement (IE Return FALSE is the player should place the block in their hand and TRUE if the block place should be ignored)</returns>
		public virtual bool OnRightClick(BlockLocation block, Player p) { return false; }

		/// <summary>
		/// This method is called when a player begins digging this block
		/// </summary>
		/// <param name="block">The position of this block</param>
		/// <param name="p">The player that is digging this block</param>
		/// <param name="w">The World this block resides in</param>
		public virtual void OnDigging(BlockLocation block, Player p, World w) { }
		/// <summary>
		/// Called when a player breaks this block
		/// </summary>
		/// <param name="block">Position of this block</param>
		/// <param name="p">The player that broke this block</param>
		/// <param name="w">The World this block resides in</param>
		/// <returns>A bool representing whether this block breaking action should actually be done (ie returning false means this block WILL NOT be removed from the map</returns>
		public virtual bool OnBreak(BlockLocation block, Player p, World w) { return true;}
		/// <summary>
		/// Called when a player PLACES this block
		/// </summary>
		/// <param name="block">Postiion of this block</param>
		/// <param name="p">Player that placed this block</param>
		/// <param name="w">The World this block resides in</param>
		/// <returns>A bool representing whether this block breaking action should actually be done (ie returning false means this block WILL NOT be removed from the map</returns>
		public virtual bool OnPlace(BlockLocation block, Player p, World w) { return true; }

		/// <summary>
		/// This is called when this block is broken, but a player did NOT do it (ie by physics or an explosion)
		/// </summary>
		/// <param name="block">Position of this block</param>
		/// <param name="w">The World this block resides in</param>
		/// <returns>A bool representing whether this block breaking action should actually be done (ie returning false means this block WILL NOT be removed from the map</returns>
		public virtual bool OnBreak(BlockLocation block, World w) { return true; }
		/// <summary>
		/// This is called when a block is placed, but not by a player (IE by physics or by an enderman)
		/// </summary>
		/// <param name="block">Position of this block</param>
		/// <param name="w">The World this block resides in</param>
		public virtual void OnPlace(BlockLocation block, World w) { }

		/// <summary>
		/// This method is called when the block receives redstone power
		/// </summary>
		/// <param name="block">The location of this block</param>
		public virtual void OnPower(BlockLocation block) { }
		/// <summary>
		/// This method is called when redstone power is REMOVED from this block
		/// </summary>
		/// <param name="block">Location of this block</param>
		public virtual void OnPowerRemoved(BlockLocation block) { }

		/// <summary>
		/// This method is called when this block BEGINS to burn
		/// </summary>
		/// <param name="block">Location of this block</param>
		public virtual void OnStartBurn(BlockLocation block) { }
		/// <summary>
		/// This method is called when this block has been destroyed by fire (but BEFORE the OnBreak event is called)
		/// </summary>
		/// <param name="block">Location of this block</param>
		public virtual void OnBurnedUp(BlockLocation block) { }

		/// <summary>
		/// This is called when this block touches lava
		/// </summary>
		/// <param name="block">Location of this block</param>
		/// <param name="lavaLocation">location of the lava touching this block</param>
		public virtual void OnLavaContact(BlockLocation block, BlockLocation lavaLocation) { }
		/// <summary>
		/// This is called when the block comes into contact with water
		/// </summary>
		/// <param name="block">This blocks location</param>
		/// <param name="waterlocation">The location of the water touching this block</param>
		public virtual void OnWaterContact(BlockLocation block, BlockLocation waterlocation) { }
		#endregion
	}
}