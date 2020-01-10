﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BaseMod;
using AAMod.Dusts;
using System.IO;

namespace AAMod.NPCs.Bosses.Equinox
{
    [AutoloadBossHead]	
	public class DaybringerHead : ModNPC
	{
        public float[] customAI = new float[2];		
		public bool nightcrawler = false;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Daybringer");
            Main.npcFrameCount[npc.type] = 1;
		}

		public override void SetDefaults()
		{
            npc.lifeMax = 125000;
            npc.damage = 125;
            npc.defense = 100;
            npc.value = Item.sellPrice(0, 10, 0, 0);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.knockBackResist = 0f;
            npc.width = 68;
            npc.height = 68;
            npc.boss = true;
            npc.aiStyle = -1;
			npc.timeLeft = 500;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.DeathSound = null;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Equinox");
            musicPriority = MusicPriority.BossHigh;
            bossBag = mod.ItemType("DBBag");
		}

        public float[] internalAI = new float[2];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(internalAI[0]);
                writer.Write(internalAI[1]);
                writer.Write(isDeathRay);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                internalAI[0] = reader.ReadFloat(); //DaybringerCounter
                internalAI[1] = reader.ReadFloat(); //NightclawerCounter
                isDeathRay = reader.ReadBoolean();
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        public override void BossHeadRotation(ref float rotation)
		{
			rotation = npc.rotation;
		}

		public override bool CheckActive()
		{
			npc.timeLeft--;
			return npc.timeLeft < 50;
		}

