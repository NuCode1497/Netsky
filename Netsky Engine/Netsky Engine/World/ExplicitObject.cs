using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Netsky_Engine.World
{
    public abstract class ExplicitObject : DrawnObject3D
    {
        public VertexPositionTangentTexture[] vertices;
        public int[] indices;

        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        public override void LoadContent()
        {
            base.LoadContent();
            ColorMap = Global.nullDiffuse;
            NormalMap = Global.nullNormal;
            SpecularMap = Global.nullSpecular;
            vertices = new VertexPositionTangentTexture[0];
            indices = new int[0];

            Material.ambient = 0f;
            Material.diffuse = 1f;
            Material.shininess = 1f;
            Material.matteColor = Color.Blue.ToVector4();
        }
        protected override void DrawModel()
        {
            Global.World.SetValue(matrix);
            for (int j = 0; j < Global.effect.CurrentTechnique.Passes.Count; j++)
            {
                Global.effect.CurrentTechnique.Passes[j].Apply();
                Global.device.SetVertexBuffer(vertexBuffer);
                Global.device.Indices = indexBuffer;
                Global.device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, 2);
            }
        }
        protected virtual void SetupVertices()
        {
        }
        protected virtual void SetupIndices()
        {
        }
        protected void CopyToBuffers()
        {
            vertexBuffer = new VertexBuffer(Global.device, VertexPositionTangentTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
            indexBuffer = new IndexBuffer(Global.device, typeof(int), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }
    }
}
