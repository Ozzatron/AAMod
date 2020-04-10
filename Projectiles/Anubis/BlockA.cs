﻿using BaseMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AAMod.Projectiles.Anubis
{
    public class BlockA : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 128;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
        }

        public float[] internalAI = new float[1];

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(internalAI[0]);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                internalAI[0] = reader.ReadFloat();
            }
        }

        public override void AI()
        {
            for (int m = projectile.oldPos.Length - 1; m > 0; m--)
            {
                projectile.oldPos[m] = projectile.oldPos[m - 1];
            }
            projectile.oldPos[0] = projectile.position;

            if (projectile.frame < 5)
            {
                if (projectile.frameCounter++ > 2)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                }
            }
            else
            {
                projectile.frame = 5;
            }

            if (internalAI[0]++ > 40)
            {
                if (projectile.ai[0] == 0)
                {
                    if (projectile.velocity.X < 12)
                    {
                        projectile.velocity.X += .1f;
                    }
                }
                else if (projectile.ai[0] == 1)
                {
                    if (projectile.velocity.X > -12)
                    {
                        projectile.velocity.X -= .1f;
                    }
                    projectile.direction = projectile.spriteDirection = -1;
                }

                Projectile clearCheck = Main.projectile[(int)projectile.ai[1]];
                if (Collision.CheckAABBvAABBCollision(projectile.position, projectile.Size, clearCheck.position, clearCheck.Size))
                {
                    for (int m = 0; m < 40; m++)
                    {
                        Dust.NewDust(projectile.position, projectile.width, projectile.height, 32, 0f, 0f, 100, default, 1.6f);
                    }
                    clearCheck.Kill();
                    projectile.Kill();
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item62, (int)projectile.position.X, (int)projectile.position.Y);
        }

        public override bool PreDraw(SpriteBatch sb, Color dColor)
        {
            Rectangle frame = BaseDrawing.GetFrame(projectile.frame, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height / 6, 0, 0);

            BaseDrawing.DrawAfterimage(sb, Main.projectileTexture[projectile.type], 0, projectile, 2f, 1f, Math.Abs((int)projectile.velocity.X), true, 0f, 0f, dColor, frame, 6);

            BaseDrawing.DrawTexture(sb, Main.projectileTexture[projectile.type], 0, projectile.position, projectile.width, projectile.height, projectile.scale, projectile.rotation, 0, 6, frame, dColor, true);
            return false;
        }
    }
}