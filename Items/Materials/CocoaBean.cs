using Terraria.ModLoader;

namespace AAMod.Items.Materials
{
    public class CocoaBean : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cocoa Beans");
        }
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 99;
            item.rare = 2;
        }
    }
}