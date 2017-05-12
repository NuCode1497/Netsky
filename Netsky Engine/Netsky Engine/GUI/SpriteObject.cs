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
    public abstract class SpriteObject : ScreenObject
    {
        public enum TextureStyle
        {
            Fill,
            Fit,
            Stretch,
            Tile,
            Center,
        }
        static public Vector2 collisionPoint;
        public Texture2D texture = Global.placeholder;
        public Color[,] textureData;
        Vector2 _TR;
        Vector2 _BL;
        Vector2 _BR;
        public Color hlColor;
        public Color hlOverColor;
        public Color hlClickColor;
        public Color hlSetColor;
        public Texture2D hlTexture;
        public override void LoadContent()
        {
            base.LoadContent();
            textureData = TextureTo2DArray(texture);
            matrix = Matrix.Identity;
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            _TR = new Vector2(texture.Width, 0);
            _BL = new Vector2(0, texture.Height);
            _BR = new Vector2(texture.Width, texture.Height);
            hlColor = Color.Transparent;
            hlOverColor = new Color(255, 50, 50, 125);
            hlTexture = CreateTexture(Color.White);
            hlClickColor = new Color(255, 255, 55, 175);
            hlSetColor = new Color(50, 255, 50, 125);
            //radius is the distance from center to corner of texture
            radius = ((float)(Math.Sqrt(texture.Height * texture.Height + texture.Width + texture.Width) / 2));
        }
        public override void LoadCopy(ScreenObject so)
        {
            base.LoadCopy(so);
            SpriteObject spo = (SpriteObject)so;
            texture = spo.texture;
            textureData = spo.textureData;
            matrix = spo.matrix;
            origin = spo.origin;
            _TR = spo._TR;
            _BL = spo._BL;
            _BR = spo._BR;
            hlColor = spo.hlColor;
            hlOverColor = spo.hlOverColor;
            hlTexture = spo.hlTexture;
            hlClickColor = spo.hlClickColor;
            hlSetColor = spo.hlSetColor;
            radius = spo.radius;
        }
        public override void Update(GameTime gameTime)
        {
            //abstract classes must not have state switches
            //non-abstract classes must have state switches
            base.Update(gameTime);
            //use UpdateKinetics() here
            //use UpdateSubstance() here
        }
        public override void Draw(GameTime gameTime)
        {
            //abstract classes must not have state switches
            //non-abstract classes must have state switches
            //use drawTexture() here
            //use drawBoundingBox() here
        }
        public override void UpdateKinetics(GameTime gameTime)
        {
            base.UpdateKinetics(gameTime);
            Width = (int)(texture.Width * scale);
            Height = (int)(texture.Height * scale);
        }
        protected void UpdateSubstance()
        {
            //This is used for collision detection
            //Bounding Box done with matrices
            //TLmatrix is the top left corner of the texture relative to the screen
            Vector2 TL = Vector2.Transform(Vector2.Zero, matrix);
            Vector2 TR = Vector2.Transform(_TR, matrix);
            Vector2 BL = Vector2.Transform(_BL, matrix);
            Vector2 BR = Vector2.Transform(_BR, matrix);
            double m = Math.Min(TL.X, Math.Min(TR.X, Math.Min(BL.X, BR.X)));
            double n = Math.Min(TL.Y, Math.Min(TR.Y, Math.Min(BL.Y, BR.Y)));
            double o = Math.Max(TL.X, Math.Max(TR.X, Math.Max(BL.X, BR.X)));
            double p = Math.Max(TL.Y, Math.Max(TR.Y, Math.Max(BL.Y, BR.Y)));
            rectangle = new Rectangle((int)(m), (int)(n), (int)(o - m), (int)(p - n));
            //rectangle = new Rectangle((int)(TL.X), (int)(TL.Y), (int)(texture.Width * screenScale), (int)(texture.Height * screenScale));
            #region notes
            ////Unmodified bounding box for reference
            //rectangle = new Rectangle((int)(screenPosition.X - origin.X * screenScale),
            //                          (int)(screenPosition.Y - origin.Y * screenScale),
            //                          (int)(texture.Width * screenScale), (int)(texture.Height * screenScale));

            ////Bounding Box done without matrices for reference
            //double a = (origin.X * screenScale) * Math.Cos(-angle);
            //double b = (origin.Y * screenScale) * Math.Sin(-angle);
            //double c = (origin.X * screenScale) * Math.Sin(-angle);
            //double d = (origin.Y * screenScale) * Math.Cos(-angle);
            //double x1 = screenPosition.X - a - b;
            //double y1 = screenPosition.Y + c - d;
            //double x2 = screenPosition.X + a - b;
            //double y2 = screenPosition.Y - c - d;
            //double x3 = screenPosition.X - a + b;
            //double y3 = screenPosition.Y + c + d;
            //double x4 = screenPosition.X + a + b;
            //double y4 = screenPosition.Y - c + d;
            //double e = Math.Min(x1, Math.Min(x2, Math.Min(x3, x4)));
            //double f = Math.Min(y1, Math.Min(y2, Math.Min(y3, y4)));
            //double g = Math.Max(x1, Math.Max(x2, Math.Max(x3, x4)));
            //double h = Math.Max(y1, Math.Max(y2, Math.Max(y3, y4)));
            //rectangle = new Rectangle((int)(e), (int)(f), (int)(g - e), (int)(h - f));
            #endregion
        }
        protected void drawTexture()
        {
            //display the object
            Global.spriteBatch.Draw(texture, relativePosition, null, color, relativeAngle, origin, relativeScale, spriteEffects, relativeLayer);
        }
        protected void drawTexture(Texture2D t)
        {
            //display the object
            Global.spriteBatch.Draw(t, relativePosition, null, color, relativeAngle, origin, relativeScale, spriteEffects, relativeLayer);
        }
        protected void drawTextureStretch()
        {
            //display the object
            Global.spriteBatch.Draw(texture, rectangle, null, color, 0, Vector2.Zero, spriteEffects, relativeLayer);
        }
        public void drawBoundingBox()
        {
            //display the bounding box
            Global.spriteBatch.Draw(hlTexture, rectangle, null, hlColor, 0, Vector2.Zero, spriteEffects, relativeLayer - .0001f);
        }
        public override void drawHighlight(GameTime gameTime)
        {
            hlColor = hlOverColor;
            drawBoundingBox();
        }
        public override void drawPoke(GameTime gameTime)
        {
            hlColor = hlClickColor;
            drawBoundingBox();
        }
        public override void drawSelection(GameTime gameTime)
        {
            hlColor = hlSetColor;
            drawBoundingBox();
        }
        //Manual Texture
        //http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2D/Manual_texture_creation.php
        static public Texture2D CreateTexture(Color c)
        {
            Color[] textureColors = { c };
            Texture2D texture = new Texture2D(Global.currentGame.GraphicsDevice, 1, 1);
            texture.SetData(textureColors);
            return texture;
        }
        static public Texture2D CreateTexture(int width, int height, Color c)
        {
            Color[] textureColors = new Color[width * height];
            Texture2D texture = new Texture2D(Global.currentGame.GraphicsDevice, width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    textureColors[i + j * width] = c;
                }
            }
            texture.SetData(textureColors);
            return texture;
        }
        static public Texture2D CreateTexture(int width, int height, Texture2D texture, TextureStyle style)
        {
            //get the data of the base texture
            Color[,] InTextureColors = TextureTo2DArray(texture);
            //prepare the new texture
            Color[,] OutTextureColors = new Color[width, height];
            Texture2D outTexture = new Texture2D(Global.currentGame.GraphicsDevice, width, height);
            switch (style)
            {
                case TextureStyle.Center:
                    //align the centers of each texture
                    int nx = width / 2 - texture.Width / 2;
                    int ny = height / 2 - texture.Height / 2;
                    int nyreset = ny;
                    int bx = 0;
                    int by = 0;
                    int w = texture.Width;
                    int h = texture.Height;
                    //check if base texture is larger than new texture
                    if (nx < 0)
                    {
                        bx = 0 - nx;
                        nx = 0;
                        w = width;
                    }
                    if (ny < 0)
                    {
                        by = 0 - ny;
                        ny = 0;
                        nyreset = ny;
                        h = height;
                    }
                    //go through each point in the base texture
                    int k = nx;
                    for (int i = bx; i < bx + w; i++)
                    {
                        int l = ny;
                        for (int j = by; j < by + h; j++)
                        {
                            //skip over points that are outside the new texture
                            //transcribe the base texture to the new texture
                            OutTextureColors[k,l] = InTextureColors[i,j];
                            l++;
                        }
                        k++;
                    }
                    break;
                case TextureStyle.Fill:
                    //to do
                case TextureStyle.Fit:
                    //to do
                case TextureStyle.Stretch:
                    //to do
                case TextureStyle.Tile:
                    int tilex = 0;
                    int tiley = 0;
                    for (int i = 0; i < width; i++)
                    {
                        if (tilex >= texture.Width) tilex = 0;
                        tiley = 0;
                        for (int j = 0; j < height; j++)
                        {
                            if (tiley >= texture.Height) tiley = 0;
                            OutTextureColors[i, j] = InTextureColors[tilex, tiley];
                            tiley++;
                        }
                        tilex++;
                    }
                    break;
            }
            Color[] textureColors = new Color[width * height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    textureColors[i + j * width] = OutTextureColors[i, j];
                }
            }
            outTexture.SetData(textureColors);
            return outTexture;
        }
        public override void BounceInsideParent()
        {
            hitWall = false;
            if (position.X > parent.Width - texture.Width * scale / 2)
            {
                position.X = parent.Width - texture.Width * scale / 2;
                velocity.X = Math.Abs(velocity.X) * -1;
                hitWall = true;
            }
            if (position.X < 0 + texture.Width * scale / 2)
            {
                position.X = 0 + texture.Width * scale / 2;
                velocity.X = Math.Abs(velocity.X);
                hitWall = true;
            }

            if (position.Y > parent.Height - texture.Height * scale / 2)
            {
                position.Y = parent.Height - texture.Height * scale / 2;
                velocity.Y = Math.Abs(velocity.Y) * -1;
                hitWall = true;
            }
            if (position.Y < 0 + texture.Height * scale / 2)
            {
                position.Y = 0 + texture.Height * scale / 2;
                velocity.Y = Math.Abs(velocity.Y);
                hitWall = true;
            }
        }
        //1D array texture data to 2D array
        //http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2D/Putting_CD_into_practice.php
        static protected Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }
        //per pixel collision detection
        //reports the coordinate where two textures are to draw to the same screen pixel if both are non-transparent
        //http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2D/Putting_CD_into_practice.php
        static public Vector2 TexturesCollide(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2)
        {
            Matrix mat1to2 = mat1 * Matrix.Invert(mat2);
            int width1 = tex1.GetLength(0);
            int height1 = tex1.GetLength(1);
            int width2 = tex2.GetLength(0);
            int height2 = tex2.GetLength(1);

            for (int x1 = 0; x1 < width1; x1++)
            {
                for (int y1 = 0; y1 < height1; y1++)
                {
                    Vector2 pos1 = new Vector2(x1, y1);
                    Vector2 pos2 = Vector2.Transform(pos1, mat1to2);

                    int x2 = (int)pos2.X;
                    int y2 = (int)pos2.Y;
                    if ((x2 >= 0) && (x2 < width2))
                    {
                        if ((y2 >= 0) && (y2 < height2))
                        {
                            if (tex1[x1, y1].A > 20)
                            {
                                if (tex2[x2, y2].A > 20)
                                {
                                    Vector2 screenPos = Vector2.Transform(pos1, mat1);
                                    return screenPos;
                                }
                            }
                        }
                    }
                }
            }

            return new Vector2(-1, -1);
        }
        static public void ObjectsCollide(SpriteObject ob1, SpriteObject ob2)
        {
            //use bool collision to determine if there was a collision
            //use collisionPoint to find the point of collision
            ObjectsCollideByRectangle(ob1, ob2);
            if (collision) collisionPoint = TexturesCollide(ob1.textureData, ob1.matrix, ob2.textureData, ob2.matrix);
            if (collisionPoint.X == -1) collision = false;
        }
        static public void ObjectsCollide(ScreenObject so1, ScreenObject so2)
        {
            if (so1 is SpriteObject && so2 is SpriteObject)
            {
                ObjectsCollide((SpriteObject)so1, (SpriteObject)so2);
            }
            else
            {
                ObjectsCollideByRectangle(so1, so2);
            }
        }
    }
}
