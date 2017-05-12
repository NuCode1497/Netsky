// (c) Copyright 2010 Dr. Thomas Fernandez
//          All rights reserved.

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
    //should make this abstract
    //remove states and create subclasses
    public class TextObject : ScreenObject
    {
        public const int STATE_ALIGN_RIGHT = 4;
        public const int STATE_SHADOW = 5;
        public const int STATE_OUTLINE = 6;
        public string text;
        public SpriteFont font;

        public override void LoadContent()
        {
            font = Global.Consolas18;
            base.LoadContent();
            layer = .9f;
            text = "";
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
                    UpdateKinetics(gameTime);
                    break;
                case STATE_OUTLINE:
                case STATE_SHADOW:
                case STATE_ACTIVE:
                    base.Update(gameTime);
                    UpdateKinetics(gameTime);
                    break;
                case STATE_ALIGN_RIGHT:
                    base.Update(gameTime);
                    UpdateKinetics(gameTime);
                    alignRight();
                    break;
            }
        }
        private void alignRight()
        {
            origin.X = font.MeasureString(text).X;
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
                case STATE_ALIGN_RIGHT:
                case STATE_ACTIVE:
                    Global.spriteBatch.DrawString(font, text, relativePosition, 
                        color, relativeAngle, origin, relativeScale, spriteEffects, relativeLayer);
                    break;
                case STATE_SHADOW:
                    Global.spriteBatch.DrawString(font, text, relativePosition + Vector2.One,
                        Color.Black, relativeAngle, origin, relativeScale, spriteEffects, relativeLayer - 0.01f);
                    Global.spriteBatch.DrawString(font, text, relativePosition,
                        color, relativeAngle, origin, relativeScale, spriteEffects, relativeLayer);
                    break;
                case STATE_OUTLINE:
                    Global.spriteBatch.DrawString(font, text, relativePosition + Vector2.UnitX,
                        Color.Black, relativeAngle, origin, relativeScale, spriteEffects, relativeLayer - 0.01f);
                    Global.spriteBatch.DrawString(font, text, relativePosition - Vector2.UnitX,
                        Color.Black, relativeAngle, origin, relativeScale, spriteEffects, relativeLayer - 0.01f);
                    Global.spriteBatch.DrawString(font, text, relativePosition - Vector2.UnitY,
                        Color.Black, relativeAngle, origin, relativeScale, spriteEffects, relativeLayer - 0.01f);
                    Global.spriteBatch.DrawString(font, text, relativePosition + Vector2.UnitY,
                        Color.Black, relativeAngle, origin, relativeScale, spriteEffects, relativeLayer - 0.01f);
                    Global.spriteBatch.DrawString(font, text, relativePosition,
                        color, relativeAngle, origin, relativeScale, spriteEffects, relativeLayer);
                    break;
            }
        }

    }
}
