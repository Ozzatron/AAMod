using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace AAMod.Tiles.Furniture.Doom
{
    public class DoomChandelier : ModTile
	{
		public override void SetDefaults()
		{

            //Main.tileFlame[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.StyleWrapLimit = 37;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Doom Chandelier");
            AddMapEntry(new Color(200, 0, 0), name);
            dustType = mod.DustType("DoomDust");
            adjTiles = new int[] { TileID.Chandeliers };
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        }
        public override void HitWire(int i, int j)
        {
            int left = i - (Main.tile[i, j].frameX / 18) % 3;
            int top = j - (Main.tile[i, j].frameY / 18) % 3;
            for (int x = left; x < left + 3; x++)
            {
                for (int y = top; y < top + 3; y++)
                {

                    if (Main.tile[x, y].frameX >= 54)
                    {
                        Main.tile[x, y].frameX -= 54;
                    }
                    else
                    {
                        Main.tile[x, y].frameX += 54;
                    }
                }
            }
            if (Wiring.running)
            {
                Wiring.SkipWire(left, top);
                Wiring.SkipWire(left, top + 1);
                Wiring.SkipWire(left + 1, top);
                Wiring.SkipWire(left + 1, top + 1);
            }
            NetMessage.SendTileSquare(-1, left, top + 1, 2);

        }

        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameX < 36)
            {
                r = 0.9f;
                g = 0.9f;
                b = 0.9f;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 48, 32, mod.ItemType("DoomChandelier"));
			Chest.DestroyChest(i, j);
		}
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Color color = new Color(100, 100, 100, 0);
            int frameX = Main.tile[i, j].frameX;
            int frameY = Main.tile[i, j].frameY;
            int width = 20;
            int offsetY = 2;
            int height = 20;
            int offsetX = 2;
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            for (int k = 0; k < 7; k++)
            {
                Main.spriteBatch.Draw(mod.GetTexture("Glowmasks/DoomChandelier_Glow"), new Vector2(i * 16 - (int)Main.screenPosition.X + offsetX - (width - 16f) / 2f, j * 16 - (int)Main.screenPosition.Y + offsetY) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
