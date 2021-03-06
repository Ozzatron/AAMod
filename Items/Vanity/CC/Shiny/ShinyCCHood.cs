using System.Collections.Generic;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace AAMod.Items.Vanity.CC.Shiny
{
    [AutoloadEquip(EquipType.Head)]
	public class ShinyCCHood : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Draconian Cultist Mask");
			Tooltip.SetDefault(@"The mask of a crazy dragon enthusiast
'Great for impersonating Ancients Awakened Developers!'");
		}

		public override void SetDefaults() 
		{
			item.width = 18;
			item.height = 18;
			item.value = 10000;
			item.rare = 2;
			item.vanity = true;
		}

		public override void ModifyTooltips(List<TooltipLine> list)
		{
			foreach (TooltipLine line2 in list)
			{
				if (line2.mod == "Terraria" && line2.Name == "ItemName")
				{
					line2.overrideColor = new Color(92, 101, 150);
				}
			}
		}
	}
}