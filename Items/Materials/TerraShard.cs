﻿using Microsoft.Xna.Framework;
using Terraria;

namespace AAMod.Items.Materials
{
    public class TerraShard : BaseAAItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Shard");
            // ticksperframe, frameCount
        }

        // TODO -- Velocity Y smaller, post NewItem?
        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 18;
            item.maxStack = 999;
            item.value = 100;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, Color.LimeGreen.ToVector3() * 0.55f * Main.essScale);
        }
    }
}