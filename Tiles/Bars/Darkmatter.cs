using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AAMod.Tiles.Bars
{
    public class Darkmatter : ModTile
    {
        public override void SetDefaults()
        {
            soundType = 21;

            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            dustType = mod.DustType("DarkmatterDust");
            drop = mod.ItemType("DarkMatter");   
            AddMapEntry(new Color(0, 0, 255));
			minPick = 0;
        }
    }
}