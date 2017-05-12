using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Netsky_Engine.World
{
    public abstract class DrawnObject3D : Object3D
    {
        public Texture2D ColorMap;
        public Texture2D NormalMap;
        public Texture2D SpecularMap;
        public Material Material;

        public override void LoadContent()
        {
            base.LoadContent();
            ColorMap = Global.nullDiffuse;
            NormalMap = Global.nullNormal;
            SpecularMap = Global.nullSpecular;

            Material.ambient = 0f;
            Material.diffuseColor = Color.White.ToVector4();
            Material.diffuse = 1f;
            Material.shininess = 1f;
            Material.matteColor = Color.Wheat.ToVector4();
        }
        public override void Draw(GameTime gameTime)
        {
            //set texel maps
            Global.ColorMap.SetValue(ColorMap);
            Global.NormalMap.SetValue(NormalMap);
            Global.SpecularMap.SetValue(SpecularMap);
            //set reflective attributes
            Global.MaterialAmbient.SetValue(Material.ambient);
            Global.MaterialColor.SetValue(Material.diffuseColor);
            Global.MaterialDiffuse.SetValue(Material.diffuse);
            Global.MaterialShininess.SetValue(Material.shininess);
            Global.MaterialMatte.SetValue(Material.matteColor);

            DrawModel();
        }
        public override void DrawShadow(GameTime gameTime)
        {
            DrawModel();
        }
        protected virtual void DrawModel()
        {
        }
    }
}
