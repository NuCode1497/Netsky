using Microsoft.Xna.Framework;

namespace Netsky_Engine.World
{
    public class LightDirectional : Light
    {
        public Vector3 direction;

        public override void LoadContent()
        {
            base.LoadContent();
            parent = Object3D.Allfather;
            color = Color.White;
            direction = new Vector3(1, -1, 0);
            direction.Normalize();
            intensity = .4f;
            castsAShadow = false;
            state = STATE_ACTIVE;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Global.effect.CurrentTechnique = Global.Directional;
            Global.LightDirection.SetValue(direction);
        }
    }
}
