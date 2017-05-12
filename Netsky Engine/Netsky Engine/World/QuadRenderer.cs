using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Netsky_Engine.World
{
    /// <summary>
    /// Renders the screen by a specified view port, or the entire screen by default.
    /// </summary>
    public class QuadRenderComponent
    {
        VertexPositionTexture[] vertices = null;
        GraphicsDevice device;

        public QuadRenderComponent(GraphicsDevice device)
        {
            this.device = device;
            vertices = new VertexPositionTexture[4];
            int i = 0;
            vertices[i++] = new VertexPositionTexture(new Vector3(-1, 1, 0f), new Vector2(0, 0));
            vertices[i++] = new VertexPositionTexture(new Vector3(1, 1, 0f), new Vector2(1, 0));
            vertices[i++] = new VertexPositionTexture(new Vector3(-1, -1, 0f), new Vector2(0, 1));
            vertices[i++] = new VertexPositionTexture(new Vector3(1, -1, 0f), new Vector2(1, 1));
        }
        public void Render()
        {
            device.DrawUserPrimitives<VertexPositionTexture>
                (PrimitiveType.TriangleStrip, vertices, 0, 2);
        }
        public void SetCorners(Vector2 bottomLeft, Vector2 topRight)
        {
            vertices[0].Position.X = bottomLeft.X;
            vertices[0].Position.Y = topRight.Y;
            vertices[1].Position.X = topRight.X;
            vertices[1].Position.Y = topRight.Y;
            vertices[2].Position.X = bottomLeft.X;
            vertices[2].Position.Y = bottomLeft.Y;
            vertices[3].Position.X = topRight.X;
            vertices[3].Position.Y = bottomLeft.Y;
        }
    }
}
