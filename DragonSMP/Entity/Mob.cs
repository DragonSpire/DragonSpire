using System;

namespace DragonSpire
{
	class Mob : LivingEntity
	{
		public override string name
		{
			get
			{
				return "Mob";
			}
		}

		//TODO remove static assignment
		public EntityMobSpawnTypes MobType = EntityMobSpawnTypes.Bat;

		public override void PhysicsCall()
		{

		}

		public override byte[] GenerateMetaData()
		{
			throw new NotImplementedException();
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
}
