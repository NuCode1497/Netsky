using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.Diagnostics;
using Netsky_Engine.World;
namespace Netsky_Engine
{
    public class Game1 : Game
    {
        #region variables
        public GraphicsDeviceManager graphics;
        public enum GameState
        {
            WorldL, World, Pause
        }
        private GameState _state;
        int FPSFrames; //How many times Draw() has been called in the current second
        int frameRate;
        TimeSpan elapsedTime;
        //GUI
        public TextObject FPS;
        public World2D world2D;
        public World2D GUI;
        public BlindList VisualEffects;
        public TextObject WorldStatus;
        public GameCursor cursor;
        public Crosshairs crosshairs;
        //WORLD
        public World3D world3D;
        public Camera camera1;
        public Camera camera2;
        public LightDirectional dirLight1;
        public GenericModelObject zombie;
        public Map map;
        public GenericModelObject Cwing1;
        public GenericModelObject Acolyte1;
        public Orbiter Orbiter;
        public Orbiter Orbiter2;
        public Orbiter Orbiter3;
        public PointLight pointLight1;
        public SpotLight spotLight1;
        public SpotLight spotLight2;
        public SpotLight spotLight3;
        public SpotLight spotLight4;
        public Ball blueBall;
        public Ball ball2;
        public Ball goldBall;
        public Ball ball4;
        public Orientaid orientAid;
        public Orientaid orientAid2;
        public Orientaid orientAid3;
        public Orientaid orientAid4;
        public Arrow a1;
        public Arrow a2;
        public Arrow a3;
        public Arrow a4;
        public Arrow a5;
        public Arrow a6;
        public Arrow a7;
        public Arrow a8;
        public Arrow a9;
        public Arrow a10;
        //DEBUG
        float turboMod = 1f;
        float helAngle = 0;
        float helHeight = 5;
        Color[,] worldTextureData;
        float[,] depthData;
        #endregion
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Global.currentGame = this;

