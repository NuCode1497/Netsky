using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Netsky_Engine
{
    //Note that the click point of the cursor is the center of its image.
    //This cursor object will be updated 1 frame behind the windows cursor.
    //For implementation of a better cursor see 
    //http://allenwp.com/blog/2011/04/04/changing-the-windows-mouse-cursor-in-xna/
    public class GameCursor : SpriteObject
    {
        public const int STATE_POINT = 4;
        public const int STATE_INTERACT = 5;
        Texture2D point;
        public Color[,] pData;
        Texture2D image;
        public Color[,] imageData;

        public override int state
        {
            get
            {
                return base.state;
            }
            set
            {
                base.state = value;
                switch (value)
                {
                    case STATE_POINT:
                        //make the cursor collidable only by 1 pixel
                        texture = point;
                        textureData = pData;
                        break;
                    case STATE_INTERACT:
                        //make the cursor collidable by its image
                        texture = image;
                        textureData = imageData;
                        break;
                }
            }
        }
        public override void LoadContent()
        {
            base.LoadContent();
            relativeLayer = 1f;
            point = CreateTexture(Color.Black);
            pData = TextureTo2DArray(point);
            image = texture;
            imageData = textureData;
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
                case STATE_POINT:
                    relativePosition.X = Global.mouseState.X;
                    relativePosition.Y = Global.mouseState.Y;
                    matrix = Matrix.CreateTranslation(relativePosition.X, relativePosition.Y, 0);
                    rectangle = new Rectangle((int)relativePosition.X, (int)relativePosition.Y, 1, 1);
                    break;
                case STATE_INTERACT:
                    relativePosition.X = Global.mouseState.X;
                    relativePosition.Y = Global.mouseState.Y;
                    UpdateSubstance();
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
                case STATE_ACTIVE:
                case STATE_POINT:
                    drawTexture(image);
                    break;
                case STATE_INTERACT:
                    drawTexture();
                    break;
            }
        }
    }
}
