using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Netsky_Engine
{
    public class Crosshairs : SpriteObject
    {
        public override void LoadContent()
        {

            texture = Global.crosshairs;
        }
        public override void Update(GameTime gameTime)
        {
        }
        public override void Draw(GameTime gameTime)
        {
            Global.spriteBatch.Draw(texture,
                new Vector2((Global.windowWidth / 2) - (texture.Width / 2),
                    (Global.windowHeight / 2) - (texture.Height / 2)), Color.White);
        }
    }
}
