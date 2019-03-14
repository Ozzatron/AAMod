using System;
using System.IO;
using BaseMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AAMod.NPCs.Bosses.Zero
{
    public class NovaFocus : ModNPC
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void Star");
            Main.npcFrameCount[npc.type] = 2;
        }
        public override void SetDefaults()
        {
            npc.width = 40;
            npc.height = 54;
            npc.damage = 30;
            npc.defense = 40;
            npc.lifeMax = 37500;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCHit4;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0.0f;
            animationType = NPCID.PrimeVice;
            npc.buffImmune[20] = true;
            npc.buffImmune[24] = true;
            npc.buffImmune[39] = true;
            npc.lavaImmune = true;
            npc.netAlways = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
        }

        public float[] internalAI = new float[4];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if ((Main.netMode == 2 || Main.dedServ))
            {
                writer.Write((short)npc.localAI[0]);
                writer.Write((float)internalAI[0]);
                writer.Write((float)internalAI[1]);
                writer.Write((float)internalAI[2]);
                writer.Write((float)internalAI[3]);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == 1)
            {
                npc.localAI[0] = reader.ReadInt16();
                internalAI[0] = reader.ReadFloat();
                internalAI[1] = reader.ReadFloat();
                internalAI[2] = reader.ReadFloat();
                internalAI[3] = reader.ReadFloat();
            }
        }



        public override void HitEffect(int hitDirection, double damage)
        {
            bool flag = (npc.life <= 0 || (!npc.active && NPC.AnyNPCs(mod.NPCType<Zero>())));
            if (flag && Main.netMode != 1)
            {
                int ind = NPC.NewNPC((int)(npc.position.X + (double)(npc.width / 2)), (int)npc.position.Y + (npc.height / 2), mod.NPCType("TeslaHand"), npc.whoAmI, -1.5f, npc.ai[1], 0f, 0f, byte.MaxValue);
                Main.npc[ind].life = 1;
                Main.npc[ind].rotation = npc.rotation;
                Main.npc[ind].velocity = npc.velocity;
                Main.npc[ind].netUpdate = true;
                Main.npc[(int)npc.ai[1]].ai[3]++;
                Main.npc[(int)npc.ai[1]].netUpdate = true;
            }
        }

        int LaserTime = 0;
        Projectile laser;

        public override void AI()
        {
            npc.spriteDirection = -(int)npc.ai[0];
            if (!Main.npc[(int)npc.ai[1]].active)
            {
                npc.ai[2] += 10f;
                if (npc.ai[2] > 50.0 || Main.netMode != 2)
                {
                    npc.life = -1;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                }
            }
            if (npc.ai[2] == 0.0 || npc.ai[2] == 3.0)
            {
                if (Main.npc[(int)npc.ai[1]].ai[1] == 3.0 && npc.timeLeft > 10)
                    npc.timeLeft = 10;
                if (Main.npc[(int)npc.ai[1]].ai[1] != 0f)
                {
                    npc.localAI[0] += 3f;
                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 100.0)
                    {
                        if (npc.velocity.Y > 0.0)
                            npc.velocity.Y *= 0.96f;
                        npc.velocity.Y -= 0.07f;
                        if (npc.velocity.Y > 6.0)
                            npc.velocity.Y = 6f;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 100.0)
                    {
                        if (npc.velocity.Y < 0.0)
                            npc.velocity.Y *= 0.96f;
                        npc.velocity.Y += 0.07f;
                        if (npc.velocity.Y < -6.0)
                            npc.velocity.Y = -6f;
                    }
                    if (npc.position.X + (double)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (double)(Main.npc[(int)npc.ai[1]].width / 2) - (120.0 * npc.ai[0]))
                    {
                        if (npc.velocity.X > 0.0)
                            npc.velocity.X *= 0.96f;
                        npc.velocity.X -= 0.1f;
                        if (npc.velocity.X > 8.0)
                            npc.velocity.X = 8f;
                    }
                    if (npc.position.X + (double)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (double)(Main.npc[(int)npc.ai[1]].width / 2) - (120.0 * npc.ai[0]))
                    {
                        if (npc.velocity.X < 0.0)
                            npc.velocity.X *= 0.96f;
                        npc.velocity.X += 0.1f;
                        if (npc.velocity.X < -8.0)
                            npc.velocity.X = -8f;
                    }
                }
                else
                {
                    ++npc.ai[3];
                    if (npc.ai[3] >= 800.0)
                    {
                        ++npc.ai[2];
                        npc.ai[3] = 0.0f;
                        npc.netUpdate = true;
                    }
                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 100.0)
                    {
                        if (npc.velocity.Y > 0.0)
                            npc.velocity.Y *= 0.96f;
                        npc.velocity.Y -= 0.1f;
                        if (npc.velocity.Y > 3.0)
                            npc.velocity.Y = 3f;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 100.0)
                    {
                        if (npc.velocity.Y < 0.0)
                            npc.velocity.Y *= 0.96f;
                        npc.velocity.Y += 0.1f;
                        if (npc.velocity.Y < -3.0)
                            npc.velocity.Y = -3f;
                    }
                    if (npc.position.X + (double)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (double)(Main.npc[(int)npc.ai[1]].width / 2) - (180.0 * npc.ai[0]))
                    {
                        if (npc.velocity.X > 0.0)
                            npc.velocity.X *= 0.96f;
                        npc.velocity.X -= 0.14f;
                        if (npc.velocity.X > 8.0)
                            npc.velocity.X = 8f;
                    }
                    if (npc.position.X + (double)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (double)(Main.npc[(int)npc.ai[1]].width / 2) - (180.0 * npc.ai[0]))
                    {
                        if (npc.velocity.X < 0.0)
                            npc.velocity.X *= 0.96f;
                        npc.velocity.X += 0.14f;
                        if (npc.velocity.X < -8.0)
                            npc.velocity.X = -8f;
                    }
                }
                npc.TargetClosest(true);
                Vector2 vector2 = new Vector2(npc.position.X + (npc.width * 0.5f), npc.position.Y + (npc.height * 0.5f));
                float num1 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector2.X;
                float num2 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector2.Y;
                float num3 = (float)Math.Sqrt((num1 * (double)num1) + (num2 * (double)num2));
                float NewRotation = (float)Math.Atan2(num2, num1);
                npc.rotation = MathHelper.Lerp(npc.rotation, NewRotation, 1f / 20f);
                ++npc.localAI[0];
                if (npc.localAI[0] <= 200.0)
                    return;
                if (npc.localAI[0] > 360)
                {
                    npc.localAI[0] = 0.0f;
                    LaserTime = 0;
                }
                LaserTime++;
                if (LaserTime >= 600)
                {
                    internalAI[0] = 0;
                    if (Main.netMode != 1) laser.Kill();
                }
                else if (LaserTime >= 300)
                {
                    internalAI[1] = 100;

                }
                else if (LaserTime > 120)
                {
                    internalAI[1] -= 400 / 180;

                }
                else if (LaserTime == 120 && Main.netMode != 1)
                {
                    laser = Main.projectile[Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, mod.ProjectileType<NovaRay>(), (int)(npc.damage * 0.75f), 3f, Main.myPlayer, npc.whoAmI, 420)];
                }
                else
                {
                    internalAI[1] = 500;
                }
            }
            else
            {
                if (npc.ai[2] != 1.0)
                {
                    return;
                }
                ++npc.ai[3];
                if (npc.ai[3] >= 200.0)
                {
                    npc.localAI[0] = 0.0f;
                    npc.ai[2] = 0.0f;
                    npc.ai[3] = 0.0f;
                    npc.netUpdate = true;
                }
                Vector2 vector2 = new Vector2(npc.position.X + (npc.width * 0.5f), npc.position.Y + (npc.height * 0.5f));
                float num1 = (float)(Main.player[npc.target].position.X + (double)(Main.player[npc.target].width / 2) - 350.0) - vector2.X;
                float num2 = (float)(Main.player[npc.target].position.Y + (double)(Main.player[npc.target].height / 2) - 20.0) - vector2.Y;
                float num3 = 7f / (float)Math.Sqrt((num1 * (double)num1) + (num2 * (double)num2));
                float num4 = num1 * num3;
                float num5 = num2 * num3;
                if (npc.velocity.X > (double)num4)
                {
                    if (npc.velocity.X > 0.0)
                        npc.velocity.X *= 0.9f;
                    npc.velocity.X -= 0.1f;
                }
                if (npc.velocity.X < (double)num4)
                {
                    if (npc.velocity.X < 0.0)
                        npc.velocity.X *= 0.9f;
                    npc.velocity.X += 0.1f;
                }
                if (npc.velocity.Y > (double)num5)
                {
                    if (npc.velocity.Y > 0.0)
                        npc.velocity.Y *= 0.9f;
                    npc.velocity.Y -= 0.03f;
                }
                if (npc.velocity.Y < (double)num5)
                {
                    if (npc.velocity.Y < 0.0)
                        npc.velocity.Y *= 0.9f;
                    npc.velocity.Y += 0.03f;
                }
                npc.TargetClosest(true);
                vector2 = new Vector2(npc.position.X + (npc.width * 0.5f), npc.position.Y + (npc.height * 0.5f));
                float num6 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector2.X;
                float num7 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector2.Y;
                float num8 = (float)Math.Sqrt((num6 * (double)num6) + (num7 * (double)num7));
                float NewRotation = (float)Math.Atan2(num7, num6);
                npc.rotation = MathHelper.Lerp(npc.rotation, NewRotation, 1f / 20f);
                if (Main.netMode != 1)
                    return;
                ++npc.localAI[0];
                if (npc.localAI[0] <= 80.0)
                    return;
                if (npc.localAI[0] > 160)
                {
                    npc.localAI[0] = 0.0f;
                    LaserTime = 0;
                }
                LaserTime++;
                if (LaserTime >= 600)
                {
                    internalAI[0] = 0;
                    if (Main.netMode != 1) laser.Kill();
                }
                else if (LaserTime >= 300)
                {
                    internalAI[1] = 100;

                }
                else if (LaserTime > 120)
                {
                    internalAI[1] -= 400 / 180;

                }
                else if (LaserTime == 120 && Main.netMode != 1)
                {
                    laser = Main.projectile[Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, mod.ProjectileType<NovaRay>(), (int)(npc.damage * 0.75f), 3f, Main.myPlayer, npc.whoAmI, 420)];
                    laser.velocity = BaseUtility.RotateVector(default(Vector2), new Vector2(14f, 0f), laser.rotation);
                }
                else
                {
                    internalAI[1] = 500;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 vector7 = new Vector2(npc.position.X + ((float)npc.width * 0.5f) - (5f * npc.ai[0]), npc.position.Y + 20f);
            for (int l = 0; l < 2; l++)
            {
                float num21 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector7.X;
                float num22 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector7.Y;
                float num23;
                if (l == 0)
                {
                    num21 -= 200f * npc.ai[0];
                    num22 += 130f;
                    num23 = (float)Math.Sqrt((double)((num21 * num21) + (num22 * num22)));
                    num23 = 92f / num23;
                    vector7.X += num21 * num23;
                    vector7.Y += num22 * num23;
                }
                else
                {
                    num21 -= 50f * npc.ai[0];
                    num22 += 80f;
                    num23 = (float)Math.Sqrt((double)((num21 * num21) + (num22 * num22)));
                    num23 = 60f / num23;
                    vector7.X += num21 * num23;
                    vector7.Y += num22 * num23;
                }
                float rotation7 = (float)Math.Atan2((double)num22, (double)num21) - 1.57f;
                Color color7 = Lighting.GetColor((int)vector7.X / 16, (int)(vector7.Y / 16f));
                Texture2D Arm = mod.GetTexture("NPCs/Bosses/Zero/ZeroArm");
                Texture2D ArmGlow = mod.GetTexture("Glowmasks/ZeroArm_Glow");
                Main.spriteBatch.Draw(Arm, new Vector2(vector7.X - Main.screenPosition.X, vector7.Y - Main.screenPosition.Y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, Arm.Width, Arm.Height)), color7, rotation7, new Vector2((float)Main.boneArmTexture.Width * 0.5f, (float)Main.boneArmTexture.Height * 0.5f), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(ArmGlow, new Vector2(vector7.X - Main.screenPosition.X, vector7.Y - Main.screenPosition.Y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, Arm.Width, Arm.Height)), GetGlowAlpha(), rotation7, new Vector2((float)Main.boneArmTexture.Width * 0.5f, (float)Main.boneArmTexture.Height * 0.5f), 1f, SpriteEffects.None, 0f);
                if (l == 0)
                {
                    vector7.X += num21 * num23 / 2f;
                    vector7.Y += num22 * num23 / 2f;
                }
                else if (Main.rand.Next(2) == 0)
                {

                    vector7.X += (num21 * num23) - 16f;
                    vector7.Y += (num22 * num23) - 6f;
                    int num24 = Dust.NewDust(new Vector2(vector7.X, vector7.Y), 30, 10, mod.DustType<Dusts.VoidDust>(), num21 * 0.02f, num22 * 0.02f, 0, default(Color), 2.5f);
                    Main.dust[num24].noGravity = false;
                }
            }
            return base.PreDraw(spriteBatch, drawColor);
        }

        public Color GetGlowAlpha()
        {
            return AAColor.ZeroShield * (Main.mouseTextColor / 255f);
        }

        public static Texture2D glowTex = null;
        public float auraPercent = 0f;
        public bool auraDirection = true;

        

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (glowTex == null)
            {
                glowTex = mod.GetTexture("Glowmasks/NovaFocus_Glow");
            }
            if (auraDirection) { auraPercent += 0.1f; auraDirection = auraPercent < 1f; }
            else { auraPercent -= 0.1f; auraDirection = auraPercent <= 0f; }
            BaseMod.BaseDrawing.DrawAura(spriteBatch, glowTex, 0, npc, auraPercent, 1f, 0f, 0f, GetGlowAlpha());
            BaseMod.BaseDrawing.DrawTexture(spriteBatch, glowTex, 0, npc, GetGlowAlpha());
        }

    }
}
