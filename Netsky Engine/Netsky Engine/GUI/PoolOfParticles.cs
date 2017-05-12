using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Netsky_Engine
{
    public class PoolOfParticles : PoolOfScreenObjects
    {
        public Texture2D texture;
        public Particle commonParticle;

        public override void LoadContent()
        {
            //instead of creating thousands of new settings, use common settings
            commonParticle = new Particle();
            commonParticle.texture = texture;
            commonParticle.LoadContent();
            base.LoadContent();
            for (int i = 0; i < numberOfObjects; i++)
            {
                Particle particle = new Particle();
                particle.LoadCopy(commonParticle);
                Add(particle);
            }
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_INACTIVE:
                    break;
                case STATE_DEATH:
                    base.Clear();
                    state = STATE_INACTIVE;
                    break;
                case STATE_BIRTH:
                    break;
                case STATE_ACTIVE:
                    base.Update(gameTime);
                    break;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_ACTIVE:
                    base.Draw(gameTime);
                    break;
            }
        }
        public void renderExplosion(ScreenObject parent, Vector2 pos, int numParticles, float size, float maxAge, GameTime gameTime)
        {
                //stopwatch.Reset();  //++++++++++ Test 1 ++++++++++++++++++
                //stopwatch.Start();  //++++++++++ Test 1 ++++++++++++++++++
            for (int i = 0; i < numParticles; i++)
            {
                Particle p = (Particle)GetNextObject();
                p.parent = parent;
                p.position = pos;
                p.birthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                p.maxAge = maxAge;
                p.scale = size * .25f;
                p.color = Color.White;
                p.state = Particle.STATE_FADE;

                float particleDistance = (float)Global.random.NextDouble() * size;
                Vector2 displacement = new Vector2(particleDistance, 0);
                float ang = MathHelper.ToRadians(Global.random.Next(360));
                float ang2 = MathHelper.ToRadians(Global.random.Next(360));
                float ang3 = (float)Global.random.NextDouble() * .01f;
                float ang3d = Global.random.Next(-1, 2);
                displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(ang2));
                p.angularVelocity = ang3d * ang3;
                p.angle = ang;
                p.velocity = displacement;
                p.friction = 0.97f;
            }
                //stopwatch.Stop();   //---------- Test 1 ------------------
                //test1 = stopwatch.Elapsed;  //-- Test 1 ------------------
        }
    }
}
