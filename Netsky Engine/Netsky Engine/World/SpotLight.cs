using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Dynamic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Netsky_Engine.World
{
    public class SpotLight : Light
    {
        public float decay;
        public float coneAngle;
        public Vector3 coneDirection
        {
            get
            {
                Vector3 d = facingPosition - position;
                d.Normalize();
                return d;
            }
        }
        public Attenuation Attenuation;
        public override void LoadContent()
        {
            base.LoadContent();
            color = Color.Cyan;
            intensity = .5f;
            position = new Vector3(10, 20, 50);
            facingPosition = new Vector3(0f, -1, -0.7f);
            facingPosition.Normalize();
            decay = 10f;
            coneAngle = (float)Math.Cos(MathHelper.ToRadians(30));
            Attenuation.Constant = 0f;
            Attenuation.Linear = 1f;
            Attenuation.Quadratic = 0f;

            minDepth = 1f;
            maxDepth = 1000.0f;
            viewPort = 1.0f;
            fieldOfView = MathHelper.PiOver2;
            state = STATE_FOCUS;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (castsAShadow) Global.effect.CurrentTechnique = Global.Spot2;
            else Global.effect.CurrentTechnique = Global.Spot;
            Global.LightDecay.SetValue(decay);
            Global.LightAngle.SetValue(coneAngle);
            Global.LightDirection.SetValue(coneDirection);
            Global.LightPosition.SetValue(position);
            Global.LightViewProjection.SetValue(viewProjection);
            Global.LightAttenuationConstant.SetValue(Attenuation.Constant);
            Global.LightAttenuationLinear.SetValue(Attenuation.Linear);
            Global.LightAttenuationQuadratic.SetValue(Attenuation.Quadratic);
        }
    }
}
