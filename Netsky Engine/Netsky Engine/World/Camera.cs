using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Netsky_Engine.World
{
    public class Camera : DirectedObject
    {
        //Properties
        public Matrix view;
        public Matrix projection;
        public Matrix viewProjection;
        public float maxDepth
        {
            get { return _maxDepth; }
            set { _maxDepth = MathHelper.Clamp(value, 100, 5000); }
        }
        float _maxDepth;
        public float minDepth
        {
            get { return _minDepth; }
            set { _minDepth = MathHelper.Clamp(value, 0.1f, 99); }
        }
        float _minDepth;
        /// <summary>
        /// controls the angle of the camera cone, i.e. the size of the far rectangle.
        /// </summary>
        public float fieldOfView
        {
            get { return _FOV; }
            set { _FOV = MathHelper.Clamp(value, 0.01f, 3.14f); }
        }
        float _FOV;
        /// <summary>
        /// Aspect Ratio.
        /// </summary>
        public float viewPort;
        public Vector3 relativePosition;
        public Vector3 relativeFacing;
        public Vector3 up;
        //state
        public const int STATE_FOCUS = 11;
        public const int STATE_TRANSITION = 12;
        public const int STATE_PAN_CAMERA = 13;
        public const int STATE_PAN_POSITION = 14;
        public const int STATE_PEEK = 15;
        //transition
        float rate;
        Camera tc;
        Vector3 finalPosition;
        Vector3 finalDirection;
        Quaternion finalRotation;
        Stopwatch watch;
        int timeStop;
        //Model
        public GenericModelObject model;
        public Frustum frustum;
        public float transparency;
        private int[] indices;
        private IndexBuffer indexBuffer;
        //OVERLAY
        public RenderTarget2D FinalImage { get; set; }

        public override void LoadContent()
        {
            base.LoadContent();
            state = STATE_BIRTH;
            maxDepth = 2000;
            minDepth = 0.001f;
            fieldOfView = MathHelper.PiOver4;
            viewPort = Global.device.Viewport.AspectRatio;

            transparency = 0.3f;

            model = new GenericModelObject();
            model.LoadContent();
            model.model = Global.cameraModel;
            model.parent = this;
            model.Material.ambient = 0.5f;
            model.Material.diffuse = 0.5f;
            model.Material.shininess = 0.1f;
            model.Material.matteColor = Color.Green.ToVector4()/10;
            model.Material.diffuseColor = Color.Green.ToVector4();
            model.Material.diffuseColor.W = transparency;

            frustum = new Frustum();
            frustum.LoadContent();
            frustum.parent = this;

            friction = 0.985f;

            watch = new Stopwatch();
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_DEATH:
                case STATE_INACTIVE:
                    break;
                case STATE_BIRTH:
                    state = STATE_ACTIVE;
                    break;
                default:
                    base.Update(gameTime);
                    break;
                case STATE_PEEK:
                    Peek();
                    break;
                case STATE_PAN_CAMERA:
                    CameraPan(gameTime);
                    UpdateCamera();
                    break;
                case STATE_PAN_POSITION:
                    PositionPan(gameTime);
                    break;
                case STATE_TRANSITION:
                    Transition(gameTime);
                    break;
                case STATE_ACTIVE:
                    UpdateKinetics(gameTime);
                    UpdateWorldMatrix();
                    facingPosition = Vector3.Transform(Vector3.Forward, rotation) + translation;
                    UpdateCamera();
                    UpdateComponents();
                    break;
                case STATE_CONTROL:
                    UpdateKinetics(gameTime);
                    CollideWithGround(0.5f);
                    UpdateWorldMatrix();
                    facingPosition = Vector3.Transform(Vector3.Forward, rotation) + translation;
                    UpdateCamera();
                    UpdateComponents();
                    break;
            }
        }
        private void UpdateComponents()
        {
            model.matrix = matrix;
            frustum.UpdateComponents(viewProjection);
        }
        #region Camera Functions
        /// <summary>
        /// Show the view of another camera for one frame.
        /// </summary>
        /// <param name="camera">The other camera</param>
        public void Peek(Camera camera)
        {
            state = STATE_PEEK;
            viewProjection = camera.viewProjection;
        }
        /// <summary>
        /// Pans the camera to another camera.
        /// </summary>
        /// <param name="camera">The camera where the pan ends.</param>
        /// <param name="rate">The speed of the pan.</param>
        public void Pan(Camera camera, float rate, int time)
        {
            Halt();
            state = STATE_PAN_CAMERA;
            this.tc = camera;
            this.rate = rate;
            this.timeStop = time;
            watch.Restart();
        }
        /// <summary>
        /// Pans the camera to a position.
        /// </summary>
        /// <param name="position">The camera will end the pan here.</param>
        /// <param name="rate">The speed of the pan.</param>
        public void Pan(Vector3 position, Quaternion rotation, Vector3 direction, float rate, int time)
        {
            Halt();
            state = STATE_PAN_POSITION;
            this.finalPosition = position;
            this.finalRotation = rotation;
            this.finalDirection = direction;
            this.rate = rate;
            this.timeStop = time;
            watch.Restart();
        }
        /// <summary>
        /// Pans the camera to another camera, then switches over to the other camera.
        /// </summary>
        /// <param name="camera">The camera that will be the new camera.</param>
        /// <param name="rate">The speed of the pan.</param>
        public void TransitionTo(Camera camera, float rate, int time)
        {
            Halt();
            state = STATE_TRANSITION;
            this.tc = camera;
            this.rate = rate;
            this.timeStop = time;
            watch.Restart();
        }
        /// <summary>
        /// Copies the properties of another camera.
        /// </summary>
        /// <param name="camera">The other camera</param>
        public void Mimic(Camera camera)
        {
            facingPosition = camera.facingPosition;
            position = camera.position;
            view = camera.view;
            projection = camera.projection;
            viewProjection = camera.viewProjection;
            maxDepth = camera.maxDepth;
            minDepth = camera.minDepth;
            fieldOfView = camera.fieldOfView;
            viewPort = camera.viewPort;
            relativePosition = camera.relativePosition;
            relativeFacing = camera.relativeFacing;
            up = camera.up;
            angle = camera.angle;
            spin = camera.spin;
        } //move to directed object
        void Peek()
        {
            state = previousState;
        }
        void CameraPan(GameTime gameTime)
        {
            if (watch.ElapsedMilliseconds > timeStop)
            {
                watch.Reset();
                state = previousState;
                position = tc.position;
                facingPosition = tc.facingPosition;
            }
            else
            {
                float time = gameTime.ElapsedGameTime.Milliseconds / 128f;
                time = rate * time;
                position = Vector3.Lerp(position, tc.position, time);
                facingPosition = Vector3.Lerp(facingPosition, tc.facingPosition, time);
            }
        }
        void PositionPan(GameTime gameTime)
        {
            if (watch.ElapsedMilliseconds > timeStop)
            {
                watch.Reset();
                state = previousState;
                position = finalPosition;
                facingPosition = finalDirection;
            }
            else
            {
                float time = gameTime.ElapsedGameTime.Milliseconds / 128f;
                time = rate * time;
                position = Vector3.Lerp(position, finalPosition, time);
                facingPosition = Vector3.Lerp(facingPosition, finalDirection, time);
            }
        }
        void Transition(GameTime gameTime)
        {
            if (watch.ElapsedMilliseconds > timeStop)
            {
                watch.Reset();
                state = STATE_ACTIVE;
                dimension.camera = tc;
            }
            else
            {
                CameraPan(gameTime);
            }
        }
        protected virtual void UpdateCameraRelative()
        {
            up = Vector3.Transform(Vector3.Up, spin);
            view = Matrix.CreateLookAt(position, facingPosition, up);
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, viewPort, minDepth, maxDepth);
            viewProjection = view * projection;
        }
        protected virtual void UpdateCamera()
        {
            up = Vector3.Transform(Vector3.Up, rotation);
            view = Matrix.CreateLookAt(translation, facingPosition, up);
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, viewPort, minDepth, maxDepth);
            viewProjection = view * projection;
        }
        #endregion
        public override void Draw(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_INACTIVE:
                case STATE_DEATH:
                case STATE_BIRTH:
                case STATE_CONTROL:
                    break;
                default:
                    model.Draw(gameTime);
                    frustum.DrawOutline();
                    break;
            }
        }
    }
}
