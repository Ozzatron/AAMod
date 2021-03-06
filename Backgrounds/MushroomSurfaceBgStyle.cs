﻿using Terraria;
using Terraria.ModLoader;

namespace AAMod.Backgrounds
{
    class MushroomSurfaceBgStyle : ModSurfaceBgStyle
    {
        public override bool ChooseBgStyle()
        {
            return !Main.gameMenu && Main.LocalPlayer.GetModPlayer<AAPlayer>().ZoneMush;
        }

        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            return mod.GetBackgroundSlot("Backgrounds/MushroomBG3");
        }

        public override int ChooseMiddleTexture()
        {
            return mod.GetBackgroundSlot("Backgrounds/MushroomBG2");
        }

        public override int ChooseFarTexture()
        {
            return mod.GetBackgroundSlot("Backgrounds/MushroomBG1");
        }
    }

    public class MushroomUgBgStyle : ModUgBgStyle
    {
        public override bool ChooseBgStyle()
        {
            return !Main.gameMenu && Main.LocalPlayer.GetModPlayer<AAPlayer>().ZoneMush;
        }

        public override void FillTextureArray(int[] textureSlots)
        {
            textureSlots[0] = mod.GetBackgroundSlot("Backgrounds/MushroomUG2");
            textureSlots[1] = mod.GetBackgroundSlot("Backgrounds/MushroomUG1");
            textureSlots[2] = mod.GetBackgroundSlot("Backgrounds/MushroomUG4");
            textureSlots[3] = mod.GetBackgroundSlot("Backgrounds/MushroomUG3");
        }
    }
}
