using Terraria.ModLoader;

namespace AAMod.Items.Blocks.BogwoodF
{
    public class BogwoodPlatform : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Bogwood Platform");
		}

		public override void SetDefaults()
		{
			item.width = 8;
			item.height = 10;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.createTile = mod.TileType("BogwoodPlatform");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe;
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Bogwood");
            recipe.SetResult(this, 2);
            recipe.AddRecipe(); 
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(this, 2);
            recipe.SetResult(null, "Bogwood");
            recipe.AddRecipe();
        }
	}
}
