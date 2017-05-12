using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Netsky_Engine.World
{
    public sealed class GenericModelObject : ModelObject
    {
        public override void LoadContent()
        {
            base.LoadContent();
            state = STATE_ACTIVE;
        }
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            switch (state)
            {
                case STATE_ACTIVE:
                    UpdateKinetics(gameTime);
                    UpdateWorldMatrix();
                    base.Update(gameTime);
                    break;
            }
        }
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            switch (state)
            {
                case STATE_ACTIVE:
                    base.Draw(gameTime);
                    break;
            }
        }
    }
}
