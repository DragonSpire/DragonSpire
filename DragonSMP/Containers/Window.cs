using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire
{
	/// <summary>
	/// This controls the windows and interactions with windows that a player has
	/// </summary>
	class PlayerWindowManager
	{
		bool isCurrentWindowInventory = true; //Whether or not the player is currently working out of their inventory
		SLOT CursorSlot; //This is the item that is held in the player hand

		WindowBase[] Windows = new WindowBase[16]; //Holds current windows (note that on accesing we need to verify the window is still legit!
		byte WindowIndex = 1; //Current Window Index

		Player p;

		//Get the current window that the player is working in
		WindowBase CurrentWindow
		{
			get
			{
				if (isCurrentWindowInventory) return Windows[0];
				else return Windows[WindowIndex];
			}
		}

		internal PlayerWindowManager(Player Owner) //Constructor for the PlayerWindowManager
		{
			p = Owner;
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
	/// This class controls the window types in the server
	/// </summary>
	internal class WindowManager
	{

	}
	/// <summary>
	/// This is the base window class, all window types will inherit from this class
	/// </summary>
	public abstract class WindowBase
	{

	}
}
