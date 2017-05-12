using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Netsky_Engine.World
{
    public abstract class ModelObject : DrawnObject3D
    {
        public Model model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                modelTransforms = new Matrix[_model.Bones.Count];
                _model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            }
        }
        Model _model;
        protected Matrix[] modelTransforms;
        public override void LoadContent()
        {
            base.LoadContent();
        }
        protected override void DrawModel()
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                //ignore objects offscreen
                //if (Global.currentGame.camera1.frustum.Contains(mesh.BoundingSphere) == ContainmentType.Disjoint) continue;

                Global.World.SetValue(modelTransforms[mesh.ParentBone.Index] * matrix);

                //draw object
                for (int i = 0; i < mesh.MeshParts.Count; i++)
                {
                    var part = mesh.MeshParts[i];

                    if (part.PrimitiveCount > 0)
                    {
                        Global.device.SetVertexBuffer(part.VertexBuffer);
                        Global.device.Indices = part.IndexBuffer;

                        for (int j = 0; j < Global.effect.CurrentTechnique.Passes.Count; j++)
                        {
                            Global.effect.CurrentTechnique.Passes[j].Apply();

                            //If you get error here: Set "generate tangent frames" to true
                            Global.device.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                part.VertexOffset, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                        }
                    }
                }
            }
        }
    }
}
