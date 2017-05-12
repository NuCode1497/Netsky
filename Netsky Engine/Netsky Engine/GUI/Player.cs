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
    class Player : SpriteObject
    {
        public const int STATE_PULSATE = 4;
        public const int STATE_BOUNCE = 5;
        bool hasJumped = false;
        public override void LoadContent()
        {
            base.LoadContent();
            CollisionMethod = ObjectsCollide;
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_INACTIVE:
                    break;
                case STATE_DEATH:
                    state = STATE_INACTIVE;
                    break;
                case STATE_BIRTH:
                    state = STATE_ACTIVE;
                    break;
                case STATE_ACTIVE:
                    base.Update(gameTime);
                    UpdateKinetics(gameTime);
                    UpdateSubstance();
                    break;
                case STATE_PULSATE:
                    base.Update(gameTime);
                    UpdateKinetics(gameTime);
                    UpdateSubstance();
                    pulsate();
                    break;
                case STATE_BOUNCE:
                    base.Update(gameTime);
                    UpdateKinetics(gameTime);
                    UpdateSubstance();
                    BounceInsideParent();
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
                case STATE_BOUNCE:
                case STATE_PULSATE:
                case STATE_ACTIVE:
                    base.drawTexture();
                    //hlColor = hlOverColor;
                    //base.drawBoundingBox(Global.spriteBatch);
                    break;
            }
        }
        private void Control()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                velocity.X = -2f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                velocity.X = 2f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W) && hasJumped == false)
            {
                position.Y -= 6f;
                velocity.Y = -3f;
                hasJumped = true;
            }
            if (position.Y >= Global.windowHeight - texture.Height) hasJumped = false;
        }
    }
}
