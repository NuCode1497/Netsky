using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Netsky_Engine.World;

namespace Netsky_Engine.World
{
    /// <summary>
    /// A self contained dimension of 3D objects drawn with deferred shadow rendering.
    /// </summary>
    public class World3D : ListOfObject3Ds
    {
        #region states
        public const int STATE_DEBUG = 5;
        public const int STATE_D_CONTROL = 6;
        public const int STATE_TURBO = 7;
        public const int STATE_TURBO_CONTROL = 8;
        public const int STATE_D_TURBO_CONTROL = 9;
        public const int STATE_D_TURBO = 10;
        public enum DebugModes
        {
            None, Split, Color, Normal, Specular, Depth, Light, Reflect, NoTexture
        }
        public enum ControlModes
        {
            /// <summary>
            /// Camera is not controlled by the user.
            /// </summary>
            None, 
            /// <summary>
            /// Camera movement is controlled with the keyboard 
            /// and rotation is controlled by the mouse 
            /// with a fixed rotation.
            /// </summary>
            Quake, 
            /// <summary>
            /// Camera movement and rotation is controlled with the keyboard.
            /// </summary>
            Free,
        }
        public enum UpdateModes
        {
            /// <summary>
            /// World objects are  not updated.
            /// </summary>
            Freeze, 
            /// <summary>
            /// Normal updates.
            /// </summary>
            Normal, 
            /// <summary>
            /// World objects are updated with modified gameTime.
            /// </summary>
            Turbo,
            /// <summary>
            /// Update one frame.
            /// </summary>
            Step,
        }
        public DebugModes DebugMode
        {
            get
            {
                return _DebugMode;
            }
            set
            {
                _DebugMode = value;
                switch (value)
                {
                    case DebugModes.None:
                        switch (state)
                        {
                            case STATE_DEBUG:
                                state = STATE_ACTIVE;
                                break;
                            case STATE_D_CONTROL:
                                state = STATE_CONTROL;
                                break;
                            case STATE_D_TURBO_CONTROL:
                                state = STATE_TURBO_CONTROL;
                                break;
                        }
                        break;
                    default:
                        switch (state)
                        {
                            case STATE_ACTIVE:
                                state = STATE_DEBUG;
                                break;
                            case STATE_TURBO:
                                state = STATE_D_TURBO;
                                break;
                            case STATE_CONTROL:
                                state = STATE_D_CONTROL;
                                break;
                            case STATE_TURBO_CONTROL:
                                state = STATE_D_TURBO_CONTROL;
                                break;
                        }
                        break;
                }
            }
        }
        public ControlModes ControlMode
        {
            get
            {
                return _ControlMode;
            }
            set
            {
                _ControlMode = value;
                switch (value)
                {
                    case ControlModes.None:
                        switch (state)
                        {
                            case STATE_CONTROL:
                                state = STATE_ACTIVE;
                                break;
                            case STATE_D_CONTROL:
                                state = STATE_DEBUG;
                                break;
                            case STATE_TURBO_CONTROL:
                                state = STATE_TURBO;
                                break;
                            case STATE_D_TURBO_CONTROL:
                                state = STATE_D_TURBO;
                                break;
                        }
                        break;
                    default:
                        switch (state)
                        {
                            case STATE_BIRTH:
                            case STATE_ACTIVE:
                                state = STATE_CONTROL;
                                break;
                            case STATE_DEBUG:
                                state = STATE_D_CONTROL;
                                break;
                            case STATE_TURBO:
                                state = STATE_TURBO_CONTROL;
                                break;
                            case STATE_D_TURBO:
                                state = STATE_D_TURBO_CONTROL;
                                break;
                        }
                        break;
                }
            }
        }
        public  UpdateModes UpdateMode
        {
            get
            {
                return _UpdateMode;
            }
            set
            {
                _UpdateMode = value;
                switch (value)
                {
                    case UpdateModes.Normal:
                        switch (state)
                        {
                            case STATE_TURBO:
                                state = STATE_ACTIVE;
                                break;
                            case STATE_D_TURBO:
                                state = STATE_DEBUG;
                                break;
                            case STATE_TURBO_CONTROL:
                                state = STATE_CONTROL;
                                break;
                            case STATE_D_TURBO_CONTROL:
                                state = STATE_D_CONTROL;
                                break;
                        }
                        break;
                    default:
                        switch (state)
                        {
                            case STATE_ACTIVE:
                                state = STATE_TURBO;
                                break;
                            case STATE_DEBUG:
                                state = STATE_D_TURBO;
                                break;
                            case STATE_CONTROL:
                                state = STATE_TURBO_CONTROL;
                                break;
                            case STATE_D_CONTROL:
                                state = STATE_D_TURBO_CONTROL;
                                break;
                        }
                        break;
                }
            }
        }
        private DebugModes _DebugMode;
        private ControlModes _ControlMode;
        private UpdateModes _UpdateMode;
        #endregion

