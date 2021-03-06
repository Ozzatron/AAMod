using Terraria.ModLoader;
using Terraria.ID;

namespace AAMod.Items.Walls
{
    public class DaybreakWall : BaseAAItem
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createWall = mod.WallType("DaybreakBrickWall"); //put your CustomBlock Tile name
        }
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daybreak Brick Wall");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DaybreakBrick");
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(this, 4);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(null, "DaybreakBrick");
            recipe.AddRecipe();
        }
    }
}
