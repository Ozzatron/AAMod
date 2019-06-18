using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using BaseMod;
using System.IO;

namespace AAMod.NPCs.Bosses.Retriever
{
    [AutoloadBossHead]
    public class Retriever : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Retriever");
            Main.npcFrameCount[npc.type] = 14;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.lifeMax = 30000;
            npc.damage = 80;
            npc.defense = 30;
            npc.buffImmune[BuffID.Ichor] = true;
            npc.value = Item.sellPrice(0, 10, 50, 0);
            npc.knockBackResist = 0f;
            npc.width = 92;
            npc.height = 54;
            npc.friendly = false;
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = new LegacySoundStyle(3, 4, Terraria.Audio.SoundType.Sound);
            npc.DeathSound = new LegacySoundStyle(4, 14, Terraria.Audio.SoundType.Sound);
            npc.netAlways = true;
            bossBag = mod.ItemType("RetrieverBag");

            music = mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/Siege");
        }

        public float[] customAI = new float[3];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if ((Main.netMode == 2 || Main.dedServ))
            {
                writer.Write(customAI[0]);
                writer.Write(customAI[1]);
                writer.Write(customAI[2]);
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
            }
        }

        public Color color;

        public override bool PreDraw(SpriteBatch spritebatch, Color dColor)
        {
            Texture2D glowTex = mod.GetTexture("Glowmasks/Retriever_Glow1");
            Texture2D glowTex1 = mod.GetTexture("Glowmasks/Retriever_Glow2");
            color = BaseUtility.MultiLerpColor((Main.player[Main.myPlayer].miscCounter % 100) / 100f, BaseDrawing.GetLightColor(npc.position), BaseDrawing.GetLightColor(npc.position), Color.Violet, BaseDrawing.GetLightColor(npc.position), Color.Violet, BaseDrawing.GetLightColor(npc.position));
            BaseDrawing.DrawTexture(spritebatch, Main.npcTexture[npc.type], 0, npc, dColor);
            BaseDrawing.DrawTexture(spritebatch, glowTex, 0, npc, color);
            BaseDrawing.DrawTexture(spritebatch, glowTex1, 0, npc, Color.White);
            return false;
        }

        public override void NPCLoot()
        {
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/RetrieverGore1"), 1f);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/RetrieverGore2"), 1f);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/RetrieverGore3"), 1f);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/RetrieverGore4"), 1f);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/RetrieverGore5"), 1f);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/RetrieverGore6"), 1f);

            if (Main.rand.Next(10) == 0)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RetrieverTrophy"));
            }
            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else
            {

                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SoulofSight, Main.rand.Next(25, 40));
                if (Main.rand.Next(10) == 0)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RetrieverMask"));
                }
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("FulguriteBar"), Main.rand.Next(30, 64));
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;   //boss drops
            AAWorld.downedRetriever = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.6f * bossLifeScale);  //boss life scale in expertmode
            npc.damage = (int)(npc.damage * 0.8f);  //boss damage increase in expermode
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
        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = (npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
        }

		public Vector2 offsetBasePoint = new Vector2(240, 0);
		
        public float moveSpeed = 10f;

        public override void AI()
        {
            Player targetPlayer = Main.player[npc.target];
            color = BaseUtility.MultiLerpColor(Main.player[Main.myPlayer].miscCounter % 100 / 100f, BaseDrawing.GetLightColor(npc.position), BaseDrawing.GetLightColor(npc.position), Color.Violet, BaseDrawing.GetLightColor(npc.position), Color.Violet, BaseDrawing.GetLightColor(npc.position));

            Lighting.AddLight((int)(npc.Center.X + (npc.width / 2)) / 16, (int)(npc.position.Y + (npc.height / 2)) / 16, color.R / 255, color.G / 255, color.B / 255);

            if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 6000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 6000f)
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 6000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 6000f)
                {
                    npc.active = false;
                    return;
                }
            }       

            if (Main.dayTime)
            {
                npc.velocity.Y -= 4;
                npc.netUpdate2 = true;
                if (npc.position.Y + npc.velocity.Y <= 0f && Main.netMode != 1) { BaseAI.KillNPC(npc); npc.netUpdate2 = true; }
                return;
            }

            bool forceChange = false;

            bool Dive1 = npc.life < npc.lifeMax * .8f;
            bool Dive2 = npc.life < npc.lifeMax * .5f;
            bool Dive3 = npc.life < npc.lifeMax * .2f;
            int DiveSpeed = Dive1 ? 14 : Dive2 ? 17 : 20;
			int ShootLaserRate = 10;
			offsetBasePoint.X = customAI[2];
			
            if (Main.netMode != 1 && npc.ai[0] != 2 && npc.ai[0] != 3)
            {
                int stopValue = 60;
                npc.ai[3]++;
                if (npc.ai[3] > stopValue) npc.ai[3] = stopValue;
                forceChange = npc.ai[3] >= stopValue;
            }
            if (npc.ai[0] == 1) //move to starting charge position
            {
                moveSpeed = 11f;
                Vector2 point = targetPlayer.Center + offsetBasePoint + new Vector2(0f, -250f);
                MoveToPoint(point);
                if (Main.netMode != 1 && (Vector2.Distance(npc.Center, point) < 10f || forceChange))
                {
                    npc.ai[0] = 2;
                    npc.ai[1] = targetPlayer.Center.X;
                    npc.ai[2] = targetPlayer.Center.Y;
                    npc.ai[3] = 0;
                    npc.netUpdate = true;
                }
                BaseAI.LookAt(targetPlayer.Center, npc, 0, 0f, 0.1f, false);
            }
            else
            if (npc.ai[0] == 2) //dive down
            {
                moveSpeed = DiveSpeed;
                Vector2 targetCenter = new Vector2(npc.ai[1], npc.ai[2]);
                Vector2 point = targetCenter - offsetBasePoint + new Vector2(0f, 250f);
                MoveToPoint(point);
                if (Main.netMode != 1 && Vector2.Distance(npc.Center, point) < 10f)
                {
                    npc.ai[0] = Dive1 ? 3 : 0;
                    npc.ai[1] = Dive1 ? targetPlayer.Center.X : 0;
                    npc.ai[2] = Dive1 ? targetPlayer.Center.Y : 0;
                    npc.ai[3] = 0;
                    npc.netUpdate = true;
                }
                BaseAI.Look(npc, 0, 0f, 0.1f, false);
            }
            else
            if (npc.ai[0] == 3) //dive up
            {
                moveSpeed = DiveSpeed;
                Vector2 targetCenter = new Vector2(npc.ai[1], npc.ai[2]);
                Vector2 point = targetCenter + offsetBasePoint + new Vector2(0f, -250f);
                MoveToPoint(point);
                if (Main.netMode != 1 && Vector2.Distance(npc.Center, point) < 10f)
                {
                    npc.ai[0] = Dive2 ? 4 : 0;
                    npc.ai[1] = Dive2 ? targetPlayer.Center.X : 0;
                    npc.ai[2] = Dive2 ? targetPlayer.Center.Y : 0;
                    npc.ai[3] = 0;
                    npc.netUpdate = true;
                }
                BaseAI.Look(npc, 0, 0f, 0.1f, false);
            }
            else
            if (npc.ai[0] == 4) //dive down
            {
                moveSpeed = DiveSpeed;
                Vector2 targetCenter = new Vector2(npc.ai[1], npc.ai[2]);
                Vector2 point = targetCenter + offsetBasePoint + new Vector2(0f, -250f);
                MoveToPoint(point);
                if (Main.netMode != 1 && Vector2.Distance(npc.Center, point) < 10f)
                {
					npc.ai[0] = Dive3 ? 5 : 0;
                    npc.ai[1] = Dive3 ? targetPlayer.Center.X : 0;
                    npc.ai[2] = Dive3 ? targetPlayer.Center.Y : 0;
                    npc.ai[3] = 0;
					npc.netUpdate2 = true;
                }
                BaseAI.Look(npc, 0, 0f, 0.1f, false);
            }
            else
            if (npc.ai[0] == 5) //dive up
            {
                moveSpeed = DiveSpeed;
                Vector2 targetCenter = new Vector2(npc.ai[1], npc.ai[2]);
                Vector2 point = targetCenter + offsetBasePoint + new Vector2(0f, -250f);
                MoveToPoint(point);
                if (Main.netMode != 1 && Vector2.Distance(npc.Center, point) < 10f)
                {
					npc.ai[0] = 0;
					npc.ai[1] = 0;
					npc.ai[2] = 0;
					npc.ai[3] = 0;
					npc.netUpdate = true;
                }
                BaseAI.Look(npc, 0, 0f, 0.1f, false);
            }else
            if (npc.ai[0] == 6) //shoot lasers right
            {
                moveSpeed = 11f;
                Vector2 point = targetPlayer.Center + offsetBasePoint + new Vector2(0f, -250f);
                MoveToPoint(point);
                BaseAI.LookAt(targetPlayer.Center, npc, 0, 0f, 0.1f, false);
                if (Main.netMode != 1)
                {
					customAI[0]++;
					if(customAI[0] > 200)
					{
						npc.ai[0] = 0;
						npc.ai[1] = 0;
						npc.ai[2] = 0;
						npc.ai[3] = 0;
						customAI[0] = 0;
						npc.netUpdate = true;						
					}
					if(Vector2.Distance(npc.Center, point) < 10f || customAI[0] > 50)
					{
						BaseAI.ShootPeriodic(npc, targetPlayer.position, targetPlayer.width, targetPlayer.height, mod.ProjectileType<RetrieverShot>(), ref customAI[1], ShootLaserRate, (int)(npc.damage * .75f), 12f, false);
					}
                }
            }else
            if (npc.ai[0] == 7) //shoot lasers left
            {
                moveSpeed = 11f;
                Vector2 point = targetPlayer.Center + offsetBasePoint + new Vector2(0f, -250f);
                MoveToPoint(point);
                BaseAI.LookAt(targetPlayer.Center, npc, 0, 0f, 0.1f, false);
                if (Main.netMode != 1)
                {
					customAI[0]++;
					if(customAI[0] > 200)
					{
						npc.ai[0] = 0;
						npc.ai[1] = 0;
						npc.ai[2] = 0;
						npc.ai[3] = 0;
						customAI[0] = 0;
						npc.netUpdate = true;						
					}	
					if(Vector2.Distance(npc.Center, point) < 10f)
					{						
						BaseAI.ShootPeriodic(npc, targetPlayer.position, targetPlayer.width, targetPlayer.height, mod.ProjectileType<RetrieverShot>(), ref customAI[1], ShootLaserRate, (int)(npc.damage * .75f), 12f, false);
					}
                }
            }				
            else //standard movement
            {
                moveSpeed = 8;
                Vector2 point = targetPlayer.Center + offsetBasePoint;
                MoveToPoint(point);
                if (Main.netMode != 1 && (Vector2.Distance(npc.Center, point) < 50f || forceChange))
                {
                    npc.ai[1]++;
                    if (npc.ai[1] > 150)
                    {
                        if (Main.rand.Next(2) == 0)
                        {
                            offsetBasePoint.X = 240;
                        }
                        else
                        {
                            offsetBasePoint.X = -240;
                        }
						customAI[2] = offsetBasePoint.X;
						if(Main.rand.Next(3) == 0) //lasers
						{
							npc.ai[0] = offsetBasePoint.X < 0 ? 7 : 6;
							npc.ai[1] = 0;
							npc.ai[2] = 0;
							npc.ai[3] = 0;
							npc.netUpdate2 = true;						
						}else
						{
							npc.ai[0] = 1;
							npc.ai[1] = 0;
							npc.ai[2] = 0;
							npc.ai[3] = 0;
							npc.netUpdate2 = true;
						}
                    }
                }
                BaseAI.LookAt(targetPlayer.Center, npc, 0, 0f, 0.1f, false);
            }
        }
		
        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[0] == 6 || npc.ai[0] == 7) //firing lasers
            {
                npc.frameCounter++;
                if (npc.frameCounter >= 4)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y > (frameHeight * 13))
                    {
                        npc.frame.Y = (frameHeight * 10);
                    }
                }				
            }
            else
            {
                npc.frameCounter++;
                if (npc.frameCounter >= 10)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                }
				if (npc.frame.Y > (frameHeight * 3))
				{
					npc.frameCounter = 0;
					npc.frame.Y = 0;
				}				
            }

        }
		

        public void FindFrameOld(int frameHeight)
        {
            if (customAI[0] <= 300)
            {
                if (customAI[0] >= 293)
                {
                    npc.frame.Y = (frameHeight * 5);
                }
                else if (customAI[0] >= 286)
                {
                    npc.frame.Y = (frameHeight * 6);
                }
                else if (customAI[0] >= 279)
                {
                    npc.frame.Y = (frameHeight * 7);
                }
                else if (customAI[0] >= 272)
                {
                    npc.frame.Y = (frameHeight * 8);
                }
                else if (customAI[0] >= 265)
                {
                    npc.frame.Y = (frameHeight * 9);
                }
                else if (customAI[0] >= 258)
                {
                    npc.frame.Y = (frameHeight * 10);
                }
                else if (customAI[0] >= 251)
                {
                    npc.frame.Y = (frameHeight * 11);
                }
                else if (customAI[0] >= 60)
                {
                    npc.frameCounter++;
                    if (npc.frameCounter >= 7)
                    {
                        npc.frameCounter = 0;
                        npc.frame.Y += frameHeight;
                    }
                    if (npc.frame.Y > (frameHeight * 13))
                    {
                        npc.frame.Y = frameHeight * 11;
                    }
                }
                else if (customAI[0] >= 59)
                {
                    npc.frame.Y = (frameHeight * 10);
                }
                else if (customAI[0] == 1)
                {
                    npc.frame.Y = (frameHeight * 7);
                }
            }
            else
            {

                npc.frameCounter++;
                if (npc.frameCounter >= 10)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y > (frameHeight * 3))
                    {
                        npc.frameCounter = 0;
                        npc.frame.Y = 0;
                    }
                }
            }

        }


        public void MoveToPoint(Vector2 point, bool goUpFirst = false)
        {
            if (moveSpeed == 0f || npc.Center == point) return; //don't move if you have no move speed
			float moveSpd = moveSpeed;			
            Vector2 dist = point - npc.Center;
            float length = (dist == Vector2.Zero ? 0f : dist.Length());
			if(length < 50f)
				moveSpd /= 2f;
            if (length < moveSpd)
            {
				moveSpd = length;
            }
            npc.velocity = (length <= 5f ? Vector2.Zero : Vector2.Normalize(dist));
            npc.velocity *= moveSpd;
        }
    }
}
