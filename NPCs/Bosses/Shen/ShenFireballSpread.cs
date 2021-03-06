using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AAMod.NPCs.Bosses.Shen
{
    public class ShenFireballSpread : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            Main.projFrames[projectile.type] = 4;
        }

        public override void PostAI()
        {
            if (projectile.frameCounter++ > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame > 3)
                {
                    projectile.frame = 0;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.hostile = true;
            projectile.timeLeft = 240;
            projectile.aiStyle = -1;
            cooldownSlot = 1;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<Dusts.Discord>(), 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
        }
        public override void AI()
        {
            if (--projectile.ai[0] == 0)
            {
                projectile.netUpdate = true;
                projectile.velocity = Vector2.Zero;
            }
            if (--projectile.ai[1] == 0)
            {
                projectile.netUpdate = true;
                Player target = Main.player[Player.FindClosest(projectile.position, projectile.width, projectile.height)];
                projectile.velocity = projectile.DirectionTo(target.Center + target.velocity * 30) * 30;
            }
        }
    }
}