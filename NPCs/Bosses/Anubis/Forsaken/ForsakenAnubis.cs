﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AAMod.Items.Boss.Anubis;
using BaseMod;
using System.IO;

namespace AAMod.NPCs.Bosses.Anubis.Forsaken
{
    public class ForsakenAnubis : ModNPC
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Anubis; Forsaken Judge");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 76;
            npc.height = 100;
            npc.aiStyle = -1;
            npc.damage = 55;
            npc.defense = 60;
            npc.lifeMax = 180000;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath6;
            npc.knockBackResist = 0f;
            npc.boss = true;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/AnubisA");
        }

        public float[] internalAI = new float[4];

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(internalAI[0]);
                writer.Write(internalAI[1]);
                writer.Write(internalAI[2]);
                writer.Write(internalAI[3]);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                internalAI[0] = reader.ReadFloat();
                internalAI[1] = reader.ReadFloat();
                internalAI[2] = reader.ReadFloat();
                internalAI[3] = reader.ReadFloat();
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.75f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }

        public int RuneCount = 9;

        public override void AI()
        {
            if (!npc.HasPlayerTarget)
            {
                npc.TargetClosest();
            }

            Player player = Main.player[npc.target];

            if (player.Center.X < npc.Center.X)
            {
                npc.direction = npc.spriteDirection = 1;
            }
            else
            {
                npc.direction = npc.spriteDirection = -1;
            }

            npc.dontTakeDamage = false;
            npc.noGravity = true;

            if (internalAI[0] == 0)
            {
                npc.velocity.Y += 0.002f;
                if (npc.velocity.Y > .1f)
                {
                    internalAI[0] = 1f;
                    npc.netUpdate = true;
                }
            }
            else
            if (internalAI[0] == 1)
            {
                npc.velocity.Y -= 0.002f;
                if (npc.velocity.Y < -.1f)
                {
                    internalAI[0] = 0f;
                    npc.netUpdate = true;
                }
            }

            if (npc.life < npc.lifeMax / 3)
            {
                if (internalAI[2] == 0)
                {
                    if (Main.netMode != 1)
                    {
                        for (int m = 0; m < RuneCount; m++)
                        {
                            int p = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("CurseGlyphs"), npc.damage / 2, 0, Main.myPlayer);
                            Main.npc[p].Center = npc.Center;
                            Main.npc[p].velocity = new Vector2(MathHelper.Lerp(-1f, 1f, (float)Main.rand.NextDouble()), MathHelper.Lerp(-1f, 1f, (float)Main.rand.NextDouble()));
                            Main.npc[p].velocity *= 8f;
                            Main.npc[p].ai[0] = m;
                            Main.npc[p].netUpdate2 = true;
                            int dustType = ModContent.DustType<Dusts.JudgementDust>();
                            int pieCut = 20;
                            for (int i = 0; i < pieCut; i++)
                            {
                                int dustID = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, dustType, 0f, 0f, 100, Color.White, 1.6f);
                                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(6f, 0f), i / (float)pieCut * 6.28f);
                                Main.dust[dustID].noLight = false;
                                Main.dust[dustID].noGravity = true;
                            }
                            for (int i = 0; i < pieCut; i++)
                            {
                                int dustID = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, dustType, 0f, 0f, 100, Color.White, 2f);
                                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(9f, 0f), i / (float)pieCut * 6.28f);
                                Main.dust[dustID].noLight = false;
                                Main.dust[dustID].noGravity = true;
                            }
                        }
                    }
                    internalAI[2] = 1;
                }
                npc.damage = 70;
            }

            npc.ai[1]++;

            switch (npc.ai[0])
            {
                case 0:

                    int proj = Main.rand.Next(2) == 0 ? ModContent.ProjectileType<ForsakenBlast>() : ModContent.ProjectileType<ForsakenSkull>();

                    int damage = npc.damage / 2;

                    BaseAI.ShootPeriodic(npc, player.position, player.width, player.height, proj, ref npc.ai[3], 60, damage, 10, true);

                    if (npc.ai[3] == 30)
                    {
                        Teleport();
                    }

                    if (npc.ai[1] >= 200)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        Teleport();
                    }
                    break;
                case 1:

                    if (npc.ai[1] == 10)
                    {
                        if (Main.rand.Next(2) == 0 && npc.life < npc.lifeMax * (2/3))
                        {
                            if (npc.life < npc.lifeMax / 3)
                            {
                                int a = Projectile.NewProjectile(npc.position, Vector2.Zero, ModContent.ProjectileType<HorusSummon>(), 0, 0, Main.myPlayer, npc.Center.X - 200, npc.Center.Y);
                                Main.npc[a].Center = npc.Center;
                                int b = Projectile.NewProjectile(npc.position, Vector2.Zero, ModContent.ProjectileType<HorusSummon>(), 0, 0, Main.myPlayer, npc.Center.X + 200, npc.Center.Y);
                                Main.npc[b].Center = npc.Center;
                                int c = Projectile.NewProjectile(npc.position, Vector2.Zero, ModContent.ProjectileType<HorusSummon>(), 0, 0, Main.myPlayer, npc.Center.X, npc.Center.Y - 200);
                                Main.npc[c].Center = npc.Center;
                                int d = Projectile.NewProjectile(npc.position, Vector2.Zero, ModContent.ProjectileType<HorusSummon>(), 0, 0, Main.myPlayer, npc.Center.X, npc.Center.Y + 200);
                                Main.npc[d].Center = npc.Center;
                            }
                            else
                            {
                                int a = Projectile.NewProjectile(npc.position, Vector2.Zero, ModContent.ProjectileType<HorusSummon>(), 0, 0, Main.myPlayer, npc.Center.X - 180, npc.Center.Y - 60);
                                Main.npc[a].Center = npc.Center;
                                int b = Projectile.NewProjectile(npc.position, Vector2.Zero, ModContent.ProjectileType<HorusSummon>(), 0, 0, Main.myPlayer, npc.Center.X + 180, npc.Center.Y - 60);
                                Main.npc[b].Center = npc.Center;
                                int c = Projectile.NewProjectile(npc.position, Vector2.Zero, ModContent.ProjectileType<HorusSummon>(), 0, 0, Main.myPlayer, npc.Center.X, npc.Center.Y - 200);
                                Main.npc[c].Center = npc.Center;
                            }
                        }
                        else
                        {
                            if (npc.life < npc.lifeMax / 2)
                            {
                                int m = NPC.NewNPC((int)npc.position.X + 100, (int)npc.position.Y, ModContent.NPCType<CurseCircle>());
                                Main.npc[m].Center = new Vector2(npc.Center.X + 100, npc.Center.Y);

                                int n = NPC.NewNPC((int)npc.position.X - 100, (int)npc.position.Y, ModContent.NPCType<CurseCircle>());
                                Main.npc[n].Center = new Vector2(npc.Center.X - 100, npc.Center.Y);

                                int o = NPC.NewNPC((int)npc.position.X, (int)npc.position.Y + 100, ModContent.NPCType<CurseCircle>());
                                Main.npc[o].Center = new Vector2(npc.Center.X, npc.Center.Y + 100);

                                int p = NPC.NewNPC((int)npc.position.X, (int)npc.position.Y - 100, ModContent.NPCType<CurseCircle>());
                                Main.npc[p].Center = new Vector2(npc.Center.X, npc.Center.Y - 100);
                            }
                            else
                            {
                                int m = NPC.NewNPC((int)npc.position.X + 100, (int)npc.position.Y, ModContent.NPCType<CurseCircle>());
                                Main.npc[m].Center = new Vector2(npc.Center.X + 100, npc.Center.Y - 30);

                                int n = NPC.NewNPC((int)npc.position.X - 100, (int)npc.position.Y, ModContent.NPCType<CurseCircle>());
                                Main.npc[n].Center = new Vector2(npc.Center.X - 100, npc.Center.Y - 30);

                                int o = NPC.NewNPC((int)npc.position.X - 100, (int)npc.position.Y, ModContent.NPCType<CurseCircle>());
                                Main.npc[o].Center = new Vector2(npc.Center.X, npc.Center.Y + 130);
                            }
                        }
                    }

                    if (npc.ai[1] >= 130)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        Teleport();
                    }
                    break;
                case 2:

                    if (npc.ai[1] == 120)
                    {
                        BaseAI.FireProjectile(player.position, npc.position, ModContent.ProjectileType<ForsakenStaff>(), npc.damage / 2, 14, 10, -1);
                    }
                    if (npc.ai[1] == 140)
                    {
                        ScepterTeleport();
                    }

                    if (npc.ai[1] > 160 && !AAGlobalProjectile.AnyProjectiles(ModContent.ProjectileType<ForsakenStaff>()))
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        Teleport();
                    }

                    break;

                case 3:
                    if (npc.life > npc.lifeMax / 2)
                    {
                        if (npc.ai[1] == 40)
                        {
                            int l = Projectile.NewProjectile(player.position + new Vector2(-800, 0), Vector2.Zero, ModContent.ProjectileType<BlockF>(), npc.damage / 2, 7, Main.myPlayer, 0, 0);
                            int r = Projectile.NewProjectile(player.position + new Vector2(800, 0), Vector2.Zero, ModContent.ProjectileType<BlockF>(), npc.damage / 2, 7, Main.myPlayer, 1, 0);
                            Main.projectile[l].ai[1] = r;
                            Main.projectile[l].Center = player.Center + new Vector2(-800, 0);
                            Main.projectile[r].ai[1] = l;
                            Main.projectile[r].Center = player.Center + new Vector2(800, 0);
                        }
                        if (npc.ai[1] == 80)
                        {
                            int u = Projectile.NewProjectile(player.position + new Vector2(0, -800), Vector2.Zero, ModContent.ProjectileType<BlockF1>(), npc.damage / 2, 7, Main.myPlayer, 0, 0);
                            int d = Projectile.NewProjectile(player.position + new Vector2(0, 800), Vector2.Zero, ModContent.ProjectileType<BlockF1>(), npc.damage / 2, 7, Main.myPlayer, 1, 0);
                            Main.projectile[u].ai[1] = d;
                            Main.projectile[u].Center = player.Center + new Vector2(0, -800);
                            Main.projectile[d].ai[1] = u;
                            Main.projectile[d].Center = player.Center + new Vector2(0, 800);
                        }
                        if (npc.ai[1] > 160 && !AAGlobalProjectile.AnyProjectiles(ModContent.ProjectileType<Block>()))
                        {
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            Teleport();
                        }
                    }
                    else
                    {
                        if (npc.ai[1] % 30 == 0)
                        {
                            if (Main.rand.Next(2) == 0)
                            {
                                int l = Projectile.NewProjectile(player.position + new Vector2(-800, 0), Vector2.Zero, ModContent.ProjectileType<BlockF>(), npc.damage / 2, 7, Main.myPlayer, 0, 0);
                                int r = Projectile.NewProjectile(player.position + new Vector2(800, 0), Vector2.Zero, ModContent.ProjectileType<BlockF>(), npc.damage / 2, 7, Main.myPlayer, 1, 0);
                                Main.projectile[l].ai[1] = r;
                                Main.projectile[l].Center = player.Center + new Vector2(-800, 0);
                                Main.projectile[r].ai[1] = l;
                                Main.projectile[r].Center = player.Center + new Vector2(800, 0);
                            }
                            else
                            {
                                int u = Projectile.NewProjectile(player.position + new Vector2(0, -800), Vector2.Zero, ModContent.ProjectileType<BlockF1>(), npc.damage / 2, 7, Main.myPlayer, 0, 0);
                                int d = Projectile.NewProjectile(player.position + new Vector2(0, 800), Vector2.Zero, ModContent.ProjectileType<BlockF1>(), npc.damage / 2, 7, Main.myPlayer, 1, 0);
                                Main.projectile[u].ai[1] = d;
                                Main.projectile[u].Center = player.Center + new Vector2(0, -800);
                                Main.projectile[d].ai[1] = u;
                                Main.projectile[d].Center = player.Center + new Vector2(0, 800);
                            }
                        }

                        if (npc.ai[1] > 240 && !AAGlobalProjectile.AnyProjectiles(ModContent.ProjectileType<BlockF>()))
                        {
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            Teleport();
                        }
                    }
                    break;
                default:
                    npc.ai[0] = 0;
                    goto case 0;
            }
        }

        public override void NPCLoot()
        {
            NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, ModContent.NPCType<TownNPCs.Anubis>());
        }

        public void AliveCheck(Player player)
        {
            if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 5000f || !player.ZoneDesert)
            {
                npc.TargetClosest();
                if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 5000f || !player.ZoneDesert)
                {
                    if (Main.netMode != 1) BaseUtility.Chat("...Sorry, but you aren't ready yet.", Color.ForestGreen);
                    int a = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<TownNPCs.Anubis>());
                    Main.npc[a].Center = npc.Center;
                    npc.active = false;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter > 6)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y > frameHeight * 3)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public void Teleport()
        {
            Vector2 position = npc.Center + (Vector2.One * -20f);
            int num84 = 40;
            int height3 = num84;
            for (int num85 = 0; num85 < 3; num85++)
            {
                int num86 = Dust.NewDust(position, num84, height3, 240, 0f, 0f, 100, default, 1.5f);
                Main.dust[num86].position = npc.Center + (Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num84 / 2f);
            }
            for (int num87 = 0; num87 < 15; num87++)
            {
                int num88 = Dust.NewDust(position, num84, height3, ModContent.DustType<Dusts.ForsakenDust>(), 0f, 0f, 50, default, 3.7f);
                Main.dust[num88].position = npc.Center + (Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num84 / 2f);
                Main.dust[num88].noGravity = true;
                Main.dust[num88].noLight = true;
                Main.dust[num88].velocity *= 3f;
                Main.dust[num88].velocity += npc.DirectionTo(Main.dust[num88].position) * (2f + (Main.rand.NextFloat() * 4f));
                num88 = Dust.NewDust(position, num84, height3, ModContent.DustType<Dusts.ForsakenDust>(), 0f, 0f, 25, default, 1.5f);
                Main.dust[num88].position = npc.Center + (Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num84 / 2f);
                Main.dust[num88].velocity *= 2f;
                Main.dust[num88].noGravity = true;
                Main.dust[num88].fadeIn = 1f;
                Main.dust[num88].color = Color.Black * 0.5f;
                Main.dust[num88].noLight = true;
                Main.dust[num88].velocity += npc.DirectionTo(Main.dust[num88].position) * 8f;
            }
            for (int num89 = 0; num89 < 10; num89++)
            {
                int num90 = Dust.NewDust(position, num84, height3, ModContent.DustType<Dusts.ForsakenDust>(), 0f, 0f, 0, default, 2.7f);
                Main.dust[num90].position = npc.Center + (Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(npc.velocity.ToRotation(), default) * num84 / 2f);
                Main.dust[num90].noGravity = true;
                Main.dust[num90].noLight = true;
                Main.dust[num90].velocity *= 3f;
                Main.dust[num90].velocity += npc.DirectionTo(Main.dust[num90].position) * 2f;
            }
            for (int num91 = 0; num91 < 30; num91++)
            {
                int num92 = Dust.NewDust(position, num84, height3, ModContent.DustType<Dusts.ForsakenDust>(), 0f, 0f, 0, default, 1.5f);
                Main.dust[num92].position = npc.Center + (Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(npc.velocity.ToRotation(), default) * num84 / 2f);
                Main.dust[num92].noGravity = true;
                Main.dust[num92].velocity *= 3f;
                Main.dust[num92].velocity += npc.DirectionTo(Main.dust[num92].position) * 3f;
            }

            Player player = Main.player[npc.target];
            Vector2 targetPos = player.Center;
            int posX = Main.rand.Next(-400, 400);

            int posY = Main.rand.Next(0, 400);
            if (posX > -100 && posX < 100)
            {
                 posY = Main.rand.Next(100, 400);
            }

            npc.position = new Vector2(targetPos.X + posX, targetPos.Y - posY);
            int pieCut = 20;
            Main.PlaySound(SoundID.Item14, npc.position);
            for (int m = 0; m < pieCut; m++)
            {
                int dustID = Dust.NewDust(new Vector2(npc.Center.X - 1, npc.Center.Y - 1), 2, 2, ModContent.DustType<Dusts.JudgementDust>(), 0f, 0f, 100, Color.White, 1.6f);
                Main.dust[dustID].velocity = BaseMod.BaseUtility.RotateVector(default, new Vector2(6f, 0f), m / (float)pieCut * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
            for (int m = 0; m < pieCut; m++)
            {
                int dustID = Dust.NewDust(new Vector2(npc.Center.X - 1, npc.Center.Y - 1), 2, 2, ModContent.DustType<Dusts.JudgementDust>(), 0f, 0f, 100, Color.White, 2f);
                Main.dust[dustID].velocity = BaseMod.BaseUtility.RotateVector(default, new Vector2(9f, 0f), m / (float)pieCut * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
        }

        public void ScepterTeleport()
        {
            Vector2 position = npc.Center + (Vector2.One * -20f);
            int num84 = 40;
            int height3 = num84;
            for (int num85 = 0; num85 < 3; num85++)
            {
                int num86 = Dust.NewDust(position, num84, height3, 240, 0f, 0f, 100, default, 1.5f);
                Main.dust[num86].position = npc.Center + (Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num84 / 2f);
            }
            for (int num87 = 0; num87 < 15; num87++)
            {
                int num88 = Dust.NewDust(position, num84, height3, ModContent.DustType<Dusts.ForsakenDust>(), 0f, 0f, 50, default, 3.7f);
                Main.dust[num88].position = npc.Center + (Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num84 / 2f);
                Main.dust[num88].noGravity = true;
                Main.dust[num88].noLight = true;
                Main.dust[num88].velocity *= 3f;
                Main.dust[num88].velocity += npc.DirectionTo(Main.dust[num88].position) * (2f + (Main.rand.NextFloat() * 4f));
                num88 = Dust.NewDust(position, num84, height3, ModContent.DustType<Dusts.ForsakenDust>(), 0f, 0f, 25, default, 1.5f);
                Main.dust[num88].position = npc.Center + (Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num84 / 2f);
                Main.dust[num88].velocity *= 2f;
                Main.dust[num88].noGravity = true;
                Main.dust[num88].fadeIn = 1f;
                Main.dust[num88].color = Color.Black * 0.5f;
                Main.dust[num88].noLight = true;
                Main.dust[num88].velocity += npc.DirectionTo(Main.dust[num88].position) * 8f;
            }
            for (int num89 = 0; num89 < 10; num89++)
            {
                int num90 = Dust.NewDust(position, num84, height3, ModContent.DustType<Dusts.ForsakenDust>(), 0f, 0f, 0, default, 2.7f);
                Main.dust[num90].position = npc.Center + (Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(npc.velocity.ToRotation(), default) * num84 / 2f);
                Main.dust[num90].noGravity = true;
                Main.dust[num90].noLight = true;
                Main.dust[num90].velocity *= 3f;
                Main.dust[num90].velocity += npc.DirectionTo(Main.dust[num90].position) * 2f;
            }
            for (int num91 = 0; num91 < 30; num91++)
            {
                int num92 = Dust.NewDust(position, num84, height3, ModContent.DustType<Dusts.ForsakenDust>(), 0f, 0f, 0, default, 1.5f);
                Main.dust[num92].position = npc.Center + (Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(npc.velocity.ToRotation(), default) * num84 / 2f);
                Main.dust[num92].noGravity = true;
                Main.dust[num92].velocity *= 3f;
                Main.dust[num92].velocity += npc.DirectionTo(Main.dust[num92].position) * 3f;
            }

            Vector2 targetPos = Main.player[npc.target].Center;
            targetPos.X += 300 * (npc.Center.X < targetPos.X ? 1 : -1);
            targetPos.Y -= 300;
            npc.position = targetPos;

            int pieCut = 20;
            for (int m = 0; m < pieCut; m++)
            {
                int dustID = Dust.NewDust(new Vector2(npc.Center.X - 1, npc.Center.Y - 1), 2, 2, ModContent.DustType<Dusts.JudgementDust>(), 0f, 0f, 100, Color.White, 1f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(6f, 0f), m / (float)pieCut * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
            for (int m = 0; m < pieCut; m++)
            {
                int dustID = Dust.NewDust(new Vector2(npc.Center.X - 1, npc.Center.Y - 1), 2, 2, ModContent.DustType<Dusts.JudgementDust>(), 0f, 0f, 100, Color.White, 1.5f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(9f, 0f), m / (float)pieCut * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
        }
    }
}