using Microsoft.Xna.Framework;

namespace Netsky_Engine.World
{
    public class AmbientLight : Light
    {
        public override void LoadContent()
        {
            base.LoadContent();
            parent = Object3D.Allfather;
            color = Color.White;
            intensity = .3f;
            castsAShadow = false;
            specularFactor = 1f;
            specularPower = 1f;
            state = STATE_ACTIVE;
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Global.effect.CurrentTechnique = Global.Ambient;
        }
    }
}
