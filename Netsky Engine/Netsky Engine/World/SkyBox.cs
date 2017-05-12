using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Netsky_Engine.World
{
    public class SkyBox : ModelObject
    {

        public override void LoadContent()
        {
            model = Global.skyboxModel;
            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            matrix = Matrix.CreateFromQuaternion(dimension.rotation) *
                     Matrix.CreateTranslation(dimension.camera.translation);
        }
        public override void Draw(GameTime gameTime)
        {
            CustomStateStart();
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = modelTransforms[mesh.ParentBone.Index] * matrix;
                    Vector3 up = Vector3.Transform(Vector3.Up, dimension.camera.rotation);
                    Matrix view = Matrix.CreateLookAt(dimension.camera.translation, dimension.camera.facingPosition, up);
                    effect.View = view;
                    effect.Projection = dimension.camera.projection;
                }
                mesh.Draw();
            }
            CustomStateEnd();
        }
        public override void DrawShadow(GameTime gameTime)
        {
        }

        DepthStencilState dssOLD;
        RasterizerState rsOLD;
        SamplerState ssOLD;
        BlendState bs;

        private void CustomStateStart()
        {
            dssOLD = Global.device.DepthStencilState;
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            Global.device.DepthStencilState = dss;

            rsOLD = Global.device.RasterizerState;
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            rs.FillMode = FillMode.Solid;
            Global.device.RasterizerState = rs;

            ssOLD = Global.device.SamplerStates[0];
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            Global.device.SamplerStates[0] = ss;

            bs = Global.device.BlendState;
            Global.device.BlendState = BlendState.Opaque;
        }
        private void CustomStateEnd()
        {
            Global.device.DepthStencilState = dssOLD;
            Global.device.RasterizerState = rsOLD;
            Global.device.SamplerStates[0] = ssOLD;
            Global.device.BlendState = bs;
        }
    }
}
