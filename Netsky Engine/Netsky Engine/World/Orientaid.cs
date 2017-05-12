using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Netsky_Engine.World
{
    public sealed class Orientaid : ListOfObject3Ds
    {
        public override World3D dimension
        {
            get
            {
                return base.dimension;
            }
            set
            {
                base.dimension = value;
                Left.dimension = value;
                Up.dimension = value;
                Forward.dimension = value;
            }
        }
        
        //MEMBERS
        public Arrow Left;
        public Arrow Up;
        public Arrow Forward;
        public float transparency;

        public override void LoadContent()
        {
            base.LoadContent();
            state = STATE_ACTIVE;

            transparency = 0.7f;

            Left = new Arrow();
            Left.LoadContent();
            Left.Material.diffuseColor = Color.Red.ToVector4();
            Left.Material.diffuseColor.W = transparency;
            Left.parent = this;
            Left.FaceRelative(Vector3.Left);
            Left.size = Vector3.One * 0.5f;

            Up = new Arrow();
            Up.LoadContent();
            Up.Material.diffuseColor = Color.Lime.ToVector4();
            Up.Material.diffuseColor.W = transparency;
            Up.parent = this;
            Up.FaceRelative(Vector3.Up);
            Up.size = Vector3.One * 0.5f;

            Forward = new Arrow();
            Forward.LoadContent();
            Forward.Material.diffuseColor = Color.Blue.ToVector4();
            Forward.Material.diffuseColor.W = transparency;
            Forward.parent = this;
            Forward.FaceRelative(Vector3.Forward);
            Forward.size = Vector3.One * 0.5f;

            Add(Left);
            Add(Up);
            Add(Forward);
        }

        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_ACTIVE:
                    UpdateKinetics(gameTime);
                    UpdateWorldMatrix();
                    base.Update(gameTime);
                    break;
            }
        }
        public override void DrawShadow(GameTime gameTime)
        {
        }
    }
}
