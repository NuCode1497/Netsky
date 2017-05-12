using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Netsky_Engine.World
{
    public sealed class Orbiter : ModelObject
    {
        public const int STATE_ORBIT = 4;

        public float distance;
        public Object3D orbitObject;
        public bool clockwise;
        public float rate;

        public override void LoadContent()
        {
            base.LoadContent();
            model = Global.ship2Model;
            ColorMap = Global.ship2Diffuse;
            SpecularMap = Global.ship2Specular;
            NormalMap = Global.ship2Normal;
            size = Vector3.One * 0.5f;

            distance = 5;
            clockwise = false;
            rate = 1f;
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_ORBIT:
                    UpdateKinetics(gameTime);
                    Rotate(gameTime);
                    UpdateWorldMatrix();
                    base.Update(gameTime);
                    break;
                default:
                    base.Update(gameTime);
                    break;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        private void Rotate(GameTime gameTime)
        {
            float time = gameTime.ElapsedGameTime.Milliseconds / 512f;
            float yaw = 0;
            float roll = 0;
            if (clockwise)
            {
                angle.Y -= time * rate;
                yaw = angle.Y - MathHelper.Pi + MathHelper.PiOver4;
                roll = angle.Z - MathHelper.Pi + MathHelper.PiOver4;
                position = Vector3.Transform(new Vector3(distance, 0, distance),
                    Quaternion.CreateFromAxisAngle(Vector3.Up, yaw));
            }
            else
            {
                angle.Y += time * rate;
                yaw = angle.Y + MathHelper.PiOver4;
                position = Vector3.Transform(new Vector3(distance, 0, distance), 
                    Quaternion.CreateFromAxisAngle(Vector3.Up, yaw));
            }
            spin = Quaternion.CreateFromAxisAngle(Vector3.Forward, angle.Z) *
                        Quaternion.CreateFromAxisAngle(Vector3.Up, angle.Y) *
                        Quaternion.CreateFromAxisAngle(Vector3.Right, angle.X);
        }
        public void Orbit(Object3D o, float distance, bool clockwise, float rate)
        {
            state = STATE_ORBIT;
            parent = o;
            this.distance = distance;
            this.clockwise = clockwise;
            this.rate = rate;
        }
    }
}
