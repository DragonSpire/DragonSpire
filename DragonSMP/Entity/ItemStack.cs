using System.Collections.Generic;
using System;

namespace DragonSpire
{
	public class ItemStack : ObjectEntity
	{
		internal static List<ItemStack> ItemStacks = new List<ItemStack>();

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

			Entities.Add(this);
		}

		public override void PhysicsCall()
		{
			Player[] pList = new Player[currentChunk.Players.Count];
			currentChunk.Players.Values.CopyTo(pList, 0);
			foreach (Player p in pList)
			{
				if (!p.client.loggedin || p.client.isDisconnected) continue;

				if (p.location.GetDistance(physics.Location) < p.PickupRange)
				{
					if (p.WindowManager.AddItemToInventory(ItemData))
					{
						Entity.Entities.Remove(this);
						currentChunk.Objects.Remove(EId);

						foreach (Player p2 in pList)
						{
							if (!p.client.loggedin || p.client.isDisconnected) continue;
							p2.client.SendCollectItem(p, this);
						}
						foreach (Player p3 in world.chunkManager.GetVisiblePlayers(currentRegion))
						{
							p3.client.SendDestroyEntity(this);
						}

						return;
					}
				}
			}
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