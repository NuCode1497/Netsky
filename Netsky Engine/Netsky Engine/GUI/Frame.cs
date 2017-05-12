using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Netsky_Engine
{
    public class Frame : SpriteObject
    {
        public const int STATE_CLICKTHROUGH = 4;
        public const int STATE_SHOWBORDERS = 5;
        public Color background;
        private SpriteObject borderTop;
        private SpriteObject borderLeft;
        private SpriteObject borderRight;
        private SpriteObject borderBottom;
        Color borderColor;
        public override void LoadContent()
        {
            base.LoadContent();
            background = Color.Black;
            borderColor = Color.White;
            borderTop = new Block();
            borderLeft = new Block();
            borderRight = new Block();
            borderBottom = new Block();
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_INACTIVE:
                    break;
                case STATE_DEATH:
                    state = STATE_INACTIVE;
                    break;
                case STATE_BIRTH:
                    state = STATE_ACTIVE;
                    break;
                case STATE_ACTIVE:
                    UpdateKinetics(gameTime);
                    UpdateSubstance();
                    base.Update(gameTime);
                    break;
                case STATE_CLICKTHROUGH:
                    UpdateKinetics(gameTime);
                    base.Update(gameTime);
                    break;
                case STATE_SHOWBORDERS:
                    UpdateKinetics(gameTime);
                    UpdateSubstance();
                    base.Update(gameTime);
                    UpdateBorders(gameTime);
                    break;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_INACTIVE:
                    break;
                case STATE_DEATH:
                    break;
                case STATE_BIRTH:
                    break;
                case STATE_ACTIVE:
                    drawTexture();
                    break;
                case STATE_CLICKTHROUGH:
                    drawTexture();
                    //drawBoundingBox(Global.spriteBatch);
                    break;
                case STATE_SHOWBORDERS:
                    drawTexture();
                    DrawBorders(gameTime);
                    break;
            }
        }
        private void CreateForeground()
        {
            Color[] foregroundColors = new Color[Global.screenWidth * Global.screenHeight];

            for (int x = 0; x < Global.screenWidth; x++)
            {
                for (int y = 0; y < Global.screenHeight; y++)
                {
                    foregroundColors[x + y * Global.screenWidth] = Color.Green;
                }
            }

            texture.SetData(foregroundColors);
        }
        private void CreateForeground(int x, int y, Color c)
        {
            Color[] foregroundColors = new Color[x * y];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    foregroundColors[i + j * x] = c;
                }
            }

            texture.SetData(foregroundColors);
            textureData = TextureTo2DArray(texture);
        }
        public void setBorders(int width, int inset, Texture2D texture, Color c)
        {
            //good inset values are between 0 and -width
            borderTop = new Block();
            borderTop.texture = SpriteObject.CreateTexture(
                (int)(this.texture.Width - inset * 2),
                (int)width, texture, TextureStyle.Tile);
            borderTop.LoadContent();
            borderTop.origin = Vector2.Zero;
            borderTop.parent = this;
            borderTop.position.X = 0 + inset;
            borderTop.position.Y = 0 + inset;
            borderTop.state = Block.STATE_ACTIVE;
            borderTop.layer = .1f;
            borderTop.color = c;
            borderBottom = new Block();
            borderBottom.texture = SpriteObject.CreateTexture(
                (int)(this.texture.Width - inset * 2),
                (int)width, texture, TextureStyle.Tile);
            borderBottom.LoadContent();
            borderBottom.origin = Vector2.Zero;
            borderBottom.parent = this;
            borderBottom.position.X = 0 + inset;
            borderBottom.position.Y = this.texture.Height - width - inset;
            borderBottom.state = Block.STATE_ACTIVE;
            borderBottom.layer = .1f;
            borderBottom.color = c;
            borderLeft = new Block();
            borderLeft.texture = SpriteObject.CreateTexture(
                (int)width,
                (int)(this.texture.Height - inset * 2), texture, TextureStyle.Tile);
            borderLeft.LoadContent();
            borderLeft.origin = Vector2.Zero;
            borderLeft.parent = this;
            borderLeft.position.X = 0 + inset;
            borderLeft.position.Y = 0 + inset;
            borderLeft.state = Block.STATE_ACTIVE;
            borderLeft.layer = .1f;
            borderLeft.color = c;
            borderRight = new Block();
            borderRight.texture = SpriteObject.CreateTexture(
                (int)width,
                (int)(this.texture.Height - inset * 2), texture, TextureStyle.Tile);
            borderRight.LoadContent();
            borderRight.origin = Vector2.Zero;
            borderRight.parent = this;
            borderRight.position.X = this.texture.Width - width - inset;
            borderRight.position.Y = 0 + inset;
            borderRight.state = Block.STATE_ACTIVE;
            borderRight.layer = .1f;
            borderRight.color = c;
        }
        void UpdateBorders(GameTime gameTime)
        {
            borderTop.UpdateKinetics(gameTime);
            borderTop.Update(gameTime);
            borderLeft.UpdateKinetics(gameTime);
            borderLeft.Update(gameTime);
            borderRight.UpdateKinetics(gameTime);
            borderRight.Update(gameTime);
            borderBottom.UpdateKinetics(gameTime);
            borderBottom.Update(gameTime);
        }
        void DrawBorders(GameTime gameTime)
        {
            borderTop.Draw(gameTime);
            borderLeft.Draw(gameTime);
            borderRight.Draw(gameTime);
            borderBottom.Draw(gameTime);
        }
    }
}
