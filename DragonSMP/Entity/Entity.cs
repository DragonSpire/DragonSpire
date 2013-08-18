using System;

namespace DragonSpire
{
	/// <summary>
	/// Base entity class, this will extrapolate into all subsequent entity types
	/// </summary>
	public abstract class Entity
	{
		public static Random random = new Random();

		public int EId; //This is the Entity ID, unique among all entities
		internal Physics physics; //This is the physics attached to this entity

		public World world; //The world in which this entity resides

		public Chunk _currentChunk;

		public Chunk currentChunk
		{
			get
			{
				if (_currentChunk == null) return GetCurrentChunk;
				else return _currentChunk;
			}
		}//The chunk in which this entity currently resides

		public Chunk GetCurrentChunk
		{
			get
			{
				_currentChunk = world.GetChunkAt(new ChunkLocation((int)(physics.Location.X / 16), (int)(physics.Location.Z / 16), world));
				return _currentChunk;
			}
		}

		public Chunk oldChunk = null; //The last chunk this entity was in
		internal RegionLocation currentRegion
		{
			get
			{
				return currentChunk.CL.regionLocation;
			}
		}
		internal RegionLocation oldRegion;

		public Entity()
		{
			EId = GenerateID();
		}


		public abstract void PhysicsCall();
		public abstract byte[] GenerateMetaData();


		int GenerateID()
		{
			return random.Next();
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			Entity e = obj as Entity;
			if (e == null) return false;

			if (e.EId == EId) return true;

			return false;
		}
	}

	/// <summary>
	/// This holds all mobs and players
	/// </summary>
	public abstract class LivingEntity : Entity
	{
		public abstract string name { get; }

		public float Health = 20; //The health of this entity
		public short Hunger = 0; //How hungry this entity is D:
		public float FoodSaturation = 5; //The food saturation of this entity (staves off hunger)
		public bool IsDead { get { return (Health <= 0); } } //Whether or not this entity is dead

		public bool isOnFire = false;

		//internal Inventory inventory = new Container(9); //All Living Entities will have full inventories (This will allow us better handling later on)

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			Entity e = obj as Entity;
			if (e == null) return false;

			if (e.EId == EId) return true;

			return false;
		}
	}
	/// <summary>
	/// This holds all workable Entities (IE Workbench)
	/// </summary>
	public abstract class TileEntity : Entity
	{


		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			Entity e = obj as Entity;
			if (e == null) return false;

			if (e.EId == EId) return true;

			return false;
		}
	}
	/// <summary>
	/// This holds all non-workable non-living entities (IE sand)
	/// </summary>
	public abstract class ObjectEntity : Entity
	{
		public abstract byte[] GenerateObjectData();

		public abstract EntityObjectSpawnTypes ObjectType { get; }

		public abstract int Data { get; }

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			Entity e = obj as Entity;
			if (e == null) return false;

			if (e.EId == EId) return true;

			return false;
		}
	}
}