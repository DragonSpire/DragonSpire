using System;
using System.Collections.Generic;

namespace DragonSpire
{
	internal class Inventory //Represents a players invetory
	{
		Player Owner;
		PlayerWindowManager PWM;

		SLOT[] Crafting = new SLOT[5]; //0=output 1-4 Are crafting slots
		SLOT[] Armor = new SLOT[4]; //all 4 armor slots
		internal Container MainInventory = new Container(36);

		internal short CurrentSlot = 0;
		internal short CurrentSlotIndex
		{
			get
			{
				return (short)(CurrentSlot + 27);
			}
		}

		internal Inventory(Player player, PlayerWindowManager pwm) //Constructor
		{
			Owner = player;
			PWM = pwm;
		}

		public SLOT GetSlot(int slotNumber)
		{
			return MainInventory.GetSlot(slotNumber);
		}
		public void SetSlot(int slotNumber, SLOT slot)
		{
			Console.WriteLine("Setting Slot!");
			MainInventory.SetSlot(slotNumber, slot);
			Owner.client.SendSetSlot(0, (short)(slotNumber + 9), slot);
		}
	}
}