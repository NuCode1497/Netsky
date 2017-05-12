using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Netsky_Engine.World
{
    /// <summary>
    /// An object that always faces a certain position.
    /// </summary>
    public abstract class DirectedObject : Object3D
    {
        public const int STATE_FACE_REL = 5;
        public const int STATE_FACE_ABS = 6;
        public const int STATE_TRACK_OBJ = 7;
        public const int STATE_TRACK_ABS = 8;
        public const int STATE_TRACK_REL = 9;
        public const int STATE_FACE_OBJ = 10;
        /// <summary>
        /// The absolute position this object is facing.
        /// </summary>
        public Vector3 facingPosition;
        /// <summary>
        /// The vector originating from the object, to its facing position plus the offset.
        /// </summary>
        public Vector3 viewVector;
        /// <summary>
        /// The object to track.
        /// </summary>
        public Object3D focus;
        /// <summary>
        /// An offset added to the facing position.
        /// </summary>
        public Vector3 offset;
        public override void LoadContent()
        {
            base.LoadContent();
            facingPosition = Vector3.Zero;
            viewVector = Vector3.Forward;
            offset = Vector3.Zero;
            focus = Object3D.Allfather;
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_ACTIVE:
                    UpdateKinetics(gameTime);
                    UpdateWorldMatrix();
                    break;
                case STATE_FACE_REL:
                    UpdateFaceRelative(gameTime);
                    state = previousState;
                    break;
                case STATE_TRACK_REL: 
                    UpdateFaceRelative(gameTime);
                    break;
                case STATE_FACE_ABS:
                    UpdateFaceAbsolute(gameTime);
                    state = previousState;
                    break;
                case STATE_TRACK_ABS:
                    UpdateFaceAbsolute(gameTime);
                    break;
                case STATE_TRACK_OBJ:
                    UpdateFaceObject(gameTime);
                    break;
                case STATE_FACE_OBJ:
                    UpdateFaceObject(gameTime);
                    state = previousState;
                    break;
            }
        }
        protected virtual void UpdateFaceObject(GameTime gameTime)
        {
            facingPosition = focus.translation;
            UpdateFaceAbsolute(gameTime);
        }
        protected virtual void UpdateFaceAbsolute(GameTime gameTime)
        {
            UpdateTranslation(gameTime);
            UpdateScale(gameTime);
            //absolute rotation of viewVector here requires updated absolute translation
            UpdateWorldMatrix();
            viewVector = facingPosition + offset - translation;
            Face();
            rotation = spin;
            //update matrix again to include custom rotation
            matrix = Matrix.CreateScale(scale) *
                     Matrix.CreateFromQuaternion(rotation) *
                     Matrix.CreateTranslation(translation);
        }
        protected virtual void UpdateFaceRelative(GameTime gameTime)
        {
            UpdateTranslation(gameTime);
            UpdateScale(gameTime);
            //Rotate relative viewVector after updating relative translation
            viewVector = facingPosition + offset - position;
            Face();
            UpdateWorldMatrix();
        }
        /// <summary>
        /// Update angle and spin to face down the view vector with a fixed Z rotation
        /// </summary>
        protected virtual void Face()
        {
            //Get the angle that is between the viewVector and the (z,x) plane
            angle.X = (float)Math.Atan2(viewVector.Y, new Vector2(viewVector.Z, viewVector.X).Length());
            //Get the angle of rotation around the y axis
            angle.Y = (float)Math.PI + (float)Math.Atan2(viewVector.X, viewVector.Z);
            //Don't do a barrel roll
            angle.Z = 0;

            spin = Quaternion.CreateFromAxisAngle(Vector3.Forward, angle.Z) *
                   Quaternion.CreateFromAxisAngle(Vector3.Up, angle.Y) *
                   Quaternion.CreateFromAxisAngle(Vector3.Right, angle.X);
        }
        /// <summary>
        /// Face a position relative to the parent object.
        /// </summary>
        /// <param name="position">The relative position to face.</param>
        public virtual void FaceRelative(Vector3 position)
        {
            offset = Vector3.Zero;
            facingPosition = position;
            state = STATE_FACE_REL;
        }
        /// <summary>
        /// Face a position relative to the parent object with a relative offset.
        /// </summary>
        /// <param name="position">The relative position to face.</param>
        /// <param name="offset">Face this relative position from the object.</param>
        public virtual void FaceRelative(Vector3 position, Vector3 offset)
        {
            this.offset = offset;
            facingPosition = position;
            state = STATE_FACE_REL;
        }
        /// <summary>
        /// Constantly face a position relative to the parent object.
        /// </summary>
        /// <param name="position">The relative position to face.</param>
        public virtual void TrackRelative(Vector3 position)
        {
            offset = Vector3.Zero;
            facingPosition = position;
            state = STATE_TRACK_REL;
        }
        /// <summary>
        /// Constantly face a position relative to the parent object with a relative offset.
        /// </summary>
        /// <param name="position">The relative position to face.</param>
        /// <param name="offset">Face this position from the object.</param>
        public virtual void TrackRelative(Vector3 position, Vector3 offset)
        {
            this.offset = offset;
            facingPosition = position;
            state = STATE_TRACK_REL;
        }
        /// <summary>
        /// Face an absolute position.
        /// </summary>
        /// <param name="position">The absolute position to face.</param>
        public virtual void Face(Vector3 position)
        {
            offset = Vector3.Zero;
            facingPosition = position;
            state = STATE_FACE_ABS;
        }
        /// <summary>
        /// Face an absolute position with an offset.
        /// </summary>
        /// <param name="position">The absolute position to face.</param>
        /// <param name="offset">Face this absolute translation from the given position.</param>
        public virtual void Face(Vector3 position, Vector3 offset)
        {
            this.offset = offset;
            facingPosition = position;
            state = STATE_FACE_ABS;
        }
        /// <summary>
        /// Constantly face a specific position.
        /// </summary>
        /// <param name="position">The position to face.</param>
        public virtual void Track(Vector3 position)
        {
            offset = Vector3.Zero;
            facingPosition = position;
            state = STATE_TRACK_ABS;
        }
        /// <summary>
        /// Constantly face a specific position with an offset.
        /// </summary>
        /// <param name="position">The position to face.</param>
        /// <param name="offset">Face this position from the object.</param>
        public virtual void Track(Vector3 position, Vector3 offset)
        {
            this.offset = offset;
            facingPosition = position;
            state = STATE_TRACK_ABS;
        }
        /// <summary>
        /// Face the absolute position of an object.
        /// </summary>
        /// <param name="o">The object to face.</param>
        public virtual void Face(Object3D o)
        {
            offset = Vector3.Zero;
            focus = o;
            state = STATE_FACE_OBJ;
        }
        /// <summary>
        /// Face the absolute position of an object with an offset.
        /// </summary>
        /// <param name="o">The object to face.</param>
        /// <param name="offset">Face this absolute position from the object.</param>
        public virtual void Face(Object3D o, Vector3 offset)
        {
            this.offset = offset;
            focus = o;
            state = STATE_FACE_OBJ;
        }
        /// <summary>
        /// Constantly face the given object with no roll.
        /// </summary>
        /// <param name="o">The object to face.</param>
        public virtual void Track(Object3D o)
        {
            offset = Vector3.Zero;
            focus = o;
            state = STATE_TRACK_OBJ;
        }
        /// <summary>
        /// Constantly face an offset from the given object.
        /// with no roll.
        /// </summary>
        /// <param name="o">The object to face.</param>
        /// <param name="offset">The offset from the object.</param>
        public virtual void Track(Object3D o, Vector3 offset)
        {
            this.offset = offset;
            focus = o;
            state = STATE_TRACK_OBJ;
        }
    }
}
