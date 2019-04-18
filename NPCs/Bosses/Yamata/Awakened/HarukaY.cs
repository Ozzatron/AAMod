using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BaseMod;
using Terraria.Graphics.Shaders;
using AAMod.NPCs.Bosses.AH.Haruka;

namespace AAMod.NPCs.Bosses.Yamata.Awakened
{
    [AutoloadBossHead]
    public class HarukaY : Haruka
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ashe Akuma");
            Main.npcFrameCount[npc.type] = 27;
        }

        public override void SetDefaults()
        {
            npc.boss = false;
            npc.value = Item.buyPrice(0, 0, 0, 0);
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Akuma");
        }

        public override void PostAI()
        {
            if (!NPC.AnyNPCs(mod.NPCType<YamataA>()))
            {
                npc.life = 0;
            }
        }

        public override void NPCLoot()
        {
            npc.value = 0f;
            npc.boss = false;
            int DeathAnim = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType<HarukaVanish>(), 0);
            Main.npc[DeathAnim].velocity = npc.velocity;
            if (!NPC.AnyNPCs(mod.NPCType<YamataA>()))
            {
                Main.NewText("Dad, you idiot..! Whatever, Can't really say I didn't see it coming.", new Color(72, 78, 117));
                return;
            }
            npc.DropLoot(mod.ItemType<Items.Blocks.EventideAbyssiumOre>(), Main.rand.Next(10, 25));
            Main.NewText("That's it. I'm done, YOU deal with them, dad.", new Color(72, 78, 117));
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = 0;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.6f * bossLifeScale);  //boss life scale in expertmode
            npc.damage = (int)(npc.damage * 1.3f);  //boss damage increase in expermode
        }
    }
}