		public void HandleDayNightCycle()
		{
			bool daybringerExists = NPC.AnyNPCs(ModContent.NPCType<DaybringerHead>());
			bool nightcrawlerExists = NPC.AnyNPCs(ModContent.NPCType<NightcrawlerHead>());
			if (daybringerExists && nightcrawlerExists)
            {
                if (Main.expertMode)
                {
                    Main.fastForwardTime = true;
                    Main.dayRate = 20;
                }else
                {
                    Main.fastForwardTime = true;
                    Main.dayRate = 15;
                }
            }else
            if (daybringerExists && !nightcrawlerExists)
            {
                Main.fastForwardTime = true;
                Main.dayTime = true;
                Main.dayRate = 0;
            }else
            if (!daybringerExists && nightcrawlerExists)
            {
                Main.fastForwardTime = true;
                Main.dayTime = false;
                Main.dayRate = 0;
            }else
            {
                Main.dayRate = 1;
                Main.fastForwardTime = false;
            }
		}
        bool isDeathRay = false;
		bool prevWormStronger = false;
		bool initCustom = false;
        public override bool PreAI()
        {
            if(nightcrawler)
            {
                Lighting.AddLight((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), .37f, .8f, .89f);
            }
            if (Main.netMode != 1 && !initCustom)
            {
                initCustom = true;
                customAI[0] += npc.whoAmI % 7 * 12; //so it doesn't pew all at once
                npc.velocity.X += 0.1f;
                npc.velocity.Y -= 4f;
            }
            bool isHead = npc.type == mod.NPCType("DaybringerHead") || npc.type == mod.NPCType("NightcrawlerHead");
            if (isHead)
            {
                HandleDayNightCycle();
            }
            bool isDay = Main.dayTime;
            bool wormStronger = (nightcrawler && !isDay) || (!nightcrawler && isDay);
            if (wormStronger != prevWormStronger)
            {
                int dustType = nightcrawler ? ModContent.DustType<NightcrawlerDust>() : ModContent.DustType<DaybringerDust>();
                for (int k = 0; k < 10; k++)
                {
                    int dustID = Dust.NewDust(npc.position, npc.width, npc.height, dustType, (int)(npc.velocity.X * 0.2f), (int)(npc.velocity.Y * 0.2f), 0, default, 1.5f);
                    Main.dust[dustID].noGravity = true;
                }
            }

            if (isHead) //prevents despawn and allows them to run away
            {
                bool foundTarget = TargetClosest();
                if (foundTarget)
                {
                    npc.timeLeft = 300;
                }
                else
                {
                    if (npc.timeLeft > 50) npc.timeLeft = 50;
                    npc.velocity.Y -= 0.2f;
                    if (npc.velocity.Y < -20f) npc.velocity.Y = -20f;
                    return false;
                }
            }
            else
            {
                npc.timeLeft = 300; //pieces should not despawn naturally, only despawn when the head does
            }

            float wormDistance = -26f;
            int aiCount = 2;
            float moveSpeedMax = 16f;
            npc.damage = 100;
            npc.defense = 100;

            if (wormStronger)
            {
                wormDistance = -52f;
                aiCount = !nightcrawler ? 6 : 4;
                moveSpeedMax = !nightcrawler ? 20f : 16f;
                npc.damage = 140;
                npc.defense = !nightcrawler ? 120 : 150;
            }
            
            Player target = Main.player[npc.target];

            if (!isHead && NPC.CountNPCS(ModContent.NPCType<Equiprobe>()) < 15)
            {
                SpawnProbe();
            }
            if (npc.type == mod.NPCType("NightcrawlerHead") && isDeathRay && Math.Abs(npc.DirectionTo(target.Center).ToRotation() - npc.velocity.ToRotation()) < 0.1f)
            {
                goto DeathRayCheck;
            }
            for (int m = 0; m < aiCount; m++)
            {
                int Length = nightcrawler ? 24 : 30;
                int[] wormTypes = nightcrawler ? new int[] { mod.NPCType("NightcrawlerHead"), mod.NPCType("NightcrawlerBody"), mod.NPCType("NightcrawlerTail") } : new int[] { mod.NPCType("DaybringerHead"), mod.NPCType("DaybringerBody"), mod.NPCType("DaybringerTail") };
                BaseAI.AIWorm(npc, wormTypes, Length, wormDistance, moveSpeedMax, 0.07f, true, false, false, false, false, false);
            }
            goto Normal;
            DeathRayCheck:
            for(int deathRay = 0; deathRay < Main.maxProjectiles; deathRay++)
            {
                if(Main.projectile[deathRay].active && Main.projectile[deathRay].type == mod.ProjectileType("NightclawerDeathraySmall") || Main.projectile[deathRay].type == mod.ProjectileType("NightclawerDeathray") && Main.projectile[deathRay].ai[1] == npc.whoAmI)
                {
                    return false;
                }
            }
            isDeathRay = false;
            Normal:
            npc.spriteDirection = 1;
            prevWormStronger = wormStronger;

            if(isDay)
            {
                internalAI[0] += 1f;
                if(isHead && npc.type == mod.NPCType("DaybringerHead"))
                {
                    if(internalAI[0] % 360 == 0)
                    {
                        for(int playerid = 0; playerid < 255; playerid++)
                        {
                            if(Main.player[playerid].active && !Main.player[playerid].dead && Main.player[playerid] != null && Main.player[playerid].ownedProjectileCounts[mod.ProjectileType("DaybringerStars")] <= 0)
                            {
                                Projectile.NewProjectile(Main.player[playerid].Center.X - 200f, Main.player[playerid].Center.Y + 300f, 0, 0, mod.ProjectileType("DaybringerStars"), npc.damage / 6, 5, playerid, -200f, playerid);
                                Projectile.NewProjectile(Main.player[playerid].Center.X, Main.player[playerid].Center.Y - 300f, 0, 0, mod.ProjectileType("DaybringerStars"), npc.damage / 6, 5, playerid, 0, playerid);
                                Projectile.NewProjectile(Main.player[playerid].Center.X + 200f, Main.player[playerid].Center.Y + 300f, 0, 0, mod.ProjectileType("DaybringerStars"), npc.damage / 6, 5, playerid, 200f, playerid);
                            }
                        }
                    }
                    if(Math.Abs(npc.DirectionTo(target.Center).ToRotation() - npc.velocity.ToRotation()) < 0.7f && internalAI[0] % 360 == 0)
                    {
                        Vector2 speed = Vector2.Normalize(npc.velocity) * 5f;
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, speed.X, speed.Y, mod.ProjectileType("DaybringerSun"), npc.damage / 6, 1, 255);
                    }
                    if(internalAI[0] % 120 == 30)
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == mod.NPCType("DaybringerBody") && Main.npc[i].realLife == npc.whoAmI)
                            {
                                Vector2 speed = Vector2.Normalize(new Vector2(1f, 0f).RotatedBy(Main.npc[i].rotation + 3.1415f)) * 12f;
                                Projectile.NewProjectile(Main.npc[i].Center.X, Main.npc[i].Center.Y, speed.X, speed.Y, mod.ProjectileType("DayBringerDarts"), npc.damage / 6, 0, Main.myPlayer);
                                speed = -speed;
                                Projectile.NewProjectile(Main.npc[i].Center.X, Main.npc[i].Center.Y, speed.X, speed.Y, mod.ProjectileType("DayBringerDarts"), npc.damage / 6, 0, Main.myPlayer);
                            }
                        }
                    }
                    if(internalAI[0] % 450 == 200)
                    {
                        for (int i = 0; i < Main.maxNPCs; i+=4)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == mod.NPCType("DaybringerBody") && Main.npc[i].realLife == npc.whoAmI)
                            {
                                Vector2 speed = Vector2.Normalize(new Vector2(1f, 0f).RotatedBy(Main.npc[i].rotation + 3.1415f)) * 8f;
                                Projectile.NewProjectile(Main.npc[i].Center.X, Main.npc[i].Center.Y, speed.X, speed.Y, mod.ProjectileType("DaybringerOrb"), npc.damage / 6, 0, Main.myPlayer, 0, npc.whoAmI);
                                speed = -speed;
                                Projectile.NewProjectile(Main.npc[i].Center.X, Main.npc[i].Center.Y, speed.X, speed.Y, mod.ProjectileType("DaybringerOrb"), npc.damage / 6, 0, Main.myPlayer, 0, npc.whoAmI);
                            }
                        }
                    }
                }

                if(internalAI[0] > 1000)
                {
                    internalAI[0] = 0f;
                }
            }
            else
            {
                internalAI[1] += 1f;
                if(isHead && npc.type == mod.NPCType("NightcrawlerHead"))
                {
                    if(internalAI[1] % 300 == 0)
                    {
                        for(int playerid = 0; playerid < 255; playerid++)
                        {
                            if(Main.player[playerid].active && !Main.player[playerid].dead && Main.player[playerid] != null && Main.player[playerid].ownedProjectileCounts[mod.ProjectileType("DaybringerStars")] <= 0)
                            {
                                Projectile.NewProjectile(Main.player[playerid].Center.X + Main.rand.Next(-5, 5) * 40f, Main.player[playerid].Center.Y + Main.rand.Next(-5, 5) * 40f, 0, 0, mod.ProjectileType("NightclawerCloud"), npc.damage / 6, 0, 255);
                            }
                        }
                    }
                    if(Math.Abs(npc.DirectionTo(target.Center).ToRotation() - npc.velocity.ToRotation()) < 0.5f && (npc.Center - target.Center).Length() < 900f && !isDeathRay)
                    {
                        isDeathRay = true;
                        npc.netUpdate = true;
                        if (Main.netMode != 1) Projectile.NewProjectile(npc.position, Vector2.Normalize(npc.velocity), mod.ProjectileType("NightclawerDeathraySmall"), npc.damage / 4, 0f, Main.myPlayer, 0, npc.whoAmI);
                    }
                }
                if(internalAI[1] % 120 == 90 && npc.type == mod.NPCType("NightcrawlerBody") && Main.rand.Next(15) == 0)
                {
                    Vector2 speed = Vector2.Normalize(new Vector2(1f, 0f).RotatedBy(npc.rotation + 3.1415f));
                    speed = (Main.rand.Next(2) == 0 ? 1: -1) * speed;
                    float ai = Main.rand.Next(120);
                    Vector2 speedR = Vector2.Normalize(speed.RotatedByRandom(0.6)) * 20f;
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, speedR.X, speedR.Y, mod.ProjectileType("NightclawerLaser"), npc.damage / 6, 0, Main.myPlayer, speed.ToRotation() + 1000f, ai);
                }
                if(internalAI[1] % 380 == 90)
                {
                    for (int i = 0; i < Main.maxNPCs; i+= 4)
                    {
                        if (Main.npc[i].active && Main.npc[i].type == mod.NPCType("NightcrawlerBody") && Main.npc[i].realLife == npc.whoAmI)
                        {
                            Vector2 speed = Vector2.Normalize(new Vector2(1f, 0f).RotatedBy(Main.npc[i].rotation + 3.1415f)) * .5f;
                            Projectile.NewProjectile(Main.npc[i].Center.X, Main.npc[i].Center.Y, speed.X, speed.Y, mod.ProjectileType("NightclawerScythe"), npc.damage / 6, 0, Main.myPlayer, npc.rotation, npc.spriteDirection);
                            speed = -speed;
                            Projectile.NewProjectile(Main.npc[i].Center.X, Main.npc[i].Center.Y, speed.X, speed.Y, mod.ProjectileType("NightclawerScythe"), npc.damage / 6, 0, Main.myPlayer, npc.rotation, npc.spriteDirection);
                        }
                    }
                }

                if(internalAI[1] > 1000)
                {
                    internalAI[1] = 0f;
                }
            }
            return false;
        }

        public int probeCounter = -1;
        public void SpawnProbe()
        {
			if(probeCounter == -1)
				probeCounter = 500 + Main.rand.Next(750);
			if(Main.netMode == NetmodeID.MultiplayerClient || npc.whoAmI % 3 != 0) return;
			probeCounter = Math.Max(0, probeCounter - 1);
            if (probeCounter <= 0)
            {
				probeCounter = 500 + Main.rand.Next(750);
				if(BaseAI.GetNPCs(npc.Center, mod.NPCType("Equiprobe"), 8000f).Length < 6)
				{
					int npcID = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("Equiprobe"), 0);
					Main.npc[npcID].netUpdate = true;
				}
            }
        }

		public int playerTooFarDist = 16000; //1000 tile radius, these worms move fast!		
		public bool TargetClosest()
		{
			int[] players = BaseAI.GetPlayers(npc.Center, Math.Min(20000f, playerTooFarDist * 3));
			float dist = 999999999f;
			int foundPlayer = -1;
			for (int m = 0; m < players.Length; m++)
			{
				Player p = Main.player[players[m]];
				if (Vector2.Distance(p.Center, npc.Center) < dist)
				{
					dist = Vector2.Distance(p.Center, npc.Center);
					foundPlayer = p.whoAmI;
				}
			}
			if (foundPlayer != -1)
			{
				BaseAI.SetTarget(npc, foundPlayer);
				return true;
			}
			return false;
		}

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.75f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.85f);
		}

		bool spawnedGore = false;
        public override void HitEffect(int hitDirection, double damage)
        {
			int dustType = nightcrawler ? ModContent.DustType<NightcrawlerDust>() : ModContent.DustType<DaybringerDust>();
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection, -1f, 0, default, 1.2f);
            }
            if (npc.life <= 0 || (npc.life - damage <= 0))
            {			
				Main.dayRate = 1;
                Main.fastForwardTime = false;	
				if(!spawnedGore)
				{
					spawnedGore = true;
					bool isHead = npc.type == mod.NPCType("DaybringerHead") || npc.type == mod.NPCType("NightcrawlerHead");
					bool isBody = npc.type == mod.NPCType("DaybringerBody") || npc.type == mod.NPCType("NightcrawlerBody");						
					if(nightcrawler)
					{
						if(isHead)
						{
							Gore.NewGore(npc.position, npc.velocity * 0.2f, mod.GetGoreSlot("Gores/NCGore1"), 1f);	
							Gore.NewGore(npc.position, npc.velocity * 0.2f, mod.GetGoreSlot("Gores/NCGore2"), 1f);						
						}else
						if(isBody)
						{
							Gore.NewGore(npc.position, npc.velocity * 0.2f, mod.GetGoreSlot("Gores/NCGore3"), 1f);							
						}else
						{
							Gore.NewGore(npc.position, npc.velocity * 0.2f, mod.GetGoreSlot("Gores/NCGore4"), 1f);						
						}
					}else
					{
						if(isHead)
						{
							Gore.NewGore(npc.position, npc.velocity * 0.2f, mod.GetGoreSlot("Gores/DBGore1"), 1f);	
							Gore.NewGore(npc.position, npc.velocity * 0.2f, mod.GetGoreSlot("Gores/DBGore2"), 1f);						
						}else
						if(isBody)
						{
							Gore.NewGore(npc.position, npc.velocity * 0.2f, mod.GetGoreSlot("Gores/DBGore3"), 1f);							
						}else
						{
							Gore.NewGore(npc.position, npc.velocity * 0.2f, mod.GetGoreSlot("Gores/DBGore4"), 1f);						
						}					
					}
					for (int k = 0; k < 15; k++)
					{
						Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection, -1f, 0, default, 1.5f);
					}
				}
            }
        }

        public override void NPCLoot()
        {
            int otherWormAlive = nightcrawler ? mod.NPCType("DaybringerHead") : mod.NPCType("NightcrawlerHead");
            if (!nightcrawler)
            {
                AAWorld.downedDB = true;
                BaseAI.DropItem(npc, mod.ItemType("DBTrophy"), 1, 1, 15, true);
            }
            else
            {
                AAWorld.downedNC = true;
                BaseAI.DropItem(npc, mod.ItemType("NCTrophy"), 1, 1, 15, true);
            }
            if (NPC.CountNPCS(otherWormAlive) == 0)
            {
                AAWorld.downedEquinox = true;
            }
			string wormType = nightcrawler ? "Nightcrawler" : "Daybringer";
			if (Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType(wormType + "Trophy"));
			}
			if (Main.expertMode)
			{
                npc.DropBossBags();
			}
			else
			{
				if (Main.rand.Next(7) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType(wormType + "Mask"));
				}
                if (!nightcrawler)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Stardust"), Main.rand.Next(30, 75));
                }
                else
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DarkEnergy"), Main.rand.Next(30, 75));
                }
			}
        }

		public Color GetAuraAlpha()
		{
			Color c = Color.White * (Main.mouseTextColor / 255f);
			//c.A = 255;
			return c;
		}

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            ModifyCritArea(npc, ref crit);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            ModifyCritArea(npc, ref crit);
            if (projectile.penetrate != 1)
            {
                for(int i = 0; i < Main.maxNPCs; i ++)
                {
                    if(Main.npc[i].active && (Main.npc[i].whoAmI == npc.realLife || (Main.npc[i].realLife >= 0 && Main.npc[i].realLife == npc.realLife)))
                    {
                        Main.npc[i].immune[projectile.owner] = 10;
                    }
                }
            }
        }

        private void ModifyCritArea(NPC npc, ref bool crit)
        {
            if (npc.realLife >= 0)
            {
                if (npc.whoAmI == npc.realLife)
                {
                    crit = true;
                }
                if (npc.ai[0] == 0)
                {
                    crit = false;
                }
            }
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            if (npc.realLife >= 0 && npc.whoAmI != npc.realLife)
            {
                damage = 0;
                npc.lifeRegen = 0;
            }
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            MakeSegmentsImmune(npc, projectile.owner);
        }

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            MakeSegmentsImmune(npc, player.whoAmI);
        }

        public void MakeSegmentsImmune(NPC npc, int id)
        {
            if (npc.realLife >= 0)
            {
                bool last = false;
                NPC parent = Main.npc[npc.realLife];
                parent.lifeRegen = npc.lifeRegen;
                int i = 0;
                while (parent.ai[0] > 0 || last)
                {
                    parent.immune[id] = npc.immune[id];
                    for (int j = 0; j < npc.buffType.Length; j++)
                    {
                        if (npc.buffType[j] > 0 && npc.buffTime[j] > 0)
                        {
                            parent.buffType[j] = npc.buffType[j];
                            parent.buffTime[j] = npc.buffTime[j];
                        }
                    }
                    if (last) { break; }
                    parent = Main.npc[(int)parent.ai[0]];
                    if (parent.ai[0] == 0) { last = true; }
                    if (i++ > 200) { throw new InvalidOperationException("Recursion detected"); } // Just in case
                }
            }
        }

        public override bool PreDraw(SpriteBatch spritebatch, Color dColor)
        {
            bool wormStronger = (nightcrawler && !Main.dayTime) || (!nightcrawler && Main.dayTime);
            Texture2D tex = Main.npcTexture[npc.type];
            npc.width = 68;
            npc.height = 68;
            if (wormStronger)
            {
                npc.width = 136;
                npc.height = 136;
                string texName = "NPCs/Bosses/Equinox/";
                if (npc.type == mod.NPCType("DaybringerHead")) { texName += "DaybringerHeadBig"; }
                else
                if (npc.type == mod.NPCType("DaybringerBody")) { texName += "DaybringerBodyBig"; }
                else
                if (npc.type == mod.NPCType("DaybringerTail")) { texName += "DaybringerTailBig"; }
                else
                if (npc.type == mod.NPCType("NightcrawlerHead")) { texName += "NightcrawlerHeadBig"; }
                else
                if (npc.type == mod.NPCType("NightcrawlerBody")) { texName += "NightcrawlerBodyBig"; }
                else
                if (npc.type == mod.NPCType("NightcrawlerTail")) { texName += "NightcrawlerTailBig"; }
                tex = mod.GetTexture(texName);

                int diff = Main.LocalPlayer.miscCounter % 50;
                float diffFloat = diff / 50f;
                float auraPercent = BaseUtility.MultiLerp(diffFloat, 0f, 1f, 0f); //did it this way so it's syncronized between all the segments
                BaseDrawing.DrawAura(spritebatch, tex, 0, npc, auraPercent, 2f, 0f, 0f, GetAuraAlpha());
            }
            BaseDrawing.DrawTexture(spritebatch, tex, 0, npc, Color.White); //GetAuraAlpha());				
            return false;
        }
    }
}