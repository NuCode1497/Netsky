using Microsoft.Xna.Framework;

namespace Netsky_Engine.World
{
    public abstract class Light : Camera
    {
        public bool castsAShadow = false;
        public Color color = Color.White;
        public float intensity = 0f;
        public float specularFactor = 1f;
        public float specularPower = 1f;

        public override void LoadContent()
        {
            base.LoadContent();
            castsAShadow = false;
            color = Color.White;
            intensity = .3f;
            specularFactor = 1f;
            specularPower = 1f;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            Global.LightColor.SetValue(color.ToVector4());
            Global.LightIntensity.SetValue(intensity);
            Global.LightSpecFactor.SetValue(specularFactor);
            Global.LightSpecPower.SetValue(specularPower);
        }
    }
}
