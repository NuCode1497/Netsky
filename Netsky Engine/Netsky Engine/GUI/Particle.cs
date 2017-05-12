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
    public class Particle : SpriteObject
    {
        public const int STATE_FADE = 4;
        public Vector2 originalPosition;
        public float birthTime;
        public float maxAge;
        public override void LoadContent()
        {
            base.LoadContent();
            position.X = 0;
            position.Y = 0;
            mass = 0;
            scale = 1f;
            layer = 0.01f;
            friction = 1f;
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_INACTIVE:
                    break;
                case STATE_DEATH:
                    state = STATE_INACTIVE;
                    Death(this);
                    break;
                case STATE_BIRTH:
                    state = STATE_ACTIVE;
                    break;
                case STATE_ACTIVE:
                    base.Update(gameTime);
                    UpdateKinetics(gameTime);
                    BounceInsideParent();
                    if (hitWall)
                    {
                        state = STATE_DEATH;
                    }
                    break;
                case STATE_FADE:
                    float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
                    float timeAlive = now - birthTime;
                    if (timeAlive > maxAge)
                    {
                        state = STATE_DEATH;
                    }
                    else
                    {
                        //get remaining lifetime
                        float relAge = timeAlive / maxAge;
                        //update position
                        UpdateKinetics(gameTime);
                        Vector2 travelDist = position - originalPosition;
                        if (hitWall)
                        {
                            state = STATE_DEATH;
                        }
                        //fade out color
                        float invAge = 1.0f - relAge;
                        color = new Color(new Vector4(invAge, invAge, invAge, invAge));
                        
                    }
                    break;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_INACTIVE:
                    break;
                case STATE_DEATH:
                    break;
                case STATE_BIRTH:
                    break;
                case STATE_FADE:
                case STATE_ACTIVE:
                    base.Draw(gameTime);
                    drawTexture();
                    break;
            }
        }
    }
}
