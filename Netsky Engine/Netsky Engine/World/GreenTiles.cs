using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Netsky_Engine.World
{
    public class GreenTiles : ExplicitObject
    {
        public override void LoadContent()
        {
            base.LoadContent();
            ColorMap = Global.groundDiffuse;
            NormalMap = Global.groundNormal;
            SpecularMap = Global.groundSpecular;

            state = STATE_ACTIVE;
            SetupVertices();
            SetupIndices();
            CopyToBuffers();

            Material.ambient = 0.1f;
            Material.diffuse = 0.7f;
            Material.shininess = 1.5f;
            Material.matteColor = Color.Cyan.ToVector4() / 4;
        }
        protected override void SetupVertices()
        {
            vertices = new VertexPositionTangentTexture[4];

            vertices[0].Position = new Vector3(-10f, 0f, -10f);
            vertices[0].TextureCoordinate = new Vector2(0, 0);
            vertices[0].Normal = new Vector3(0, 1, 0);
            vertices[0].Binormal = new Vector3(0, 0, -1);
            vertices[0].Tangent = new Vector3(1, 0, 0);

            vertices[1].Position = new Vector3(10f, 0f, -10f);
            vertices[1].TextureCoordinate = new Vector2(1, 0);
            vertices[1].Normal = new Vector3(0, 1, 0);
            vertices[1].Binormal = new Vector3(0, 0, -1);
            vertices[1].Tangent = new Vector3(1, 0, 0);

            vertices[2].Position = new Vector3(-10f, 0f, 10f);
            vertices[2].TextureCoordinate = new Vector2(0, 1);
            vertices[2].Normal = new Vector3(0, 1, 0);
            vertices[2].Binormal = new Vector3(0, 0, -1);
            vertices[2].Tangent = new Vector3(1, 0, 0);

            vertices[3].Position = new Vector3(10f, 0f, 10f);
            vertices[3].TextureCoordinate = new Vector2(1, 1);
            vertices[3].Normal = new Vector3(0, 1, 0);
            vertices[3].Binormal = new Vector3(0, 0, -1);
            vertices[3].Tangent = new Vector3(1, 0, 0);
        }
        protected override void SetupIndices()
        {
            indices = new int[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 3;
            indices[3] = 0;
            indices[4] = 3;
            indices[5] = 2;
        }
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
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
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            switch (state)
            {
                case STATE_ACTIVE:
                    base.Draw(gameTime);
                    break;
            }
        }
        public override Object3D Clone()
        {
            GreenTiles clone = new GreenTiles();
            clone.LoadContent();
            clone.parent = parent;
            clone.dimension = dimension;
            clone.angle = angle;
            clone.position = position;
            clone.spin = spin;
            clone.size = size;
            clone.matrix = matrix;
            clone.mass = mass;
            clone.radius = radius;
            clone.state = state;
            return clone;
        }
    }
}
