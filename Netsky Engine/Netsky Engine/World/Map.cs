using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Netsky_Engine.World
{
    public class Map : Object3D
    {
        public float gridSpacing;
        public int length;
        public int width;
        public Object3D[,] tiles;
        public Object3D defaultTile;

        public override void LoadContent()
        {
            base.LoadContent();
            state = STATE_BIRTH;

            length = 10;
            width = 10;
            gridSpacing = 20;

        }
        private void reFormMap()
        {
            defaultTile = new GreenTiles();
            defaultTile.LoadContent();
            defaultTile.parent = this;
            defaultTile.dimension = dimension;

            tiles = new Object3D[length, width];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Object3D tile = defaultTile.Clone();
                    tile.position.X = (-length / 2 + i) * gridSpacing;
                    tile.position.Z = (-width/2 + j) * gridSpacing;
                    tile.angle.Y = MathHelper.ToRadians(Global.random.Next(0, 3) * 90);
                    tiles[i, j] = tile;
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_BIRTH:
                    reFormMap();
                    state = STATE_ACTIVE;
                    break;
                case STATE_ACTIVE:
                    foreach (Object3D tile in tiles)
                    {
                        tile.Update(gameTime);
                    }
                    break;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_BIRTH:
                    break;
                case STATE_ACTIVE:
                    foreach (Object3D tile in tiles)
                    {
                        tile.Draw(gameTime);
                    }
                    break;
            }
        }
    }
}
