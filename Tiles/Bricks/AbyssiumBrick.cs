﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AAMod.Tiles.Bricks
{
    class AbyssiumBrick : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileLighted[Type] = false;
            Main.tileBlockLight[Type] = true;
            drop = mod.ItemType("AbyssiumBrick");   
            AddMapEntry(new Color(0, 0, 51));
            dustType = ModContent.DustType<Dusts.AbyssiumDust>();
        }
    }
}
