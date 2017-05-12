using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Netsky_Engine.World
{
    public class Frustum : Object3D
    {
        public BoundingFrustum boundingFrustum;
        private IndexBuffer indexBuffer1;
        private IndexBuffer indexBuffer2;
        private int[] indices1;
        private int[] indices2;
        public VertexBuffer vertexBuffer;
        public VertexPositionTangentTexture[] vertices;
        public override void LoadContent()
        {
            boundingFrustum = new BoundingFrustum(Matrix.Identity);
            SetUpVertices();
            SetupIndices();
            CopyToBuffers();
        }
        private void SetUpVertices()
        {
            vertices = new VertexPositionTangentTexture[8];

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

            vertices[4].Position = new Vector3(-10f, 0f, -10f);
            vertices[4].TextureCoordinate = new Vector2(0, 0);
            vertices[4].Normal = new Vector3(0, 1, 0);
            vertices[4].Binormal = new Vector3(0, 0, -1);
            vertices[4].Tangent = new Vector3(1, 0, 0);

            vertices[5].Position = new Vector3(10f, 0f, -10f);
            vertices[5].TextureCoordinate = new Vector2(1, 0);
            vertices[5].Normal = new Vector3(0, 1, 0);
            vertices[5].Binormal = new Vector3(0, 0, -1);
            vertices[5].Tangent = new Vector3(1, 0, 0);

            vertices[6].Position = new Vector3(-10f, 0f, 10f);
            vertices[6].TextureCoordinate = new Vector2(0, 1);
            vertices[6].Normal = new Vector3(0, 1, 0);
            vertices[6].Binormal = new Vector3(0, 0, -1);
            vertices[6].Tangent = new Vector3(1, 0, 0);

            vertices[7].Position = new Vector3(10f, 0f, 10f);
            vertices[7].TextureCoordinate = new Vector2(1, 1);
            vertices[7].Normal = new Vector3(0, 1, 0);
            vertices[7].Binormal = new Vector3(0, 0, -1);
            vertices[7].Tangent = new Vector3(1, 0, 0);
        }
        private void SetupIndices()
        {
            indices1 = new int[24];
            indices1[0] = 0;
            indices1[1] = 1;
            indices1[2] = 1;
            indices1[3] = 2;
            indices1[4] = 2;
            indices1[5] = 3;
            indices1[6] = 3;
            indices1[7] = 0;
            indices1[8] = 0;
            indices1[9] = 4;
            indices1[10] = 1;
            indices1[11] = 5;
            indices1[12] = 2;
            indices1[13] = 6;
            indices1[14] = 3;
            indices1[15] = 7;
            indices1[16] = 4;
            indices1[17] = 5;
            indices1[18] = 5;
            indices1[19] = 6;
            indices1[20] = 6;
            indices1[21] = 7;
            indices1[22] = 7;
            indices1[23] = 4;

            indices2 = new int[36];
            indices2[0] = 0;
            indices2[1] = 1;
            indices2[2] = 2;
            indices2[3] = 0;
            indices2[4] = 2;
            indices2[5] = 3;
            indices2[6] = 0;
            indices2[7] = 4;
            indices2[8] = 1;
            indices2[9] = 4;
            indices2[10] = 5;
            indices2[11] = 1;
            indices2[12] = 1;
            indices2[13] = 5;
            indices2[14] = 6;
            indices2[15] = 1;
            indices2[16] = 6;
            indices2[17] = 2;
            indices2[18] = 2;
            indices2[19] = 6;
            indices2[20] = 7;
            indices2[21] = 2;
            indices2[22] = 7;
            indices2[23] = 3;
            indices2[24] = 3;
            indices2[25] = 7;
            indices2[26] = 0;
            indices2[27] = 0;
            indices2[28] = 7;
            indices2[29] = 4;
            indices2[30] = 4;
            indices2[31] = 6;
            indices2[32] = 5;
            indices2[33] = 4;
            indices2[34] = 7;
            indices2[35] = 6;
        }
        private void CopyToBuffers()
        {
            vertexBuffer = new VertexBuffer(Global.device, VertexPositionTangentTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
            indexBuffer1 = new IndexBuffer(Global.device, typeof(int), indices1.Length, BufferUsage.WriteOnly);
            indexBuffer1.SetData(indices1);
            indexBuffer2 = new IndexBuffer(Global.device, typeof(int), indices2.Length, BufferUsage.WriteOnly);
            indexBuffer2.SetData(indices2);
        }
        public override void Update(GameTime gameTime)
        {
        }
        public void UpdateComponents(Matrix viewProjection)
        {
            boundingFrustum = new BoundingFrustum(viewProjection);

            Vector3[] corners = boundingFrustum.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                vertices[i].Position = corners[i];
            }
            vertexBuffer.SetData(vertices);
        }
        public override void Draw(GameTime gameTime)
        {
            switch (parent.state)
            {
                case STATE_INACTIVE:
                case STATE_DEATH:
                case STATE_BIRTH:
                case STATE_CONTROL:
                    break;
                default:
                    DrawFill();
                    break;
            }
        }
        public void DrawFill()
        {
            Global.World.SetValue(Matrix.Identity);
            for (int j = 0; j < Global.effect.CurrentTechnique.Passes.Count; j++)
            {
                Global.effect.CurrentTechnique.Passes[j].Apply();
                Global.device.SetVertexBuffer(vertexBuffer);
                Global.device.Indices = indexBuffer2;
                Global.device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, 12);
            }
        }
        public void DrawOutline()
        {
            Global.World.SetValue(Matrix.Identity);
            for (int j = 0; j < Global.effect.CurrentTechnique.Passes.Count; j++)
            {
                Global.effect.CurrentTechnique.Passes[j].Apply();
                Global.device.SetVertexBuffer(vertexBuffer);
                Global.device.Indices = indexBuffer1;
                Global.device.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, vertices.Length, 0, 12);
            }
        }
    }
}
