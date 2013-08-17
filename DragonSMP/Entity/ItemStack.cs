using System.Collections.Generic;
using System;

namespace DragonSpire
{
	public class ItemStack : ObjectEntity
	{
		/// <summary>
		/// This is EntityObjectSpawnTypes.ItemStack
		/// </summary>
		public override EntityObjectSpawnTypes ObjectType { get { return EntityObjectSpawnTypes.ItemStack; } }

		/// <summary>
		/// This is the SLOT data that is contained in this ItemStack Entity
		/// </summary>
		public SLOT ItemData;

		public ItemStack(SLOT data, EntityLocation pl)
		{
			world = pl.world;
			physics = new Physics(pl, this, PhysicsType.Item);
			ItemData = data;
		}

		public override void PhysicsCall()
		{
			//TODO have this move closer to the player as they get near?
		}
		public override byte[] GenerateMetaData()
		{
			List<byte> bytes = new List<byte>();

			bytes.Add((byte)EntityMetaDataTypes.Slot | (byte)10); //Set the flag for SLOT | SLOT_DATA
			bytes.AddRange(ItemData.GeneratePacketSlotData());
			bytes.Add(127); //End of METADATA

			return bytes.ToArray();
		}

		public override int Data
		{
			get { return ItemData.Data; }
		}

		public override byte[] GenerateObjectData()
		{
			if (ItemData.material.sendID == 0 && ItemData.Meta == 0) return new byte[0];

			List<byte> bytes = new List<byte>();

			bytes.AddRange(DBC.GetBytes((short)0)); //x velocity
			bytes.AddRange(DBC.GetBytes((short)0)); //y velocity
			bytes.AddRange(DBC.GetBytes((short)0)); //z velocity

			return bytes.ToArray();
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