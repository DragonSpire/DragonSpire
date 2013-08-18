namespace DragonSpire
{
	internal class Inventory //Represents a players invetory
	{
		Player p;

		SLOT[] Crafting = new SLOT[5]; //0=output 1-4 Are crafting slots
		SLOT[] Armor = new SLOT[4]; //all 4 armor slots
		Container MainInventory = new Container(36);

		internal Inventory(Player player) //Constructor
		{
			p = player;
		}
	}
}