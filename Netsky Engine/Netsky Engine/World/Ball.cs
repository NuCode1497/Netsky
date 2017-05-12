using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Netsky_Engine.World
{
    public class Ball : ModelObject
    {
        public const int STATE_BOUNCE = 4;
        public const int STATE_WALLHACK = 5;
        public const int STATE_GLOW = 6;
        public const int STATE_GLOW_BOUNCE = 7;

        public PointLight Glow;

        public override void LoadContent()
        {
            base.LoadContent();
            state = STATE_ACTIVE;
            Material.diffuseColor = Color.Blue.ToVector4();
            Material.matteColor = Color.Blue.ToVector4();
            Material.shininess = 5f;
            model = Global.sphereModel;
            radius = 1;

            Glow = new PointLight();
            Glow.LoadContent();
            Glow.intensity = 1.5f;
            Glow.state = PointLight.STATE_ACTIVE;
            Glow.color = Color.Blue;
            Glow.parent = this;
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_GLOW:
                    UpdateKinetics(gameTime);
                    BounceOffGround(radius);
                    UpdateWorldMatrix();
                    Glow.position = position;
                    base.Update(gameTime);
                    break;
                case STATE_ACTIVE:
                    UpdateKinetics(gameTime);
                    BounceOffGround(radius);
                    UpdateWorldMatrix();
                    base.Update(gameTime);
                    break;
                case STATE_GLOW_BOUNCE:
                    acceleration.Y = dimension.gravity;
                    UpdateKinetics(gameTime);
                    BounceOffGround(radius);
                    UpdateWorldMatrix();
                    Glow.position = position;
                    base.Update(gameTime);
                    break;
                case STATE_BOUNCE:
                    acceleration.Y = dimension.gravity;
                    UpdateKinetics(gameTime);
                    BounceOffGround(radius);
                    UpdateWorldMatrix();
                    base.Update(gameTime);
                    break;
                case STATE_WALLHACK:
                    UpdateKinetics(gameTime);
                    UpdateWorldMatrix();
                    base.Update(gameTime);
                    break;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_INACTIVE:
                case STATE_DEATH:
                case STATE_BIRTH:
                    break;
                case STATE_WALLHACK:
                    DrawOverWorld(gameTime);
                    break;
                default:
                    base.Draw(gameTime);
                    break;
            }
        }
        private void DrawOverWorld(GameTime gameTime)
        {
            DepthStencilState dssOLD = Global.device.DepthStencilState;
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            Global.device.DepthStencilState = dss;
            base.Draw(gameTime);
            Global.device.DepthStencilState = dssOLD;
        }
    }
}
