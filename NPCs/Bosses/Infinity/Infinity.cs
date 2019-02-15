﻿using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BaseMod;
using AAMod.Buffs;

namespace AAMod.NPCs.Bosses.Infinity
{
    public class Infinity : ModNPC
    {
        public NPC Zero1;
        public NPC Zero2;
        public NPC Zero3;
        public NPC Zero4;
        public NPC Zero5;
        public NPC Zero6;
        public IZHand1 Hand1;
        public IZHand1 Hand2;
        public IZHand1 Hand3;
        public IZHand2 Hand4;
        public IZHand2 Hand5;
        public IZHand2 Hand6;

        public bool ZerosSpawned = false;
        public bool Reseting = false;
        public Vector2 topVisualOffset = default(Vector2);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infinity Zero");
            Main.npcFrameCount[npc.type] = 7;
        }

        public int varTime = 0;

        public int YvarOld = 0;

        public int XvarOld = 0;

        public bool HoriSwitch = false;
        public int f = 1;
        public float TargetDirection = (float)Math.PI / 2;
        public float s = 1;

        private bool OpeningLine = false;

        private int CoreFrame;
        private int CoreCounter;
        private int BodyFrame;
        private int BodyCounter;
        Rectangle IZFrame = new Rectangle(0, 0, 390, 472);

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.width = 38;
            npc.height = 44;
            npc.npcSlots = 100;
            npc.scale = 1f;
            npc.defense = 300;
            npc.dontTakeDamage = true;
            npc.lifeMax = 2500000;
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(30, 0, 0, 0);
            npc.boss = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;
            npc.chaseable = true;
            music = mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/IZ");
            npc.HitSound = SoundID.NPCHit44;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Sounds/IZRoar");
            bossBag = mod.ItemType("IZCache");
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 1.1f);
        }

        public float[] customAI = new float[6];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if ((Main.netMode == 2 || Main.dedServ))
            {
                writer.Write(customAI[0]);
                writer.Write(customAI[1]);
                writer.Write(customAI[2]);
                writer.Write(customAI[3]);
                writer.Write(customAI[4]);
                writer.Write(customAI[5]);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == 1)
            {
                customAI[0] = reader.ReadFloat();
                customAI[1] = reader.ReadFloat();
                customAI[2] = reader.ReadFloat();
                customAI[3] = reader.ReadFloat();
                customAI[4] = reader.ReadFloat();
                customAI[5] = reader.ReadFloat();
            }
        }
        public int roarTimer = 200;
        public bool[] roared = new bool[3];
        private int testime = 60;
        private int StormTimer = 0;
        public bool quarterHealth = false;
        public bool threeQuarterHealth = false;
        public bool HalfHealth = false;
        public bool fifthHealth = false;
        public bool OpenCore = false;
        public bool FirstCoreLine = false;

        public override void AI()
        {
            npc.timeLeft = 200;
            if (testime > 0)
            {
                testime--;
            }

            if (!OpeningLine)
            {
                if (!AAWorld.downedIZ)
                {
                    Main.NewText("Initiating Infinity Protocol. Engaging target.", new Color(158, 3, 32));
                }
                else
                {
                    Main.NewText("Initiating Infinity Protocol. Engaging known threat.", new Color(158, 3, 32));
                }
                OpeningLine = true;
            }

            StormTimer++;
            if (StormTimer >= 750)
            {
                StormTimer = 0;
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, npc.velocity.X * 2f, npc.velocity.Y * 2f, mod.ProjectileType("InfinityStorm"), npc.damage, 0, Main.myPlayer);
            }

            if (Main.netMode != 2)
            {
                int ThreeQuartersHealth = npc.lifeMax * (int).75f;
                int HalfHealth = npc.lifeMax * (int).5f;
                int QuarterHealth = npc.lifeMax * (int).25f;

                if (roarTimer > -1) roarTimer--;
                if (npc.life <= ThreeQuartersHealth && !roared[0])
                {
                    roared[0] = true;
                    roarTimer = 200;
                }
                if (npc.life <= HalfHealth && !roared[1])
                {
                    roared[1] = true;
                    roarTimer = 200;
                }
                if (npc.life <= QuarterHealth && !roared[2])
                {
                    roared[2] = true;
                    roarTimer = 200;
                }
                if (roarTimer == 180)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Sounds/IZRoar"), npc.Center);
                }
            }

            Player player = Main.player[npc.target];
            /*if (player != null)
            {
                float dist = npc.Distance(player.Center);
                if (dist > 1200) //trigger teleporting stuff
                {
                    npc.dontTakeDamage = true;
                    npc.alpha += 10;
                    if (npc.alpha >= 255) //teleport, you're invisible!
                    {
                        npc.alpha = 254; //don't let it hit 255 or it will despawn!
                        Vector2 tele = new Vector2(player.Center.X, player.Center.Y);
                        npc.Center = tele;
                        npc.dontTakeDamage = false;
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Sounds/IZRoar"), npc.Center);
                    }
                }
                else //you're close to the player, so make sure you're visible!
                {
                    npc.dontTakeDamage = false; //to prevent you from being indestructible if the teleport is interrupted
                    npc.alpha -= 25;
                    if (npc.alpha <= 0)
                    {
                        npc.alpha = 0;
                    }
                }
            }*/

            //commented out since code won't teleport and allows body to be damaged...

            float movementMax = 1.5f;
            if (fifthHealth)
            {
                movementMax = 3f;
            }
            if (npc.target > -1)
            {
                Player targetPlayer = Main.player[npc.target];
                if (!targetPlayer.dead) //speed changes depending on how far the player is
                {
                    npc.alpha -= 10;
                    if (npc.alpha <= 0)
                    {
                        npc.alpha = 0;
                    }
                    movementMax = MathHelper.Lerp(1f, 4f, Math.Min(1f, Math.Max(0f, (Vector2.Distance(npc.Center, targetPlayer.Center) / 1000f))));
                }
                if (targetPlayer.dead) //speed changes depending on how far the player is
                {
                    npc.alpha += 10;
                    if (npc.alpha >= 255)
                    {
                        npc.active = false;
                    }
                }
            }
            //customAI is used here because the original ai and localAI are both used elsewhere. It is synced above.
            BaseAI.AIElemental(npc, ref customAI, false, 0, false, false, 800f, 600f, 60, movementMax);
            if (!ZerosSpawned)
            {
                if (Main.netMode != 1)
                {
                    int latestNPC = npc.whoAmI;
                    int handType = 0;
                    latestNPC = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 100, mod.NPCType("IZHand1"), 0, npc.whoAmI);
                    Main.npc[latestNPC].ai[0] = npc.whoAmI;
                    Main.npc[latestNPC].ai[1] = handType;
                    handType++;
                    Zero1 = Main.npc[latestNPC];
                    if (Zero1.type == mod.NPCType("IZHand1"))
                    {
                        Hand1 = (IZHand1)Zero1.modNPC;
                    }
                    latestNPC = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 100, mod.NPCType("IZHand1"), 0, npc.whoAmI);
                    Main.npc[latestNPC].ai[0] = npc.whoAmI;
                    Main.npc[latestNPC].ai[1] = handType;
                    handType++;
                    Zero2 = Main.npc[latestNPC];
                    if (Zero2.type == mod.NPCType("IZHand1"))
                    {
                        Hand2 = (IZHand1)Zero2.modNPC;
                    }
                    latestNPC = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 100, mod.NPCType("IZHand1"), 0, npc.whoAmI);
                    Main.npc[latestNPC].ai[0] = npc.whoAmI;
                    Main.npc[latestNPC].ai[1] = handType;
                    handType++;
                    Zero3 = Main.npc[latestNPC];
                    if (Zero3.type == mod.NPCType("IZHand1"))
                    {
                        Hand3 = (IZHand1)Zero3.modNPC;
                    }
                    latestNPC = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 100, mod.NPCType("IZHand2"), 0, npc.whoAmI);
                    Main.npc[latestNPC].ai[0] = npc.whoAmI;
                    Main.npc[latestNPC].ai[1] = handType;
                    handType++;
                    Zero4 = Main.npc[latestNPC];
                    if (Zero4.type == mod.NPCType("IZHand2"))
                    {
                        Hand4 = (IZHand2)Zero4.modNPC;
                    }
                    latestNPC = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 100, mod.NPCType("IZHand2"), 0, npc.whoAmI);
                    Main.npc[latestNPC].ai[0] = npc.whoAmI;
                    Main.npc[latestNPC].ai[1] = handType;
                    handType++;
                    Zero5 = Main.npc[latestNPC];
                    if (Zero5.type == mod.NPCType("IZHand2"))
                    {
                        Hand5 = (IZHand2)Zero5.modNPC;
                    }
                    latestNPC = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 100, mod.NPCType("IZHand2"), 0, npc.whoAmI);
                    Main.npc[latestNPC].ai[0] = npc.whoAmI;
                    Main.npc[latestNPC].ai[1] = handType;
                    Zero6 = Main.npc[latestNPC];
                    if (Zero6.type == mod.NPCType("IZHand2"))
                    {
                        Hand6 = (IZHand2)Zero6.modNPC;
                    }
                    npc.ai[0] = 600;
                    npc.netUpdate = true;
                }
                ZerosSpawned = true;
            }
            if (testime == 0 && (Zero1 == null || Zero2 == null || Zero3 == null || Zero4 == null || Zero5 == null || Zero6 == null || !Zero1.active || !Zero2.active || !Zero3.active || !Zero4.active || !Zero5.active || !Zero6.active))
            {
                Reseting = true;
                testime = 60;
            }
            if ((Zero1 == null || !Zero1.active) && (Zero2 == null || !Zero2.active) && (Zero3 == null || !Zero3.active) && (Zero4 == null || !Zero4.active) && (Zero5 == null || !Zero5.active) && (Zero6 == null || !Zero6.active))
            {
                ZerosSpawned = false;
            }
            for (int m = npc.oldPos.Length - 1; m > 0; m--)
            {
                npc.oldPos[m] = npc.oldPos[m - 1];
            }
            npc.oldPos[0] = npc.position;

            if (npc.ai[3] == 6)
            {
                customAI[4] = 1;
                npc.ai[0]--;
                npc.ai[2] = 1;
                customAI[5] = 60;
                npc.dontTakeDamage = false;
                if (!FirstCoreLine)
                {
                    FirstCoreLine = true;
                    BaseUtility.Chat("Zero Units in critical condition. Rerouting resources to repair systems. Core defense temporarily disabled.", new Color(158, 3, 32));
                }
                if (npc.ai[0] <= 0)
                {
                    BaseUtility.Chat("Zero Units sufficiently repaired. Reengaging Core defense system.", new Color(158, 3, 32));
                    if (Hand1 != null && Hand2 != null && Hand3 != null && Hand4 != null && Hand5 != null && Hand6 != null && Hand1.npc.active && Hand2.npc.active && Hand3.npc.active && Hand4.npc.active && Hand5.npc.active && Hand6.npc.active)
                    {
                        Hand1.RepairMode = false;
                        Hand2.RepairMode = false;
                        Hand3.RepairMode = false;
                        Hand4.RepairMode = false;
                        Hand5.RepairMode = false;
                        Hand6.RepairMode = false;
                    }
                    npc.ai[3] = 0;
                    npc.ai[2] = 0;
                    npc.ai[0] = 600;
                    npc.dontTakeDamage = true;
                }
            }
            else
            {
                customAI[5]--;
                if (customAI[5] <= 0)
                {
                    customAI[4] = 0;
                }
            }
        }

        public bool Dead = false;

        public override void NPCLoot()
        {
            Dead = true;
            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType<Oblivion>(), 0, 0);
            AAPlayer.ZeroKills += 1;
            AAWorld.downedIZ = true;
            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else
            {
                npc.DropLoot(mod.ItemType("Infinitium"), 25, 35);
                string[] lootTable =
                {
                    "Genocide",
                    "Nova",
                    "Sagittarius",
                    "TotalDestruction",
                    "Annihilator",
                    "InfinityBlade"
                };
                int loot = Main.rand.Next(lootTable.Length);
                npc.DropLoot(mod.ItemType(lootTable[loot]));
                npc.DropLoot(Items.Boss.EXSoul.type);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter > 5)
            {
                npc.frameCounter = 0;
                CoreCounter += 1;
            }
            if (CoreCounter > 6)
            {
                CoreCounter = 0;
            }
            if (roarTimer > -1)
            {
                if (npc.ai[2] == 1)
                {
                    IZFrame.Y = 3 * IZFrame.Height;
                }
                else
                {
                    IZFrame.Y = 2 * IZFrame.Height;
                }
            }
            else
            {
                if (npc.ai[2] == 1)
                {
                    IZFrame.Y = IZFrame.Height;
                }
                else
                {
                    IZFrame.Y = 0;
                }
            }
            npc.frame.Y = CoreCounter * frameHeight;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[npc.target];
            if (player.vortexStealthActive && projectile.ranged)
            {
                damage /= 2;
                crit = false;
            }
            if (projectile.penetrate == -1 && !projectile.minion)
            {
                projectile.damage *= (int).2;
            }
            else if (projectile.penetrate >= 1)
            {
                projectile.damage *= (int).2;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = mod.ItemType("GrandHealingPotion");
        }

        private void ModifyHit(ref int damage)
        {
            damage = (int)(damage * 0.6f);
            if (damage >= 800)
            {
                damage = 800;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 3f;
            return null;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= ((npc.lifeMax / 4) * 3) && threeQuarterHealth == false)
            {
                if (Main.netMode != 1) BaseUtility.Chat("WARNING. Systems have reached 75% efficiency.", new Color(158, 3, 32));
                threeQuarterHealth = true;
                roarTimer = 200;
            }
            if (npc.life <= npc.lifeMax / 2 && HalfHealth == false)
            {
                if (Main.netMode != 1) BaseUtility.Chat("Redirecting resources to offensive systems.", new Color(158, 3, 32));
                HalfHealth = true;
                npc.defense = 250;
                IZHand1.damageIdle = 250;
                IZHand1.damageCharging = 350;
                roarTimer = 200;
            }
            if (npc.life <= npc.lifeMax / 4 && quarterHealth == false)
            {
                if (Main.netMode != 1) BaseUtility.Chat("CRITICAL WARNING. Systems have reached 25% efficiency. Failure imminent.", new Color(158, 3, 32));
                quarterHealth = true;
                roarTimer = 200;
            }
            if (npc.life <= npc.lifeMax / 6 && !fifthHealth)
            {
                fifthHealth = true;
                if (Main.netMode != 1) BaseUtility.Chat("Terrarian, you will not win this. Rerouting all resources to offensive systems.", new Color(158, 3, 32));
                npc.defense = 200;
                IZHand1.damageIdle = 350;
                IZHand1.damageCharging = 500;
                roarTimer = 200;
            }

            if (npc.life <= npc.lifeMax / 6)
            {
                music = mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/LastStand");
            }
            if (npc.life <= 0)
            {
                float randomSpread = (Main.rand.Next(-50, 50) / 100);
                Gore.NewGore(npc.Center, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/IZGore1"), 1f);
                Gore.NewGore(npc.Center, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/IZGore2"), 1f);
                Gore.NewGore(npc.Center, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/IZGore3"), 1f);
                Gore.NewGore(npc.Center, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/IZGore4"), 1f);
                Gore.NewGore(npc.Center, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/IZGore5"), 1f);
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 400;
                npc.height = 350;
                npc.position.X = npc.position.X - npc.width / 2;
                npc.position.Y = npc.position.Y - npc.height / 2;
                for (int num621 = 0; num621 < 60; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType<Dusts.VoidDust>(), 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 90; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType<Dusts.VoidDust>(), 0f, 0f, 100, default(Color), 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public static Color infinityGlowRed = new Color(233, 53, 53);
        public static Color GetGlowAlpha(bool aura)
        {
            return (aura ? infinityGlowRed : Color.White) * (Main.mouseTextColor / 255f);
        }

        public Color GetRedAlpha()
        {
            return new Color(233, 53, 53) * (Main.mouseTextColor / 255f);
        }

        public static Texture2D glowTex = null;
        public float auraPercent = 0f;
        public bool auraDirection = true;
        public bool saythelinezero = false;

        public Vector2 GetConnectionPoint(int handType)
        {
            float offsetX = 0, offsetY = 0;
            switch (handType)
            {
                case 0: offsetX = -62; offsetY = -80; break;
                case 1: offsetX = -32; offsetY = -44; break;
                case 2: offsetX = -46; offsetY = -20; break;
                case 3: offsetX = 62; offsetY = -80; break;
                case 4: offsetX = 32; offsetY = -44; break;
                case 5: offsetX = 46; offsetY = -20; break;
                default: break;
            }
            offsetX *= 2f;
            offsetY *= 2f;
            return new Vector2(offsetX, offsetY);
        }

        public Color GetAlpha(Color newColor, float alph)
        {
            int alpha = 255 - (int)(255 * alph);
            float alphaDiff = (float)(255 - alpha) / 255f;
            int newR = (int)((float)newColor.R * alphaDiff);
            int newG = (int)((float)newColor.G * alphaDiff);
            int newB = (int)((float)newColor.B * alphaDiff);
            int newA = (int)newColor.A - alpha;
            if (newA < 0) newA = 0;
            if (newA > 255) newA = 255;
            return new Color(newR, newG, newB, newA);
        }

        public override bool PreDraw(SpriteBatch sb, Color dColor)
        {
            Color AlphaColor = GetAlpha(dColor, npc.alpha);
            if (glowTex == null)
            {
                glowTex = mod.GetTexture("NPCs/Bosses/Infinity/Infinity_Glow");
            }

            Texture2D BodyTex = mod.GetTexture("NPCs/Bosses/Infinity/InfinityBody");
            Texture2D glowTex1 = mod.GetTexture("Glowmasks/InfinityCore_Glow");
            Vector2 drawCenter = new Vector2(npc.Center.X, npc.Center.Y);
            if (auraDirection) { auraPercent += 0.1f; auraDirection = auraPercent < 1f; }
            else { auraPercent -= 0.1f; auraDirection = auraPercent <= 0f; }
            BaseDrawing.DrawTexture(sb, Main.npcTexture[npc.type], 0, npc, AlphaColor);
            BaseDrawing.DrawTexture(sb, glowTex1, 0, npc, AAColor.Oblivion);
            if (fifthHealth)
            {
                BaseDrawing.DrawTexture(sb, BodyTex, 0, npc.position, npc.width, npc.height + 225, npc.scale, npc.rotation, 0, 4, IZFrame, AlphaColor);
                BaseDrawing.DrawAura(sb, glowTex, 0, npc, auraPercent, 1f, 0f, 0f, GetRedAlpha());
                BaseDrawing.DrawTexture(sb, BodyTex, 0, npc.position, npc.width, npc.height + 225, npc.scale, npc.rotation, 0, 4, IZFrame, GetRedAlpha());
            }
            else
            {
                BaseDrawing.DrawTexture(sb, BodyTex, 0, npc.position, npc.width, npc.height + 225, npc.scale, npc.rotation, 0, 4, IZFrame, AlphaColor);
                BaseDrawing.DrawAura(sb, glowTex, 0, npc, auraPercent, 1f, 0f, 0f, GetGlowAlpha(true));
                BaseDrawing.DrawTexture(sb, BodyTex, 0, npc.position, npc.width, npc.height + 225, npc.scale, npc.rotation, 0, 4, IZFrame, GetGlowAlpha(false));
            }


            string ZeroTex = ("NPCs/Bosses/Infinity/IZHand1");

            //bottom arms
            DrawZero(sb, ZeroTex, ZeroTex + "_Glow", Zero6, AlphaColor);
            DrawZero(sb, ZeroTex, ZeroTex + "_Glow", Zero3, AlphaColor);
            //middle arms
            DrawZero(sb, ZeroTex, ZeroTex + "_Glow", Zero5, AlphaColor);
            DrawZero(sb, ZeroTex, ZeroTex + "_Glow", Zero2, AlphaColor);
            //top arms
            DrawZero(sb, ZeroTex, ZeroTex + "_Glow", Zero4, AlphaColor);
            DrawZero(sb, ZeroTex, ZeroTex + "_Glow", Zero1, AlphaColor);


            return false;
        }

        public void DrawZero(SpriteBatch spriteBatch, string zeroTexture, string glowMaskTexture, NPC Zero, Color drawColor)
        {
            if (Zero != null && Zero.active && Zero.modNPC != null && (Zero.modNPC is IZHand1 || Zero.modNPC is IZHand2))
            {
                IZHand1 handNPC = (IZHand1)Zero.modNPC;
                string ArmTex = ("NPCs/Bosses/Infinity/IZArm");
                Texture2D ArmTex2D = mod.GetTexture(ArmTex);
                Texture2D zeroTex = mod.GetTexture(zeroTexture);
                Texture2D glowTex = mod.GetTexture(glowMaskTexture);
                Vector2 ArmOrigin = new Vector2(npc.Center.X, npc.Center.Y) + GetConnectionPoint(handNPC.handType);
                Vector2 connector = Zero.Center;
                BaseDrawing.DrawChain(spriteBatch, new Texture2D[] { ArmTex2D, ArmTex2D, ArmTex2D }, 0, ArmOrigin, connector, ArmTex2D.Height - 10f, null, 1f, false, null);
                BaseDrawing.DrawTexture(spriteBatch, zeroTex, 0, Zero, BaseUtility.ColorClamp(BaseDrawing.GetNPCColor(Zero), GetGlowAlpha(true)));
                if (fifthHealth)
                {
                    BaseDrawing.DrawAura(spriteBatch, glowTex, 0, Zero, auraPercent, 1f, 0f, 0f, GetGlowAlpha(true));
                    BaseDrawing.DrawTexture(spriteBatch, glowTex, 0, Zero, GetRedAlpha());
                }
                else
                {
                    BaseDrawing.DrawTexture(spriteBatch, glowTex, 0, Zero, GetGlowAlpha(false));
                }
            }
        }

    }
}
