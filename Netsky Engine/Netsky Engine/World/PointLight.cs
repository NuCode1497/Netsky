using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Netsky_Engine.World
{
    public class PointLight : Light
    {
        public const int STATE_BOUNCE = 4;

        public Attenuation Attenuation;
        public override void LoadContent()
        {
            base.LoadContent();
            state = STATE_ACTIVE;
            color = Color.Red;
            position = new Vector3(0, 2, 0);
            intensity = .7f;
            radius = 10f;
            Attenuation.Constant = 0f;
            Attenuation.Linear = 1f;
            Attenuation.Quadratic = 0f;
        }
        public override void Update(GameTime gameTime)
        {
            switch(state)
            {
                case STATE_ACTIVE:
                    break;
                case STATE_BOUNCE:
                    acceleration.Y = -0.5f;
                    UpdateKinetics(gameTime);
                    BounceOffGround(1);
                    break;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Global.effect.CurrentTechnique = Global.Point;
            Global.LightPosition.SetValue(position);
            Global.LightRadius.SetValue(radius);
            Global.LightAttenuationConstant.SetValue(Attenuation.Constant);
            Global.LightAttenuationLinear.SetValue(Attenuation.Linear);
            Global.LightAttenuationQuadratic.SetValue(Attenuation.Quadratic);
        }
    }
}
