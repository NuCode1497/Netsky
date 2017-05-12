using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Netsky_Engine.World;
namespace Netsky_Engine
{
    public class Global
    {
        //DEVICE STATES
        static public Game1 currentGame;
        static public ContentManager Content;
        static public SpriteBatch spriteBatch;
        static GraphicsDeviceManager graphics;
        static public GraphicsDevice device;
        static public DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        static public int defaultWindowWidth = 1792;
        static public int defaultWindowHeight = 1008;
        static public int windowWidth
        {
            get
            {
                return graphics.PreferredBackBufferWidth;
            }
            set
            {
                graphics.PreferredBackBufferWidth = value;
                graphics.ApplyChanges();
            }
        }
        static public int windowHeight
        {
            get
            {
                return graphics.PreferredBackBufferHeight;
            }
            set
            {
                graphics.PreferredBackBufferHeight = value;
                graphics.ApplyChanges();
            }
        }
        static public int screenWidth
        {
            get
            {
                return displayMode.Width;
            }
        }
        static public int screenHeight
        {
            get
            {
                return displayMode.Height;
            }
        }

        //INPUT STATES
        static public KeyboardState keyboardState;
        static public KeyboardState oldKeyboardState;
        static public MouseState mouseState;
        static public MouseState prevMouseState;
        static public int mouseScrollChange;
        static public Random random = new Random();

        //ASSETS
        #region ASSETS
        static public QuadRenderComponent quadRenderer;
        static public Texture2D placeholder;
        static public Texture2D cursorImage;
        static public Texture2D crosshairs;
        static public SpriteFont Consolas18;
        static public SpriteFont Verdana8;
        static public Model skyboxModel;
        static public Model zombieModel;
        static public Model zombieSceneModel;
        static public Texture2D zombieDiffuse;
        static public Texture2D zombieNormal;
        static public Texture2D groundDiffuse;
        static public Texture2D groundNormal;
        static public Texture2D groundSpecular;
        static public Texture2D nullDiffuse;
        static public Texture2D nullNormal;
        static public Texture2D nullSpecular;
        static public Model ship1Model;
        static public Model ship2Model;
        static public Texture2D ship1Diffuse;
        static public Texture2D ship1Normal;
        static public Texture2D ship1Specular;
        static public Texture2D ship2Diffuse;
        static public Texture2D ship2Normal;
        static public Texture2D ship2Specular;
        static public Model sphereModel;
        static public Model arrowShaft;
        static public Model arrowHead;
        static public Model cameraModel;
        #endregion

        //EFFECTS
        #region EFFECTS
        static public Effect effect;
        static public Texture2D blackImage;
        static public Vector2 halfPixel;
        //INPUT
        static public EffectParameter World;
        static public EffectParameter ViewProjection;
        static public EffectParameter InvViewProjection;
        static public EffectParameter CameraPosition;
        static public EffectParameter ColorMap;
        static public EffectParameter NormalMap;
        static public EffectParameter DepthMap;
        static public EffectParameter SpecularMap;
        static public EffectParameter ShadowMap;
        static public EffectParameter LightMap;
        static public EffectParameter PreviousLightMap;
        static public EffectParameter ReflectMap;
        static public EffectParameter PreviousReflectMap;
        static public EffectParameter HalfPixel;
        //LIGHT
        static public EffectParameter LightColor;
        static public EffectParameter LightIntensity;
        static public EffectParameter LightSpecFactor;
        static public EffectParameter LightSpecPower;
        static public EffectParameter LightPosition;
        static public EffectParameter LightDirection;
        static public EffectParameter LightAngle;
        static public EffectParameter LightDecay;
        static public EffectParameter LightRadius;
        static public EffectParameter LightViewProjection;
        static public EffectParameter LightAttenuationConstant;
        static public EffectParameter LightAttenuationLinear;
        static public EffectParameter LightAttenuationQuadratic;
        //MATERIAL
        static public EffectParameter MaterialAmbient; //ambient factor
        static public EffectParameter MaterialColor; //diffuse color
        static public EffectParameter MaterialDiffuse; //diffuse factor
        static public EffectParameter MaterialShininess; //specular power
        static public EffectParameter MaterialMatte; //specular color; White for max shine or lerp to light color for matte
        //TECHNIQUES
        static public EffectTechnique Texture;
        static public EffectTechnique Scene;
        static public EffectTechnique Directional;
        static public EffectTechnique Ambient;
        static public EffectTechnique Point;
        static public EffectTechnique Spot;
        static public EffectTechnique Spot2;
        static public EffectTechnique Shadow;
        static public EffectTechnique Final;
        static public EffectTechnique NoTexture;
        static public EffectTechnique Basic;
        static public EffectTechnique TextureBlend;
        static public EffectTechnique Black;
        //RENDER TARGETS
        static public RenderTarget2D colorTarget;
        static public RenderTarget2D normalTarget;
        static public RenderTarget2D depthTarget;
        static public RenderTarget2D specularTarget;
        static public RenderTarget2D shadowTarget;
        static public RenderTarget2D lightTarget;
        static public RenderTarget2D reflectTarget;
        static public RenderTarget2D prevLightTarget;
        static public RenderTarget2D prevReflectTarget;
        static public RenderTarget2D debugTarget;
        #endregion

