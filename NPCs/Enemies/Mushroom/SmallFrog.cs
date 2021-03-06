using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace AAMod.NPCs.Enemies.Mushroom
{
    public class SmallFrog : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungus Frog");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 28;
            npc.aiStyle = -1;
            npc.damage = 8;
            npc.defense = 6;
            npc.lifeMax = 50;
            npc.knockBackResist = 0f;
            npc.npcSlots = 0f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.alpha = 255;
            banner = npc.type;
			bannerItem = mod.ItemType("FungusFrogBanner");
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            bool isDead = npc.life <= 0;
            if (isDead) 
            {

            }
            for (int m = 0; m < (isDead ? 35 : 6); m++)
            {
                int dustType = ModContent.DustType<Dusts.MushDust>();
                Dust.NewDust(npc.position, npc.width, npc.height, dustType, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default, isDead ? 2f : 1.5f);
            }
        }
        
        public override void AI()
        {
            npc.TargetClosest(true);
            if (npc.alpha > 0)
            {
                npc.alpha -= 4;
            }
            else
            {
                npc.alpha = 0;
            }
            Player player = Main.player[npc.target];
            if (npc.velocity.Y != 0)
            {
                if (npc.velocity.X < 0)
                {
                    npc.spriteDirection = -1;
                }
                else if (npc.velocity.X > 0)
                {
                    npc.spriteDirection = 1;
                }
            }
            else
            {
                if (player.position.X < npc.position.X)
                {
                    npc.spriteDirection = -1;
                }
                else if (player.position.X > npc.position.X)
                {
                    npc.spriteDirection = 1;
                }
            }
            if (npc.ai[0] < -10) npc.ai[0] = -10;
            BaseAI.AISlime(npc, ref npc.ai, false, 60, 3f, -2f, 6f, -4f);
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.velocity.Y < 0)
            {
                npc.frame.Y = frameHeight * 4;
            }
            else if (npc.velocity.Y > 0)
            {
                npc.frame.Y = frameHeight * 5;
            }
            else if (npc.ai[0] < -15f)
            {
                npc.frame.Y = 0;
            }
            else if (npc.ai[0] > -15f)
            {
                npc.frame.Y = frameHeight;
            }
            else if (npc.ai[0] > -10f)
            {
                npc.frame.Y = frameHeight * 2;
            }
            else if (npc.ai[0] > -5f)
            {
                npc.frame.Y = frameHeight * 3;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.GetModPlayer<AAPlayer>().ZoneMush && AAWorld.downedMonarch ? .3f : 0f;
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Items.Boss.MushroomMonarch.Mushium>(), Main.rand.Next(1, 5));
        }
    }
}