        //OVERLAY
        public RenderTarget2D FinalImage { get; set; }
        private RenderTarget2D OverlayImage;
        private RenderTarget2D TransImage;
        private BlindList3D Overlays;
        private BlindList3D Transparencies;

        //MEMBERS
        public Camera camera;
        public SkyBox skybox;
        public AmbientLight ambientLight;
        private BlindList3D Lights;

        //CONTROLS
        private Object3D controlObject;
        private MouseState originalMouseState;

        //OPTIONS
        public bool wireFrame
        {
            get
            {
                return _wireFrame == FillMode.WireFrame ? true : false;
            }
            set
            {
                if(value)
                {
                    _wireFrame = FillMode.WireFrame;
                }
                else
                {
                    _wireFrame = FillMode.Solid;
                }
            }
        }
        private FillMode _wireFrame;
        public float mouseSensitivity;
        public float keySensitivity;
        public float gravity;
        public float timeDilation;
        public float time
        {
            get
            {
                return _time;
            }
        }
        private float _time;

        public override void LoadContent()
        {
            base.LoadContent();
            dimension = this;
            state = STATE_BIRTH;

            Lights = new BlindList3D();
            Lights.LoadContent();
            ambientLight = new AmbientLight();
            ambientLight.LoadContent();
            AddLight(ambientLight);

            FinalImage = new RenderTarget2D(Global.device, Global.windowWidth, Global.windowHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            OverlayImage = new RenderTarget2D(Global.device, Global.windowWidth, Global.windowHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            TransImage = new RenderTarget2D(Global.device, Global.windowWidth, Global.windowHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            Overlays = new BlindList3D();
            Overlays.LoadContent();
            Transparencies = new BlindList3D();
            Transparencies.LoadContent();

            skybox = new SkyBox();
            skybox.LoadContent();
            skybox.dimension = this;

            //OPTIONS
            wireFrame = false;
            mouseSensitivity = 0.001f;
            keySensitivity = 1f;
            gravity = -0.4f;
            _DebugMode = DebugModes.None;
            _ControlMode = ControlModes.None;
            _UpdateMode = UpdateModes.Normal;
            timeDilation = 1f;
        }
        public override void UnloadContent()
        {
            FinalImage.Dispose();
            FinalImage = null;
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_INACTIVE:
                case STATE_DEATH:
                    break;
                case STATE_BIRTH:
                    state = STATE_ACTIVE;
                    break;
                case STATE_DEBUG:
                case STATE_ACTIVE:
                    UpdateKinetics(gameTime);
                    UpdateWorldMatrix();
                    Lights.Update(gameTime);
                    base.Update(gameTime);
                    Transparencies.Update(gameTime);
                    Overlays.Update(gameTime);
                    skybox.Update(gameTime);
                    break;
                case STATE_CONTROL:
                case STATE_D_CONTROL:
                    UpdateKinetics(gameTime);
                    UpdateWorldMatrix();
                    Control(gameTime);
                    Lights.Update(gameTime);
                    base.Update(gameTime);
                    Transparencies.Update(gameTime);
                    Overlays.Update(gameTime);
                    skybox.Update(gameTime);
                    break;
                case STATE_TURBO_CONTROL:
                case STATE_D_TURBO_CONTROL:
                    UpdateKinetics(gameTime);
                    UpdateWorldMatrix();
                    Control(gameTime);
                    controlObject.Update(gameTime);
                    Turbo(gameTime);
                    Overlays.Update(gameTime);
                    skybox.Update(gameTime);
                    break;
                case STATE_TURBO:
                case STATE_D_TURBO:
                    UpdateKinetics(gameTime);
                    UpdateWorldMatrix();
                    Overlays.Update(gameTime);
                    skybox.Update(gameTime);
                    break;
            }
        }
        public override void UpdateKinetics(GameTime gameTime)
        {
            _time = gameTime.ElapsedGameTime.Milliseconds / 100f * timeDilation;
            base.UpdateKinetics(gameTime);
        }
        #region Control
        private void Control(GameTime gameTime)
        {
            switch (ControlMode)
            {
                case ControlModes.Free:
                    UpdateFree(gameTime);
                    break;
                case ControlModes.Quake:
                    UpdateQuake(gameTime);
                    break;
            }
        }
        public void ControlQuake(Object3D o)
        {
            Mouse.SetPosition(Global.device.Viewport.Width / 2, Global.device.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();
            controlObject = o;
            controlObject.state = STATE_CONTROL;
            ControlMode = ControlModes.Quake;
        }
        private void UpdateQuake(GameTime gameTime)
        {
            if (!Global.currentGame.IsActive) return;
            if (originalMouseState != Global.mouseState)
            {
                float xDifference = Global.mouseState.X - originalMouseState.X;
                float yDifference = Global.mouseState.Y - originalMouseState.Y;
                controlObject.angle.Y -= (float)(mouseSensitivity * xDifference); //change -= to invert y axis
                controlObject.angle.X -= (float)(mouseSensitivity * yDifference); //change += to invert x axis
                controlObject.angle.X = MathHelper.Clamp(controlObject.angle.X, -MathHelper.PiOver2, MathHelper.PiOver2);
                Mouse.SetPosition(Global.device.Viewport.Width / 2, Global.device.Viewport.Height / 2);
            }

            if (Global.keyboardState.IsKeyDown(Keys.Up) || Global.keyboardState.IsKeyDown(Keys.W))
                controlObject.acceleration += keySensitivity * Vector3.Transform(Vector3.Forward, controlObject.spin);
            if (Global.keyboardState.IsKeyDown(Keys.Left) || Global.keyboardState.IsKeyDown(Keys.A))
                controlObject.acceleration += keySensitivity * Vector3.Transform(Vector3.Left, controlObject.spin);
            if (Global.keyboardState.IsKeyDown(Keys.Down) || Global.keyboardState.IsKeyDown(Keys.S))
                controlObject.acceleration += keySensitivity * Vector3.Transform(Vector3.Backward, controlObject.spin);
            if (Global.keyboardState.IsKeyDown(Keys.Right) || Global.keyboardState.IsKeyDown(Keys.D))
                controlObject.acceleration += keySensitivity * Vector3.Transform(Vector3.Right, controlObject.spin);
            if (Global.keyboardState.IsKeyDown(Keys.Space))
                controlObject.acceleration += keySensitivity * Vector3.Transform(Vector3.Up, controlObject.spin);
            if (Global.keyboardState.IsKeyDown(Keys.LeftShift))
                controlObject.acceleration += keySensitivity * Vector3.Transform(Vector3.Down, controlObject.spin);
        }
        public void ControlFree(Object3D o)
        {
            controlObject = o;
            ControlMode = ControlModes.Free;
        }
        private void UpdateFree(GameTime gameTime)
        {
            if (!Global.currentGame.IsActive) return;

            if (Global.keyboardState.IsKeyDown(Keys.A))
                controlObject.angle.Y += keySensitivity;
            if (Global.keyboardState.IsKeyDown(Keys.D))
                controlObject.angle.Y -= keySensitivity;
            if (Global.keyboardState.IsKeyDown(Keys.W))
                controlObject.angle.X -= keySensitivity;
            if (Global.keyboardState.IsKeyDown(Keys.S))
                controlObject.angle.X += keySensitivity;
            if (Global.keyboardState.IsKeyDown(Keys.Q))
                controlObject.angle.Z += keySensitivity;
            if (Global.keyboardState.IsKeyDown(Keys.E))
                controlObject.angle.Z -= keySensitivity;

            if (Global.keyboardState.IsKeyDown(Keys.LeftShift))
            {
                controlObject.acceleration = Vector3.Transform(new Vector3(0, 0, -.2f), controlObject.spin);
            }
            else
            {
                controlObject.acceleration = Vector3.Zero;
            }
        }
        public void ToggleCameraLight()
        {
            switch (ambientLight.state)
            {
                case AmbientLight.STATE_ACTIVE:
                    ambientLight.state = AmbientLight.STATE_INACTIVE;
                    break;
                case AmbientLight.STATE_INACTIVE:
                    ambientLight.state = AmbientLight.STATE_ACTIVE;
                    break;
            }
        }
        private void Turbo(GameTime gameTime)
        {
            switch (UpdateMode)
            {
                case UpdateModes.Step:
                    Lights.Update(gameTime);
                    base.Update(gameTime);
                    UpdateMode = UpdateModes.Freeze;
                    break;
            }
        }
        #endregion
        #region Draw
        public override void Draw(GameTime gameTime)
        {
            ResetDevice();
            switch (state)
            {
                case STATE_DEATH:
                case STATE_BIRTH:
                case STATE_INACTIVE:
                    break;
                case STATE_CONTROL:
                case STATE_TURBO:
                case STATE_ACTIVE:
                case STATE_TURBO_CONTROL:
                    DrawScene(gameTime);
                    DrawLights(gameTime);
                    Global.device.SetRenderTarget(FinalImage);
                    CombineColorAndLight();
                    DrawTransparencies(gameTime);
                    DrawLights(gameTime);
                    Global.device.SetRenderTarget(TransImage);
                    CombineColorAndLightTransparent();
                    Global.AlphaBlendTextures(FinalImage, TransImage);
                    DrawOverlays(gameTime);
                    Global.AlphaBlendTextures(FinalImage, OverlayImage);
                    break;
                case STATE_DEBUG:
                case STATE_D_CONTROL:
                case STATE_D_TURBO:
                case STATE_D_TURBO_CONTROL:
                    Debug(gameTime);
                    break;
            }
        }
        private void Debug(GameTime gameTime)
        {
            switch (DebugMode)
            {
                case DebugModes.None:
                    break;
                case DebugModes.Split:
                    DrawScene(gameTime);
                    DrawLights(gameTime);
                    Global.device.SetRenderTarget(Global.debugTarget);
                    CombineColorAndLight();
                    Global.AlphaBlendTextures(Global.debugTarget, TransImage);
                    Global.AlphaBlendTextures(Global.debugTarget, OverlayImage);
                    Global.device.SetRenderTarget(FinalImage);
                    Global.RenderTexture(Global.colorTarget, new Vector2(-1.0f, 1.0f / 3.0f), new Vector2(-1.0f / 3.0f, 1.0f));
                    Global.RenderTexture(Global.normalTarget, new Vector2(-1.0f / 3.0f, 1.0f / 3.0f), new Vector2(1.0f / 3.0f, 1.0f));
                    Global.RenderTexture(Global.specularTarget, Vector2.One / 3.0f, Vector2.One);
                    Global.device.SamplerStates[0] = SamplerState.PointClamp;
                    Global.RenderTexture(Global.depthTarget, new Vector2(-1.0f, -1.0f / 3.0f), new Vector2(-1.0f / 3.0f, 1.0f / 3.0f));
                    Global.RenderTexture(TransImage, Vector2.One / -3.0f, Vector2.One / 3.0f);
                    Global.RenderTexture(OverlayImage, new Vector2(1.0f / 3.0f, -1.0f / 3.0f), new Vector2(1.0f, 1.0f / 3.0f));
                    Global.device.SamplerStates[0] = SamplerState.LinearClamp;
                    Global.RenderTexture(Global.lightTarget, Vector2.One * -1.0f, Vector2.One / -3.0f);
                    Global.RenderTexture(Global.reflectTarget, new Vector2(-1.0f / 3.0f, -1.0f), new Vector2(1.0f / 3.0f, -1.0f / 3.0f));
                    Global.RenderTexture(Global.debugTarget, new Vector2(1.0f / 3.0f, -1.0f), new Vector2(1.0f, -1.0f / 3.0f));
                    DrawTransparencies(gameTime);
                    DrawLights(gameTime);
                    Global.device.SetRenderTarget(TransImage);
                    CombineColorAndLightTransparent();
                    DrawOverlays(gameTime);
                    break;
                case DebugModes.Color:
                    DrawScene(gameTime);
                    Global.ResetDeviceDefault();
                    Global.device.SetRenderTarget(FinalImage);
                    Global.RenderTexture(Global.colorTarget);
                    DrawOverlays(gameTime);
                    Global.AlphaBlendTextures(FinalImage, OverlayImage);
                    break;
                case DebugModes.Normal:
                    DrawScene(gameTime);
                    Global.ResetDeviceDefault();
                    Global.device.SetRenderTarget(FinalImage);
                    Global.RenderTexture(Global.normalTarget);
                    DrawOverlays(gameTime);
                    Global.AlphaBlendTextures(FinalImage, OverlayImage);
                    break;
                case DebugModes.Specular:
                    DrawScene(gameTime);
                    Global.ResetDeviceDefault();
                    Global.device.SetRenderTarget(FinalImage);
                    Global.RenderTexture(Global.specularTarget);
                    DrawOverlays(gameTime);
                    Global.AlphaBlendTextures(FinalImage, OverlayImage);
                    break;
                case DebugModes.Depth:
                    DrawScene(gameTime);
                    Global.ResetDeviceDefault();
                    Global.device.SetRenderTarget(FinalImage);
                    Global.device.SamplerStates[0] = SamplerState.PointClamp;
                    Global.RenderTexture(Global.depthTarget);
                    Global.device.SamplerStates[0] = SamplerState.LinearClamp;
                    DrawOverlays(gameTime);
                    Global.AlphaBlendTextures(FinalImage, OverlayImage);
                    break;
                case DebugModes.Light:
                    DrawOverlays(gameTime);
                    DrawScene(gameTime);
                    DrawLights(gameTime);
                    Global.ResetDeviceDefault();
                    Global.device.SetRenderTarget(FinalImage);
                    Global.RenderTexture(Global.lightTarget);
                    Global.AlphaBlendTextures(FinalImage, OverlayImage);
                    break;
                case DebugModes.Reflect:
                    DrawScene(gameTime);
                    DrawLights(gameTime);
                    Global.ResetDeviceDefault();
                    Global.device.SetRenderTarget(FinalImage);
                    Global.RenderTexture(Global.reflectTarget);
                    DrawOverlays(gameTime);
                    Global.AlphaBlendTextures(FinalImage, OverlayImage);
                    break;
                case DebugModes.NoTexture:
                    DrawNoTexture(gameTime);
                    DrawLights(gameTime);
                    Global.ResetDeviceDefault();
                    Global.device.SetRenderTarget(FinalImage);
                    CombineColorAndLight();
                    DrawOverlays(gameTime);
                    Global.AlphaBlendTextures(FinalImage, OverlayImage);
                    break;
            }
        }
        private void DrawNoTexture(GameTime gameTime)
        {
            Global.ViewProjection.SetValue(camera.viewProjection);
            Global.CameraPosition.SetValue(camera.position);
            Global.effect.CurrentTechnique = Global.NoTexture;

            Global.device.SetRenderTargets(Global.colorTarget, Global.normalTarget, Global.specularTarget, Global.depthTarget);
            Global.device.Clear(Color.Black);
            skybox.Draw(gameTime);
            base.Draw(gameTime);
        }
        private void DrawScene(GameTime gameTime)
        {
            Global.ViewProjection.SetValue(camera.viewProjection);
            Global.CameraPosition.SetValue(camera.position);
            Global.effect.CurrentTechnique = Global.Scene;

            Global.device.SetRenderTargets(Global.colorTarget, Global.normalTarget, Global.specularTarget, Global.depthTarget);
            Global.device.Clear(Color.Black);
            skybox.Draw(gameTime);
            base.Draw(gameTime);
        }
        private void DrawTransparencies(GameTime gameTime)
        {
            Global.ViewProjection.SetValue(camera.viewProjection);
            Global.CameraPosition.SetValue(camera.position);
            Global.effect.CurrentTechnique = Global.Black;

            Global.device.SetRenderTargets(Global.colorTarget, Global.normalTarget, Global.specularTarget, Global.depthTarget);
            Global.device.Clear(Color.Transparent);
            base.Draw(gameTime);
            Global.effect.CurrentTechnique = Global.Scene;
            Transparencies.Draw(gameTime);
        }
        private void DrawLights(GameTime gameTime)
        {
            Global.InvViewProjection.SetValue(Matrix.Invert(camera.viewProjection));
            Global.CloneTexture2D(Global.prevLightTarget, Global.blackImage);
            Global.CloneTexture2D(Global.prevReflectTarget, Global.blackImage);

            foreach (Light light in Lights)
            {
                if (light.state < 2) continue;
                //Render Shadow to Texture
                if (light.castsAShadow)
                {
                    Global.device.SetRenderTarget(Global.shadowTarget);
                    Global.LightViewProjection.SetValue(light.viewProjection);
                    Global.device.Clear(Color.Black);
                    Global.effect.CurrentTechnique = Global.Shadow;
                    base.DrawShadow(gameTime);
                }

                //Set Textures
                Global.device.SetRenderTargets(Global.lightTarget, Global.reflectTarget);
                Global.PreviousLightMap.SetValue(Global.prevLightTarget);
                Global.PreviousReflectMap.SetValue(Global.prevReflectTarget);
                Global.NormalMap.SetValue(Global.normalTarget);
                Global.DepthMap.SetValue(Global.depthTarget);
                Global.SpecularMap.SetValue(Global.specularTarget);
                Global.ShadowMap.SetValue(Global.shadowTarget);

                //Render Light
                Global.ResetDeviceDefault();
                Global.device.Clear(Color.Black);
                light.Draw(gameTime);
                Global.Render();

                Global.CloneTexture2D(Global.prevLightTarget, Global.lightTarget);
                Global.CloneTexture2D(Global.prevReflectTarget, Global.reflectTarget);
            }
        }
        private void CombineColorAndLight()
        {
            //Ambient Light
            Global.LightColor.SetValue(ambientLight.color.ToVector4());
            Global.LightIntensity.SetValue(ambientLight.intensity);

            Global.effect.CurrentTechnique = Global.Final;
            Global.ColorMap.SetValue(Global.colorTarget);
            Global.LightMap.SetValue(Global.lightTarget);
            Global.ReflectMap.SetValue(Global.reflectTarget);
            Global.HalfPixel.SetValue(Global.halfPixel);
            Global.MaterialColor.SetValue(Color.White.ToVector4());

            Global.ResetDeviceDefault();
            Global.device.Clear(Color.Black);
            Global.Render();
        }
        private void CombineColorAndLightTransparent()
        {
            //Ambient Light
            Global.LightColor.SetValue(ambientLight.color.ToVector4());
            Global.LightIntensity.SetValue(ambientLight.intensity);

            Global.effect.CurrentTechnique = Global.Final;
            Global.ColorMap.SetValue(Global.colorTarget);
            Global.LightMap.SetValue(Global.lightTarget);
            Global.ReflectMap.SetValue(Global.reflectTarget);
            Global.HalfPixel.SetValue(Global.halfPixel);
            Global.MaterialColor.SetValue(new Color(255,255,255,120).ToVector4());

            Global.ResetDeviceDefault();
            Global.device.Clear(Color.Black);
            Global.Render();
        }
        private void DrawOverlays(GameTime gameTime)
        {
            Global.device.SetRenderTarget(OverlayImage);
            Global.device.Clear(Color.Transparent);
            Global.effect.CurrentTechnique = Global.Basic;
            Overlays.Draw(gameTime);
        }
        public void ResetDevice()
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            Global.device.SamplerStates[0] = ss;
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            rs.FillMode = _wireFrame;
            Global.device.RasterizerState = rs;
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            Global.device.DepthStencilState = dss;
            Global.device.BlendState = BlendState.Opaque;
            Global.device.Clear(Color.SteelBlue);
        }
        #endregion
        #region Lists
        public override void Add(Object3D o)
        {
            o.dimension = this;
            base.Add(o);
        }
        public void AddOverlay(Object3D o)
        {
            o.dimension = this;
            Overlays.Add(o);
        }
        public void AddTransparent(Object3D o)
        {
            o.dimension = this;
            Transparencies.Add(o);
        }
        public override void Remove(object o)
        {
            base.Remove(o);
        }
        public void AddLight(Light light)
        {
            Lights.Add(light);
            light.dimension = this;
            light.parent = this;
        }
        public void RemoveLight(Light light)
        {
            Lights.Remove(light);
        }
        public void ClearLights()
        {
            Lights.Clear();
            AddLight(ambientLight);
        }
        #endregion
    }
}
