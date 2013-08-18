using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire
{
	/// <summary>
	/// This controls the windows and interactions with windows that a player has
	/// </summary>
	public class PlayerWindowManager
	{
		internal bool isCurrentWindowInventory = true; //Whether or not the player is currently working out of their inventory
		internal SLOT CursorSlot; //This is the item that is held in the player hand

		
		internal Window[] Windows = new Window[16]; //Holds current windows (note that on accesing we need to verify the window is still legit!
		internal byte WindowIndex = 1; //Current Window Index

		internal Player p; //The player that this manager belongs to

		//Get the current window that the player is working in
		internal Window CurrentWindow
		{
			get
			{
				if (isCurrentWindowInventory) return Windows[0];
				else return Windows[WindowIndex];
			}
		}

		internal PlayerWindowManager(Player Owner) //Constructor for the PlayerWindowManager
		{
			//TODO Create user inventory
			p = Owner;
		}

		internal void OpenWindow(Window w)
		{
			var i = GetWindowIndex();
			Windows[i] = w;
			isCurrentWindowInventory = false;

			//p.client.SendOpenWindow(w, i); //Send Window and ID (index)
			//Should also send window items, can we do that before we open it?
		}
		internal void ForceCloseWindow(byte index)
		{
			//p.client.SendCloseWindow(index);
			CloseWindow(index);
		}
		internal void CloseWindow(byte index)
		{
			isCurrentWindowInventory = true;
		}

		internal void ClickWindow(byte ID, short SlotNumber, byte button, short ActionNumber, byte Mode, SLOT ClickedItem)
		{
			bool completed = Windows[ID].Click(SlotNumber, button, Mode, ClickedItem, this);

			//if completed = false then we need to reject this action!
			//if completed = true then this action is good!
		}

		//Get and Increment the WindowIndex for history purposes
		internal byte GetWindowIndex()
		{
			WindowIndex++;
			if (WindowIndex >= 17) WindowIndex = 1; //we are skipping 0 on purpose, 0 is the players inventory
			return WindowIndex;
		}
	}

	/// <summary>
	/// This class holds active window sessions and all data related to those windows
	/// </summary>
	public class Window
	{
		List<Player> players = new List<Player>(); //This holds a list of the players that currently have this window open, this is checked each time we loop through it to be sure this player is still online

		internal WindowTypeBase WindowType; //Window types are not linked to specific types of windows (IE a furnace does not HAVE to be in a furnace, but all furnaces will have furnace windows)
		internal bool isEntityWindow = false; //True if were working with a window thats from a living creature or other player

		internal Entity e; //Null if this is not an EntityWindow
		internal BlockLocation BL; //This is only set if we have a BLOCK that has a window

		internal bool Click(short SN, byte button, byte Mode, SLOT CI, PlayerWindowManager PWM)
		{
			return WindowType.Click(SN, button, Mode, CI, this, PWM);
		}

		internal string Title;

		internal Container container;

		internal Window(bool isEntity, dynamic EidOrLocation, WindowTypeBase windowType, string name = "")
		{
			isEntityWindow = isEntity;
			if (isEntity) e = EidOrLocation;
			else BL = EidOrLocation;
			WindowType = windowType;
			if (name == "") Title = windowType.BaseName;
			else Title = name;

			container = new Container(windowType.SlotCount);
		}

		public void SetItem(short slot, SLOT data)
		{
			container.SetSlot(slot, data);

			foreach(Player p in players.ToArray())
			{
				if (!p.client.loggedin || p.client.isDisconnected || p.WindowManager.CurrentWindow != this) //Player cannot possibly have this window open
					{ players.Remove(p); } //Remove this player from the player list

				//This player has this window open, so we get to update the slot we have updated!
				//p.client.SetSlot

			}
		}
	}

	/// <summary>
	/// This class controls the window types in the server
	/// </summary>
	internal class WindowTypeManager
	{
		//Load winderz and manage them laterz
	}

	/// <summary>
	/// This is the base window TYPE class, all window types will inherit from this class
	/// </summary>
	public abstract class WindowTypeBase
	{
		/// <summary>
		/// This is the base name of your window type, this can be overriden in code elsewhere
		/// </summary>
		public abstract string BaseName { get; }

		/// <summary>
		/// This is the TOTAL number of slots your window has (note that if you use a window type you HAVE to follow the baseline for that window type, period
		/// </summary>
		public abstract short SlotCount { get; }

		/// <summary>
		/// This byte represents the TYPE of this window, be it a chest furnace or crafting block
		/// </summary>
		public abstract byte InventoryType { get; }
		/// <summary>
		/// This bool represents whether all players see the same instance of this window or not
		/// </summary>
		public abstract bool isCrossPlayer { get; }
		
		/// <summary>
		/// This method is called when a player clicks on the window
		/// </summary>
		/// <param name="SlotNumber">The index of the slot that was clicked by the player</param>
		/// <param name="bytton">The button parameter that the player clicked with</param>
		/// <param name="mode">The mode this players click is in</param>
		/// <param name="CI">The CurrentItem that the player has clicked (what is currently in that slot in the players window, if it does not match what SHOULD be in it, we should have already returned false</param>
		/// <param name="w">The WINDOW class that this click is referring to, you can get most of the information you need about the window here, any changes here will be pushed to all players that have this same window open, assuming its a crossplayer window</param>
		/// <param name="PWM">The PlayerWindowManager class that this click was called from, in this class you can get the information for the SLOT attached to the cursor and information on the player</param>
		/// <returns>This should return a bool as to whether or not this transaction has completed succesfully, return false to reject this transaction (in which case no changes should be made to the inventory.</returns>
		public abstract bool Click(short SlotNumber, byte button, byte mode, SLOT CI, Window w, PlayerWindowManager PWM);
	}

	public class WindowTypes
	{
		byte Chest = 0;
		byte LargeChest = 0;

		byte WorkBench = 1;
		byte CraftingBench = 1;

		byte Furnace = 2;
		byte Dispenser = 3;
		byte EnchantmentTable = 4;
		byte BrewingStand = 5;
		byte NPCTrade = 6;
		byte Beacon = 7;
		byte Anvil = 8;
		byte Hopper = 9;

		public short DefaultSlotCount(byte WindowType)
		{
			switch (WindowType)
			{
				case 0: //Chest (note that im only returning the size of a SMALL chest
					return 27;
				case 1: //Workbench
					return 10;
				case 2: //Furnace
					return 3;
				case 3: //Dispenser
					return 9;
				case 4: //EnchantmentTable
					return 1;
				case 5: //BrewingStand
					return 4;
				case 6: //NPC Trade
					return 3;
				case 7: //Beacon
					return 1;
				case 8: //Anvil
					return 3;
				case 9: //Hopper
					return 5;
				default:
					return 0;
			}
		}
	}
}