        static public void LoadContent()
        {
            //DEVICE STATES
            Content = currentGame.Content;
            graphics = currentGame.graphics;
            device = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(device);
            windowWidth = defaultWindowWidth;
            windowHeight = defaultWindowHeight;

            //ASSETS
            #region ASSETS
            //2D
            Consolas18 = Content.Load<SpriteFont>(@"GUI\Consolas18");
            Verdana8 = Content.Load<SpriteFont>(@"GUI\Verdana8");
            cursorImage = Content.Load<Texture2D>(@"GUI\cursor");
            placeholder = Content.Load<Texture2D>(@"GUI\nick");
            crosshairs = Content.Load<Texture2D>(@"GUI\crosshairs");

            //3D
            quadRenderer = new QuadRenderComponent(device);
            effect = Content.Load<Effect>(@"World\Effects\effects");
            nullDiffuse = Content.Load<Texture2D>(@"World\Models\null_c");
            nullNormal = Content.Load<Texture2D>(@"World\Models\null_n");
            nullSpecular = Content.Load<Texture2D>(@"World\Models\null_s");
            skyboxModel = Content.Load<Model>(@"World\Models\skybox");
            zombieModel = Content.Load<Model>(@"World\Models\zombie");
            zombieSceneModel = Content.Load<Model>(@"World\Models\zombieScene");
            zombieDiffuse = Content.Load<Texture2D>(@"World\Models\zombie_c");
            zombieNormal = Content.Load<Texture2D>(@"World\Models\zombie_n");
            groundDiffuse = Content.Load<Texture2D>(@"World\Models\ground_c");
            groundNormal = Content.Load<Texture2D>(@"World\Models\ground_n");
            groundSpecular = Content.Load<Texture2D>(@"World\Models\ground_s");
            ship1Model = Content.Load<Model>(@"World\Models\ship1");
            ship1Diffuse = Content.Load<Texture2D>(@"World\Models\ship1_c");
            ship1Normal = Content.Load<Texture2D>(@"World\Models\ship1_n");
            ship1Specular = Content.Load<Texture2D>(@"World\Models\ship1_s");
            ship2Model = Content.Load<Model>(@"World\Models\ship2");
            ship2Diffuse = Content.Load<Texture2D>(@"World\Models\ship2_c");
            ship2Normal = Content.Load<Texture2D>(@"World\Models\ship2_n");
            ship2Specular = Content.Load<Texture2D>(@"World\Models\ship2_s");
            sphereModel = Content.Load<Model>(@"World\Models\sphere");
            arrowHead = Content.Load<Model>(@"World\Models\arrowHead");
            arrowShaft = Content.Load<Model>(@"World\Models\arrowShaft");
             
            cameraModel = Content.Load<Model>(@"World\Models\camera");
            #endregion

            //EFFECTS
            #region EFFECTS
            blackImage = new Texture2D(device, screenWidth, screenHeight, false, SurfaceFormat.Color);
            halfPixel = new Vector2
            {
                X = 0.5f / (float)device.PresentationParameters.BackBufferWidth,
                Y = 0.5f / (float)device.PresentationParameters.BackBufferHeight
            };
            //INPUT
            World = effect.Parameters["xWorld"];
            ViewProjection = effect.Parameters["xViewProjection"];
            InvViewProjection = effect.Parameters["xInvViewProjection"];
            CameraPosition = effect.Parameters["xCameraPosition"];
            ColorMap = effect.Parameters["xColorMap"];
            NormalMap = effect.Parameters["xNormalMap"];
            DepthMap = effect.Parameters["xDepthMap"];
            SpecularMap = effect.Parameters["xSpecularMap"];
            ShadowMap = effect.Parameters["xShadowMap"];
            LightMap = effect.Parameters["xLightMap"];
            PreviousLightMap = effect.Parameters["xPreviousLightMap"];
            ReflectMap = effect.Parameters["xReflectMap"];
            PreviousReflectMap = effect.Parameters["xPreviousReflectMap"];
            HalfPixel = effect.Parameters["xHalfPixel"];
            HalfPixel.SetValue(halfPixel);
            //LIGHT
            LightColor = effect.Parameters["xLightColor"];
            LightIntensity = effect.Parameters["xLightIntensity"];
            LightSpecFactor = effect.Parameters["xLightSpecFactor"];
            LightSpecPower = effect.Parameters["xLightSpecPower"];
            LightPosition = effect.Parameters["xLightPosition"];
            LightDirection = effect.Parameters["xLightDirection"];
            LightAngle = effect.Parameters["xLightAngle"];
            LightDecay = effect.Parameters["xLightDecay"];
            LightRadius = effect.Parameters["xLightRadius"];
            LightViewProjection = effect.Parameters["xLightViewProjection"];
            LightAttenuationConstant = effect.Parameters["xLightAttC"];
            LightAttenuationLinear = effect.Parameters["xLightAttL"];
            LightAttenuationQuadratic = effect.Parameters["xLightAttQ"];
            //MATERIAL
            MaterialAmbient = effect.Parameters["xMaterialAmbient"];
            MaterialColor = effect.Parameters["xMaterialColor"];
            MaterialDiffuse = effect.Parameters["xMaterialDiffuse"];
            MaterialShininess = effect.Parameters["xMaterialShininess"];
            MaterialMatte = effect.Parameters["xMaterialMatte"];
            //TECHNIQUES
            Texture = effect.Techniques["Texture"];
            Scene = effect.Techniques["Scene"];
            Ambient = effect.Techniques["Ambient"];
            Directional = effect.Techniques["Directional"];
            Point = effect.Techniques["Point"];
            Spot = effect.Techniques["Spot"];
            Spot2 = effect.Techniques["Spot2"];
            Shadow = effect.Techniques["Shadow"];
            Final = effect.Techniques["Final"];
            NoTexture = effect.Techniques["NoTexture"];
            Basic = effect.Techniques["BasicNoTexture"];
            TextureBlend = effect.Techniques["TextureBlend"];
            Black = effect.Techniques["Black"];
            //RENDER TARGETS
            colorTarget = new RenderTarget2D(device, windowWidth, windowHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            normalTarget = new RenderTarget2D(device, windowWidth, windowHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            specularTarget = new RenderTarget2D(device, windowWidth, windowHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            depthTarget = new RenderTarget2D(device, windowWidth, windowHeight, false, SurfaceFormat.Single, DepthFormat.Depth24);
            shadowTarget = new RenderTarget2D(device, windowWidth * 2, windowHeight * 2, false, SurfaceFormat.Single, DepthFormat.Depth24);
            lightTarget = new RenderTarget2D(device, windowWidth, windowHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            prevLightTarget = new RenderTarget2D(device, windowWidth, windowHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            reflectTarget = new RenderTarget2D(device, windowWidth, windowHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            prevReflectTarget = new RenderTarget2D(device, windowWidth, windowHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            debugTarget = new RenderTarget2D(device, windowWidth, windowHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            #endregion

        }
        static public void UnloadContent()
        {
            colorTarget.Dispose();
            colorTarget = null;
            normalTarget.Dispose();
            normalTarget = null;
            depthTarget.Dispose();
            depthTarget = null;
            shadowTarget.Dispose();
            shadowTarget = null;
            lightTarget.Dispose();
            lightTarget = null;
            prevLightTarget.Dispose();
            prevLightTarget = null;
        }
        static int ID = 0;
        static public int generateID()
        {
            ID++;
            return ID;
        }
        public static void ResetDeviceDefault()
        {
            device.SamplerStates[0] = SamplerState.LinearClamp;

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            rs.FillMode = FillMode.Solid;
            device.RasterizerState = rs;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            device.DepthStencilState = dss;

            device.BlendState = BlendState.Opaque;

            device.Clear(Color.SteelBlue);
        }
        public static void CloneTexture2D(RenderTarget2D target, Texture2D source)
        {
            device.SetRenderTarget(target);
            device.Clear(Color.Black);
            RenderTexture(source);
            device.SetRenderTarget(null);
        }
        public static void Render()
        {            
            for (int j = 0; j < effect.CurrentTechnique.Passes.Count; j++)
            {
                effect.CurrentTechnique.Passes[j].Apply();
                quadRenderer.SetCorners(Vector2.One * -1, Vector2.One);
                quadRenderer.Render();
            }
        }
        public static void RenderTexture(Texture t)
        {
            RenderTexture(t, Vector2.One * -1, Vector2.One);
        }
        public static void RenderTexture(Texture t, Vector2 bottomLeft, Vector2 topRight)
        {
            HalfPixel.SetValue(halfPixel);
            ColorMap.SetValue(t);
            effect.CurrentTechnique = Texture;
            effect.CurrentTechnique.Passes[0].Apply();
            quadRenderer.SetCorners(bottomLeft, topRight);
            quadRenderer.Render();
        }
        public static void AlphaBlendTextures(RenderTarget2D baseTexture, Texture overlayTexture)
        {
            Global.device.SetRenderTarget(Global.prevLightTarget);
            Global.ColorMap.SetValue(baseTexture);
            Global.NormalMap.SetValue(overlayTexture);
            Global.effect.CurrentTechnique = Global.TextureBlend;
            Global.Render();
            Global.device.SetRenderTarget(null);
            Global.CloneTexture2D(baseTexture, Global.prevLightTarget);
        }
        public static T[,] TextureTo2DArray<T>(Texture2D texture) where T : struct
        {
            T[] colors1D = new T[texture.Width * texture.Height];
            texture.GetData<T>(colors1D);

            T[,] colors2D = new T[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }
        public static bool Pressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) && !oldKeyboardState.IsKeyDown(key);
        }
        public static bool WindowClicked()
        {
            return (device.Viewport.Bounds.Contains(new Point(mouseState.X, mouseState.Y))
                && ((mouseState.LeftButton == ButtonState.Pressed) && !(prevMouseState.LeftButton == ButtonState.Pressed)));
        }
    }
    public struct Material
    {
        /// <summary>
        /// Ambient Factor. Illuminates the object. Set to 0 for no illumination.
        /// Setting to 1 causes it to ignore lighting.
        /// </summary>
        public float ambient;
        /// <summary>
        /// Shades the object this color multiplied with ColorMap.
        /// </summary>
        public Vector4 diffuseColor;
        /// <summary>
        /// Diffuse Factor. Controls the brightness of reflected light sources.
        /// </summary>
        public float diffuse;
        /// <summary>
        /// Specular Power. Controls how responsive the surface is to direct reflections.
        /// </summary>
        public float shininess;
        /// <summary>
        /// Specular Color. The color of directly reflected light. Lerp from 
        /// white to surface color for a progressively more matte sheen.
        /// </summary>
        public Vector4 matteColor;
    }
    public struct Attenuation
    {
        public float Constant;
        public float Linear;
        public float Quadratic;
    }
    public struct VertexPositionTangentTexture
    {
        public Vector3 Position;
        public Vector2 TextureCoordinate;
        public Vector3 Normal;
        public Vector3 Binormal;
        public Vector3 Tangent;

        public VertexPositionTangentTexture(Vector3 Position, Vector2 TextureCoordinate, Vector3 Normal, Vector3 Binormal, Vector3 Tangent)
        {
            this.Position = Position;
            this.TextureCoordinate = TextureCoordinate;
            this.Normal = Normal;
            this.Binormal = Binormal;
            this.Tangent = Tangent;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 5, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0),
            new VertexElement(sizeof(float) * 11, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0)
        );
    }
}
