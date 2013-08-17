namespace DragonSpire
{
	internal class Inventory
	{
		internal string Title;
		internal int Size; //Multiples of 9. 
		internal ItemStack[] Contents; //The contents of this inventory
		internal short SelectedSlot = 0; //The Slot the player currently has selected on the hotbar
		internal short CurrentItem = 0; //Current Item the entity is holding (0 for none)

		//Constructors
		internal Inventory()
		{
			Title = "Inventory";
			Size = 27;
			Contents = null;
		}
		internal Inventory(string title)
		{
			Title = title;
			Size = 27;
			Contents = null;
		}
		internal Inventory(string title, int size)
		{
			Title = title;
			Size = size;
			Contents = null;
		}
		internal Inventory(string title, int size, ItemStack[] contents)
		{
			Title = title;
			Size = size;
			if (contents.Equals(null))
			{
				Contents = new ItemStack[Size];
			}
			else
			{
				//TODO: Check if Contents Length matches size.
				this.Contents = contents;
			}

		}
	}
}