using System.Collections.Generic;

namespace DragonSpire
{
	public class RecipeManager
	{
		internal static List<CraftingRecipe> CraftingRecipes = new List<CraftingRecipe>();
		internal static List<FurnaceRecipe> Furnace = new List<FurnaceRecipe>();

		public RecipeManager()
		{
		}
	}

	internal abstract class Recipe
	{
		internal abstract RecipeType Type { get; }
		//internal abstract ItemStack Result { get; }
	}

	internal abstract class CraftingRecipe : Recipe
	{
		internal override RecipeType Type
		{
			get { return RecipeType.Crafting; }
		}
		internal abstract bool IsShapeless { get; }
		//internal abstract ItemStack[] Ingredients { get; }
	}

	internal abstract class FurnaceRecipe : Recipe
	{
		internal override RecipeType Type
		{
			get { return RecipeType.Furnace; }
		}
		//internal abstract ItemStack Ingredients { get; }
	}

	internal enum RecipeType
	{
		Crafting,
		Furnace
	}
}