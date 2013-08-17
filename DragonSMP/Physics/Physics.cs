namespace DragonSpire
{
	class Physics
	{
		Entity Owner;
		PhysicsType PType;

		internal EntityLocation OldLocation; //The Entities last known position (Useful for sending the player back when they are inside of a block or something)
		internal EntityLocation Location; //Entities Current position
		internal EntityLocation Velocity; //Were just using this to hold the three dimensional velocity vector

		internal Physics(EntityLocation location, Entity owner, PhysicsType type)
		{
			Owner = owner;
			PType = type;

			Velocity = new EntityLocation(owner.world);
			Location = location;
			OldLocation = location;
		}
	}

	enum PhysicsType
	{
		//TODO Enumerate physics types
		None, //For things that don't move
		Gravity, //For things that have gravity *IE Mobs and Players
		Item, //For items, gravity AND pulled toward players
		Liquids, //I wonder?
	}
}
