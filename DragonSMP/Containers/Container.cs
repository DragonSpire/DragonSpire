using System;
using System.Collections.Generic;

namespace DragonSpire
{
	/// <summary>
	/// The container class will hold an array of items, it will be extrapolated into all inventory types
	/// </summary>
	public class Container
	{
		public static SLOT nullSlot = new SLOT(MaterialManager.GetMaterial(-1), 0, 0, new byte[0]);

		SLOT[] slots; //Do not allow the getting of slots directly
		public short slotCount
		{
			get
			{
				return (short)slots.Length;
			}
		}

		public Container(short count)
		{
			slots = new SLOT[count];
		}

		public byte[] GetAllSlotData()
		{
			var bytes = new List<byte>(); //The array we will fill with data

			for (int i = 0; i < slots.Length; i++) //Loop through all slots
			{
				bytes.AddRange(slots[i].GeneratePacketSlotData()); //Generate bytes for slot and add to array
			}

			return bytes.ToArray(); //Return data
		}

		public SLOT GetSlot(int slotNumber)
		{
			if (slotNumber >= slots.Length) throw new IndexOutOfRangeException("Slot index is larger than the number of slots in container!");
			if (slotNumber < 0) throw new IndexOutOfRangeException("Slot index is < 0!!");
			if (slots[slotNumber].material == null)
			{
				return nullSlot;
			}
			else
			{
				return slots[slotNumber];
			}
		}
		public void SetSlot(int slotNumber, SLOT slot)
		{
			if (slotNumber >= slots.Length) throw new IndexOutOfRangeException("Slot index is larger than the number of slots in container!");
			if (slotNumber < 0) throw new IndexOutOfRangeException("Slot index is < 0!!");
			slots[slotNumber] = slot;
		}
	}

	/// <summary>
	/// A slot represents one space for an item in any container
	/// </summary>
	public struct SLOT
	{
		/// <summary>
		/// The Material that is in this slot (note that id -1 is NOTHING)
		/// </summary>
		public Material material;
		/// <summary>
		/// How many of this material are in this slot
		/// </summary>
		public byte Count;
		/// <summary>
		/// The metadata for this material (default is 0)
		/// </summary>
		public short Meta;

		public byte[] ZippedNBT;
		//byte[] NBT;

		public int Data
		{
			get
			{
				if (material.isBlock)
				{
					return material.sendID | (Meta << 0xC); //Blocks have the ID and the MetaData
				}
				else
				{
					return 1; //Items only have a value of 1
				}
			}
		}


		/// <summary>
		/// Create a SLOT struct for holding inventory item data
		/// </summary>
		/// <param name="m">The material in this SLOT (to empty set to NOTHING as the material type</param>
		/// <param name="count">The number of this material that are in this slot</param>
		/// <param name="meta">The Damage or MetaData for this material</param>
		public SLOT(Material m, byte count, short meta, byte[] zippedNBT)
		{
			material = m;
			Count = count;
			Meta = meta;
			ZippedNBT = zippedNBT;
		}

		/// <summary>
		/// Generate the bytes to send over the network for this SLOT
		/// </summary>
		/// <returns>A byte array containing this slots information</returns>
		public byte[] GeneratePacketSlotData()
		{
			var bytes = new List<byte>();

			if (material == null || material.ID == -1)
			{
				bytes.AddRange(DBC.GetBytes((short)-1));
				return bytes.ToArray();
			}
			else
			{
				bytes.AddRange(DBC.GetBytes(material.sendID)); //Were sending this, so lets use the SendID
				bytes.Add(Count);
				bytes.AddRange(DBC.GetBytes(Meta)); //Add Meta

				//Get NBT Data
				byte[] NBTData = GenerateNBT();
				
				if (NBTData.Length == 0)
				{
					//We have no NBT Data
					bytes.AddRange(DBC.GetBytes((short)-1));
				}
				else
				{
					//Add NBT Data
					bytes.AddRange(DBC.GetBytes(NBTData.Length));
					bytes.AddRange(NBTData);
				}

				return bytes.ToArray();
			}
		}
		byte[] GenerateNBT()
		{
			return new byte[0];
		}
	}
}
