using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Netsky_Engine.World
{
    public sealed class Arrow : DirectedObject
    {
        public Material Material;

        public GenericModelObject head;
        public GenericModelObject shaft;
        private const float headHeight = 0.4f;

        public Color color
        {
            get
            {
                return new Color(Material.diffuseColor);
            }
            set
            {
                Material.diffuseColor = value.ToVector4();
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            Face(Vector3.One);
            state = STATE_ACTIVE;

            shaft = new GenericModelObject();
            shaft.LoadContent();
            shaft.model = Global.arrowShaft;
            shaft.parent = this;

            head = new GenericModelObject();
            head.LoadContent();
            head.model = Global.arrowHead;
            head.parent = this;

            Material.ambient = 0.2f;
            Material.diffuseColor = Color.Blue.ToVector4();
            Material.diffuse = 1f;
            Material.shininess = 1f;
            Material.matteColor = Color.Cyan.ToVector4();
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_DEATH:
                case STATE_INACTIVE:
                case STATE_BIRTH:
                    break;
                case STATE_FACE_REL:
                case STATE_TRACK_REL:
                    base.Update(gameTime);
                    UpdateRelativeComponents();
                    break;
                default:
                    base.Update(gameTime);
                    UpdateComponents();
                    break;
            }
        }
        private void UpdateComponents()
        {
            head.position.Z = -viewVector.Length();
            shaft.size = scale;
            shaft.size.Z = viewVector.Length() - headHeight * scale.Z;
            head.matrix = Matrix.CreateScale(scale) *
                          //no local rotation
                          Matrix.CreateTranslation(head.position) *
                          //no host scaling
                          Matrix.CreateFromQuaternion(rotation) *
                          Matrix.CreateTranslation(translation);
            shaft.matrix = Matrix.CreateScale(shaft.size) *
                           //no local rotation
                           //no local translation
                           //no host scaling
                           Matrix.CreateFromQuaternion(rotation) *
                           Matrix.CreateTranslation(translation);
        }
        private void UpdateRelativeComponents()
        {
            head.position.Z = -viewVector.Length();
            shaft.size = size;
            shaft.size.Z = viewVector.Length() - headHeight * size.Z;
            head.matrix = Matrix.CreateScale(size) *
                          //no local rotation
                          Matrix.CreateTranslation(head.position) *
                          Matrix.CreateScale(scale) *
                          Matrix.CreateFromQuaternion(rotation) *
                          Matrix.CreateTranslation(translation);
            shaft.matrix = Matrix.CreateScale(shaft.size) *
                           //no local rotation
                           //no local translation
                           Matrix.CreateScale(scale) *
                           Matrix.CreateFromQuaternion(rotation) *
                           Matrix.CreateTranslation(translation);
        }
        public override void Draw(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_INACTIVE:
                case STATE_DEATH:
                case STATE_BIRTH:
                    break;
                default:
                    head.Material = Material;
                    shaft.Material = Material;
                    head.Draw(gameTime);
                    shaft.Draw(gameTime);
                    break;
            }
        }
    }
}
