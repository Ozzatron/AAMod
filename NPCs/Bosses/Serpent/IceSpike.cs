using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace AAMod.NPCs.Bosses.Serpent
{
    public class IceSpike : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Blizzard);
            projectile.hostile = true;
            projectile.friendly = false;
        }

		public override void SetStaticDefaults()
		{
		    DisplayName.SetDefault("Ice Spike");
            Main.projFrames[projectile.type] = 5;
		}
    }
}
