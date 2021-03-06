﻿using Microsoft.Xna.Framework;

namespace AAMod.NPCs.Enemies.Snow
{
    public class SnakeTail : SnakeHead
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Snow Serpent");
        }
		
		public override void SetDefaults()
		{
            base.SetDefaults();
            npc.dontCountMe = true;
		}

		public override bool PreNPCLoot()
		{
			return false;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
	}
}