            TargetElapsedTime = TimeSpan.FromMilliseconds(7);
            //IsFixedTimeStep = false; //this will cause physics to desync
            //graphics.SynchronizeWithVerticalRetrace = false;
        }
        protected override void Initialize()
        {
            IsMouseVisible = false;
            Window.Title = "Netsky Engine";
            _state = GameState.WorldL;
            base.Initialize();
        }
        protected override void LoadContent()
        {
            Global.LoadContent();
            switch (state)
            {
                case GameState.WorldL:
                    LoadGUI();
                    LoadWorld();
                    break;
            }

            state = _state;
            FPSFrames = 0;
            frameRate = 0;
            elapsedTime = TimeSpan.Zero;
        }
        private void LoadGUI()
        {
            GUI = new World2D();
            GUI.LoadContent();
            GUI.setSize(Global.windowWidth, Global.windowHeight);
            GUI.position = new Vector2(Global.windowWidth / 2, Global.windowHeight / 2);
            GUI.state = World2D.STATE_BIRTH;
            GUI.layer = 0f;

            VisualEffects = new BlindList();
            VisualEffects.LoadContent();
            VisualEffects.parent = GUI;

            FPS = new TextObject();
            FPS.LoadContent();
            FPS.text = "";
            FPS.position.X = Global.windowWidth - 5;
            FPS.position.Y = 5;
            FPS.color = Color.Yellow;
            FPS.scale = 1f;
            FPS.state = TextObject.STATE_ALIGN_RIGHT;

            WorldStatus = new TextObject();
            WorldStatus.LoadContent();
            WorldStatus.font = Global.Verdana8;
            WorldStatus.text = "";
            WorldStatus.color = Color.Yellow;
            WorldStatus.scale = 1f;
            WorldStatus.state = TextObject.STATE_OUTLINE;

            cursor = new GameCursor();
            cursor.texture = Global.cursorImage;
            cursor.LoadContent();
            cursor.scale = 1f;
            cursor.state = GameCursor.STATE_POINT;

            crosshairs = new Crosshairs();
            crosshairs.LoadContent();
        }
        private void LoadWorld()
        {
            world3D = new World3D();
            world3D.LoadContent();

            camera1 = new Camera();
            camera1.LoadContent();
            camera1.position = new Vector3(0, 27, -22);
            camera1.Face(new Vector3(0, 26, -21));

            camera2 = new Camera();
            camera2.LoadContent();
            camera2.position = new Vector3(10, 10, -10);
            camera2.Face(new Vector3(0, 0, 0));
            camera2.minDepth = 1f;
            camera2.maxDepth = 50f;

            dirLight1 = new LightDirectional();
            dirLight1.LoadContent();

            zombie = new GenericModelObject();
            zombie.LoadContent();
            zombie.model = Global.zombieSceneModel;
            zombie.ColorMap = Global.zombieDiffuse;
            zombie.NormalMap = Global.zombieNormal;
            zombie.position = new Vector3(0, 0.5f, -20);
            zombie.angle.Y = MathHelper.ToRadians(-90);

            map = new Map();
            map.LoadContent();
            map.length = 10;
            map.width = 10;

            Cwing1 = new GenericModelObject();
            Cwing1.LoadContent();
            Cwing1.model = Global.ship1Model;
            Cwing1.ColorMap = Global.ship1Diffuse;
            Cwing1.NormalMap = Global.ship1Normal;
            Cwing1.SpecularMap = Global.ship1Specular;
            Cwing1.position = new Vector3(-52, 5f, 60);

            Acolyte1 = new GenericModelObject();
            Acolyte1.LoadContent();
            Acolyte1.model = Global.ship2Model;
            Acolyte1.ColorMap = Global.ship2Diffuse;
            Acolyte1.NormalMap = Global.ship2Normal;
            Acolyte1.SpecularMap = Global.ship2Specular;
            Acolyte1.position = new Vector3(-32, 5f, 60);

            Orbiter = new Orbiter();
            Orbiter.LoadContent();
            Orbiter.size = Vector3.One * 0.4f;

            Orbiter2 = new Orbiter();
            Orbiter2.LoadContent();
            Orbiter2.size = Vector3.One * 0.4f;

            Orbiter3 = new Orbiter();
            Orbiter3.LoadContent();
            Orbiter3.size = Vector3.One * 0.4f;

            pointLight1 = new PointLight();
            pointLight1.LoadContent();
            pointLight1.intensity = 1.5f;
            pointLight1.position = new Vector3(40, 5, 45);
            pointLight1.color = Color.Red;

            spotLight1 = new SpotLight();
            spotLight1.LoadContent();
            spotLight1.intensity = 2f;
            spotLight1.color = Color.White;
            spotLight1.intensity = 1.5f;
            spotLight1.position = new Vector3(10, 40, 30);
            spotLight1.Face(new Vector3(0f, -1, -0.7f));

            spotLight2 = new SpotLight();
            spotLight2.LoadContent();
            spotLight2.intensity = 2f;
            spotLight2.color = Color.Red;
            spotLight2.intensity = 1.5f;
            spotLight2.position = new Vector3(-60f, 35f, 75f);
            spotLight2.Face(new Vector3(-40f, 0f, 55f));

            spotLight3 = new SpotLight();
            spotLight3.LoadContent();
            spotLight3.intensity = 2f;
            spotLight3.color = Color.Blue;
            spotLight3.intensity = 1.5f;
            spotLight3.position = new Vector3(-20f, 35f, 75f);
            spotLight3.Face(new Vector3(-40f, 0f, 55f));

            spotLight4 = new SpotLight();
            spotLight4.LoadContent();
            spotLight4.intensity = 2f;
            spotLight4.color = Color.Green;
            spotLight4.intensity = 1.5f;
            spotLight4.position = new Vector3(-40f, 35f, 75f);
            spotLight4.Face(new Vector3(-40f, 0f, 45f));

            blueBall = new Ball();
            blueBall.LoadContent();
            blueBall.position = new Vector3(10, 15, 5);
            blueBall.friction = 1f;
            blueBall.Material.diffuseColor = Color.Blue.ToVector4();
            blueBall.Material.matteColor = Color.AliceBlue.ToVector4();
            blueBall.Material.shininess = 1f;
            blueBall.Material.ambient = 0.4f;

            ball2 = new Ball();
            ball2.LoadContent();
            ball2.position = new Vector3(0f, 1f, 0f);
            ball2.Material.diffuseColor = Color.DarkMagenta.ToVector4();
            ball2.Material.matteColor = Color.Magenta.ToVector4();
            ball2.Material.shininess = 1f;
            ball2.Material.ambient = 0.2f;

            ball4 = new Ball();
            ball4.LoadContent();
            ball4.position = new Vector3(0f, 3f, 0f);
            ball4.Material.diffuseColor = Color.Green.ToVector4();
            ball4.Material.matteColor = Color.GreenYellow.ToVector4();
            ball4.Material.shininess = 1f;
            ball4.Material.ambient = 0.2f;

            goldBall = new Ball();
            goldBall.LoadContent();
            goldBall.position = new Vector3(50f, 10f, 50f);
            goldBall.Material.diffuseColor = Color.Goldenrod.ToVector4();
            goldBall.Material.matteColor = Color.Gold.ToVector4();
            goldBall.Material.shininess = 2f;
            goldBall.size = Vector3.One * 10;

            orientAid = new Orientaid();
            orientAid.LoadContent();
            orientAid2 = new Orientaid();
            orientAid2.LoadContent();
            orientAid3 = new Orientaid();
            orientAid3.LoadContent();
            orientAid4 = new Orientaid();
            orientAid4.LoadContent();

            a1 = new Arrow();
            a1.LoadContent();
            a2 = new Arrow();
            a2.LoadContent();
            a3 = new Arrow();
            a3.LoadContent();
            a4 = new Arrow();
            a4.LoadContent();
            a5 = new Arrow();
            a5.LoadContent();
            a6 = new Arrow();
            a6.LoadContent();
            a7 = new Arrow();
            a7.LoadContent();
            a8 = new Arrow();
            a8.LoadContent();
            a9 = new Arrow();
            a9.LoadContent();
            a10 = new Arrow();
            a10.LoadContent();
        }
        protected override void UnloadContent()
        {
            world3D.UnloadContent();
            Global.UnloadContent();
        }
        protected override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case GameState.WorldL:
                    state = GameState.World;
                    world3D.Update(gameTime);
                    FrameRate(gameTime);
                    Status(gameTime);
                    GUI.Update(gameTime);
                    break;
                case GameState.World:
                    UpdateControls(gameTime);
                    if (Global.Pressed(Keys.Escape)) state = GameState.Pause;
                    Commands(gameTime);
                    world3D.Update(gameTime);
                    FrameRate(gameTime);
                    Status(gameTime);
                    GUI.Update(gameTime);
                    break;
                case GameState.Pause:
                    UpdateControls(gameTime);
                    if (Global.Pressed(Keys.Escape)) this.Exit();
                    if (Global.WindowClicked()) state = GameState.World;
                    FrameRate(gameTime);
                    Status(gameTime);
                    GUI.Update(gameTime);
                    DisplayTextureData();
                    break;
            }
            a9.position = Orbiter2.position;
        }
        public GameState state
        {
            get { return _state; }
            set
            {
                _state = value;
                switch (value)
                {
                    case GameState.WorldL:
                        GUI.Clear();
                        GUI.Add(FPS);
                        GUI.Add(WorldStatus);
                        GUI.Add(cursor);
                        //GUI.Add(crosshairs);
                        GUI.Sort();
                        world3D.Clear();
                        //world3D.Add(zombie);
                        world3D.Add(map);
                        world3D.Add(Cwing1);
                        world3D.Add(Acolyte1);
                        world3D.AddLight(dirLight1);
                        world3D.AddLight(spotLight1);
                        world3D.AddLight(spotLight2);
                        world3D.AddLight(spotLight3);
                        world3D.AddLight(spotLight4);
                        world3D.AddLight(pointLight1);
                        world3D.Add(blueBall);
                        world3D.AddLight(blueBall.Glow);
                        blueBall.state = Ball.STATE_GLOW_BOUNCE;
                        world3D.Add(Orbiter);
                        Orbiter.Orbit(blueBall, 10f, false, 0.2f);
                        world3D.Add(Orbiter2);
                        world3D.Add(Orbiter3);
                        Orbiter2.Orbit(Orbiter, 10f, true, 1f);
                        Orbiter3.Orbit(Orbiter2, 10f, false, 2f);
                        world3D.Add(goldBall);
                        world3D.Add(ball2);
                        world3D.Add(ball4);
                        world3D.AddOverlay(orientAid);
                        orientAid.parent = Orbiter3;
                        orientAid.size = Vector3.One * 12f;
                        world3D.AddOverlay(orientAid2);
                        orientAid2.parent = blueBall;
                        world3D.AddOverlay(orientAid3);
                        orientAid3.parent = Orbiter;
                        orientAid3.size = Vector3.One * 3f;
                        world3D.AddOverlay(orientAid4);
                        orientAid4.parent = Orbiter2;
                        orientAid4.size = Vector3.One * 6f;
                        //face static position from static position
                        world3D.Add(a1);
                        a1.position = new Vector3(-10, 10, 13);
                        a1.Face(new Vector3(-5, 5, 5));
                        a1.color = Color.Red;
                        //Face static position with offset from static position
                        world3D.Add(a3);
                        a3.position = new Vector3(-10, 10, 13);
                        a3.Face(new Vector3(-5, 5, 5), Vector3.One);
                        a3.color = Color.Purple;
                        //Face static position from moving position
                        world3D.Add(a2);
                        a2.parent = blueBall;
                        a2.Track(new Vector3(-5, 5, 5));
                        a2.color = Color.OrangeRed;
                        //face static position with offset from moving position
                        world3D.Add(a4);
                        a4.parent = blueBall;
                        a4.Track(new Vector3(-5, 5, 5), Vector3.One);
                        a4.color = Color.Orange;
                        //face static position from moving and rotating position
                        world3D.Add(a5);
                        a5.parent = Orbiter2;
                        a5.Track(new Vector3(-5, 5, 5));
                        a5.color = Color.Lime;
                        a5.size = Vector3.One * 3f;
                        //face static position with offset from moving and rotating position
                        world3D.Add(a6);
                        a6.parent = Orbiter2;
                        a6.Track(new Vector3(-5, 5, 5), Vector3.One);
                        a6.color = Color.Green;
                        a6.size = Vector3.One * 3f;
                        //face moving and rotating position from moving position
                        world3D.Add(a7);
                        a7.parent = blueBall;
                        a7.Track(Orbiter);
                        a7.color = Color.Tomato;
                        //face moving and rotating position from moving and rotating position
                        world3D.Add(a8);
                        a8.parent = Orbiter;
                        a8.Track(Orbiter3);
                        a8.color = Color.Yellow;
                        //face relative moving and rotating position from moving and rotating position
                        world3D.Add(a9);
                        a9.parent = Orbiter;
                        a9.TrackRelative(new Vector3(7.2f,-2.3f,-3.3f));
                        a9.color = Color.DeepPink;
                        //face moving position from static position
                        world3D.Add(a10);
                        a10.Track(Orbiter2);
                        a10.position = new Vector3(5, 5, -10);
                        a10.color = Color.CornflowerBlue;
                        world3D.Add(camera1);
                        world3D.Add(camera2);
                        world3D.AddTransparent(camera1.frustum);
                        world3D.AddTransparent(camera2.frustum);
                        world3D.camera = camera1;
                        break;
                    case GameState.World:
                        cursor.state = GameCursor.STATE_INACTIVE;
                        break;
                    case GameState.Pause:
                        cursor.state = GameCursor.STATE_ACTIVE;
                        worldTextureData = Global.TextureTo2DArray<Color>(world3D.FinalImage);
                        depthData = Global.TextureTo2DArray<float>(Global.depthTarget);
                        break;
                }
            }
        }
        private void Status(GameTime gameTime)
        {
            try
            {
                WorldStatus.text =
                    "G: " + state + " W: " + world3D.state + " UI: " + GUI.state +
                    "\n-- Camera1 --" +
                    "\nPos: " + format(camera1.position) +
                    "\nRot: " + format(camera1.spin) +
                    "\n-- Camera2 --" +
                    "\nPos: " + format(camera2.position) +
                    "\nRot: " + format(camera2.spin) +
                    "";
            }
            catch { }
            FPS.text = string.Format("{0} FPS", frameRate);
        }
        private void Commands(GameTime gameTime)
        {
            float time = gameTime.ElapsedGameTime.Milliseconds / 128f;

            if (Global.Pressed(Keys.Pause))
            {
                if (world3D.UpdateMode == World3D.UpdateModes.Freeze)
                    world3D.UpdateMode = World3D.UpdateModes.Normal;
                else world3D.UpdateMode = World3D.UpdateModes.Freeze;
            }
            if (Global.keyboardState.IsKeyDown(Keys.OemMinus))
            {
                world3D.timeDilation *= 0.99f;
            }
            if (Global.keyboardState.IsKeyDown(Keys.OemPlus))
            {
                world3D.timeDilation *= 1.01f;
            }
            if (Global.Pressed(Keys.D1))
            {
                world3D.camera.state = Camera.STATE_ACTIVE;
                world3D.camera = camera1;
                world3D.ControlQuake(camera1);
            }
            if (Global.Pressed(Keys.D2))
            {
                world3D.camera.state = Camera.STATE_ACTIVE;
                world3D.camera = camera2;
                world3D.ControlQuake(camera2);
            }
            if (Global.Pressed(Keys.D3))
            {
                world3D.camera.parent = Orbiter;
            }
            if (Global.Pressed(Keys.D4))
            {
                world3D.camera.parent = world3D;
            }
            if (Global.Pressed(Keys.D5))
            {
                world3D.camera.Track(ball2);
            }
            if (Global.keyboardState.IsKeyDown(Keys.Q))
            {
                world3D.keySensitivity *= 0.99f;
            }
            if (Global.keyboardState.IsKeyDown(Keys.E))
            {
                world3D.keySensitivity *= 1.01f;
            }
            if (Global.Pressed(Keys.F1))
            {
                if (world3D.UpdateMode == World3D.UpdateModes.Freeze)
                    world3D.UpdateMode = World3D.UpdateModes.Step;
                else world3D.UpdateMode = World3D.UpdateModes.Normal;
            }
            if (Global.Pressed(Keys.F5))
            {
                if (world3D.DebugMode == World3D.DebugModes.Split)
                    world3D.DebugMode = World3D.DebugModes.None;
                else world3D.DebugMode = World3D.DebugModes.Split;
            }
            if (Global.Pressed(Keys.F6))
            {
                if (world3D.DebugMode == World3D.DebugModes.NoTexture)
                    world3D.DebugMode = World3D.DebugModes.None;
                else world3D.DebugMode = World3D.DebugModes.NoTexture;
            }
            if (Global.Pressed(Keys.F7))
            {
                world3D.wireFrame = !world3D.wireFrame;
            }
            if (Global.Pressed(Keys.OemTilde))
            {
                camera1.Pan(spotLight1, 0.5f, 3000);
            }
            if (Global.keyboardState.IsKeyDown(Keys.Z))
            {
                world3D.camera.fieldOfView += .1f * time;
            }
            if (Global.keyboardState.IsKeyDown(Keys.X))
            {
                world3D.camera.fieldOfView -= .1f * time;
            }
            if (Global.Pressed(Keys.C))
            {
                world3D.ToggleCameraLight();
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            FPSFrames++;
            switch (state)
            {
                case GameState.Pause:
                    Global.device.SetRenderTarget(null);
                    Global.RenderTexture(world3D.FinalImage);
                    DrawGUI(gameTime);
                    break;
                case GameState.World:
                case GameState.WorldL:
                    world3D.Draw(gameTime);
                    Global.device.SetRenderTarget(null);
                    Global.RenderTexture(world3D.FinalImage);
                    DrawGUI(gameTime);
                    break;
            }
        }
        private void DrawGUI(GameTime gameTime)
        {
            //To draw visual effects at a specific layer, rather than on top of everything,
            //create a draw manager method that looks at object layers and switches blendstates as needed.
            //game objects
            Global.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
            GUI.Draw(gameTime);
            Global.spriteBatch.End();
            //visual effects
            Global.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
            VisualEffects.Draw(gameTime);
            Global.spriteBatch.End();
            //UI Elements
            Global.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
            //cursor.Draw(gameTime);
            Global.spriteBatch.End();
        }
        #region Utility Functions
        private void FrameRate(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = FPSFrames;
                FPSFrames = 0;
            }
        }
        private void DisplayTextureData()
        {
            MouseState ms = Mouse.GetState();
            if (Global.device.Viewport.Bounds.Contains(new Point(ms.X, ms.Y)))
            {
                WorldStatus.text +=
                    "\nTexture data:\n" +
                    worldTextureData[ms.X, ms.Y] +
                    "\nDepth data: " +
                    depthData[ms.X, ms.Y];
            }
        }
        private void UpdateControls(GameTime gameTime)
        {
            if (!IsActive) return;
            Global.oldKeyboardState = Global.keyboardState;
            Global.keyboardState = Keyboard.GetState();
            Global.prevMouseState = Global.mouseState;
            Global.mouseState = Mouse.GetState();
            Global.mouseScrollChange = Global.mouseState.ScrollWheelValue - Global.prevMouseState.ScrollWheelValue;
            cursor.Update(gameTime);
        }
        private List<string> ParseStream(string s)
        {
            //returns a list of the lines read from a text file
            try
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    String line;
                    List<string> list = new List<string>();
                    while ((line = sr.ReadLine()) != null)
                    {
                        list.Add(line);
                    }
                    return list;
                }
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
        private string format(Vector2 v2)
        {
            string result = "";
            try { result = "{" + v2.X.ToString("F") + "," + v2.Y.ToString("F"); }
            catch { }
            return result;
        }
        private string format(Vector3 v3)
        {
            string result = "";
            try { result = "{" + v3.X.ToString("F") + "," + v3.Y.ToString("F") + "," + v3.Z.ToString("F") + "}"; }
            catch { }
            return result;
        }
        private string format(Vector4 v4)
        {
            string result = "";
            try { result = "{" + v4.X.ToString("F") + "," + v4.Y.ToString("F") + "," + v4.Z.ToString("F") + "," + v4.W.ToString("F") + "}"; }
            catch { }
            return result;
        }
        private string format(Quaternion rot)
        {
            string result = "";
            try
            {
                result = "{" + rot.X.ToString("F") + "," + rot.Y.ToString("F") + "," + rot.Z.ToString("F") + "," + rot.W.ToString("F") + "}";
            }
            catch { }
            return result;
        }
        private string format(Matrix mat)
        {
            string result = "";
            try
            {
                result =
                    "\n  {" + mat.M11.ToString("F") + "," + mat.M12.ToString("F") + "," + mat.M13.ToString("F") + "," + mat.M14.ToString("F") + "}" +
                    "\n  {" + mat.M21.ToString("F") + "," + mat.M22.ToString("F") + "," + mat.M23.ToString("F") + "," + mat.M24.ToString("F") + "}" +
                    "\n  {" + mat.M31.ToString("F") + "," + mat.M32.ToString("F") + "," + mat.M33.ToString("F") + "," + mat.M34.ToString("F") + "}" +
                    "\n  {" + mat.M41.ToString("F") + "," + mat.M42.ToString("F") + "," + mat.M43.ToString("F") + "," + mat.M44.ToString("F") + "}";
            }
            catch { }
            return result;
        }
        private string format(Color color)
        {
            string result = "";
            try
            {
                result = "{" + color.R.ToString("F") + "," + color.G.ToString("F") + "," + color.B.ToString("F") + "," + color.A.ToString("F") + "}";
            }
            catch { }
            return result;
        }
        #endregion
    }
}
