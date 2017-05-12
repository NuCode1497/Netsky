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
    public abstract class Object3D : DynamicObject, GameObject
    {
        //state
        public const int STATE_INACTIVE = 0;
        public const int STATE_DEATH = 1;
        public const int STATE_BIRTH = 2;
        public const int STATE_ACTIVE = 3;
        public const int STATE_CONTROL = 4;
        private int _state;
        public int previousState;
        public virtual int state
        {
            get { return _state; }
            set { previousState = _state; _state = value; }
        }
        //kinetics
        public Matrix matrix;
        /// <summary>
        /// Absolute Scale
        /// </summary>
        public Vector3 scale;
        /// <summary>
        /// Relative Scale
        /// </summary>
        public Vector3 size;
        public Vector3 sizeVelocity;
        public Vector3 sizeAcceleration;
        public float sizeFriction;
        public float sizeFCritVel;
        /// <summary>
        /// Absolute Rotation
        /// </summary>
        public Quaternion rotation;
        /// <summary>
        /// Relative Rotation
        /// </summary>
        public Quaternion spin;
        /// <summary>
        /// Euler rotations around parent axis
        /// </summary>
        public Vector3 angle; //radians
        public Vector3 angularVelocity;
        public Vector3 angularAcceleration;
        public float angularFriction;
        public float angularFCritVel;
        /// <summary>
        /// Absolute Translation
        /// </summary>
        public Vector3 translation;
        /// <summary>
        /// Relative Translation
        /// </summary>
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 acceleration;
        public float friction;
        public float fCritVel;
        public float radius;
        public float mass;
        //public float momentOfInertia;  //TODO: advanced rotational physics; Implement Mass points and rigid bodies
        //public Vector3 centerOfMass;
        //public Shape[] massPoints;
        public bool hitWall;
        static public bool collision = false;
        public Action<Object3D, Object3D> CollisionMethod; //sets collision
        //Relativity
        public virtual World3D dimension
        {
            get;
            set;
        }
        public virtual Object3D parent
        {
            get;
            set;
        }
        private class AllFather : Object3D
        {
            public AllFather()
            {
                LoadContent();
            }
        }
        public static Object3D Allfather = new AllFather();
        //Events
        public delegate void FiredEvent(object sender);
        public FiredEvent OnDeath;
        public virtual void Death(object sender)
        {
            if (OnDeath != null)
            {
                OnDeath(this);
            }
        }
        //Dynamic properties
        int _ID = Global.generateID();
        public int ID
        {
            get
            {
                return _ID;
            }
            set { }
        }
        Dictionary<string, object> dynamicProperties = new Dictionary<string, object>();
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // If you try to get a value of a property 
            // not defined in the class, this method is called.

            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name.ToLower();
            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return dynamicProperties.TryGetValue(name, out result);
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // If you try to set a value of a property that is
            // not defined in the class, this method is called.

            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            dynamicProperties[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
        //Functions
        public virtual void LoadContent()
        {
            state = STATE_INACTIVE;
            matrix = Matrix.Identity;
            size = Vector3.One;
            size = Vector3.One;
            sizeVelocity = Vector3.Zero;
            sizeAcceleration = Vector3.Zero;
            sizeFriction = .98f;
            sizeFCritVel = .001f;
            spin = Quaternion.Identity;
            angle = Vector3.Zero;
            angularVelocity = Vector3.Zero;
            angularAcceleration = Vector3.Zero;
            angularFriction = .98f;
            angularFCritVel = .001f;
            position = Vector3.Zero;
            position = Vector3.Zero;
            velocity = Vector3.Zero;
            acceleration = Vector3.Zero;
            friction = .98f;
            fCritVel = .001f;
            mass = 1f;
            radius = 100f;
            hitWall = false;
            parent = Allfather;
            CollisionMethod = ObjectsCollideByRadius;
        }
        public virtual void UnloadContent()
        {
        }
        public virtual void Update(GameTime gameTime)
        {
            //abstract classes must not have state switches
            //non-abstract classes must have state switches
        }
        /// <summary>
        /// Update Scale, Rotation, Position, and Relativity.
        /// </summary>
        /// <param name="gameTime">Time since last update.</param>
        public virtual void UpdateKinetics(GameTime gameTime)
        {
            UpdateTranslation(gameTime);
            UpdateRotation(gameTime);
            UpdateScale(gameTime);
        }
        public virtual void UpdateScale(GameTime gameTime)
        {
            sizeVelocity += sizeAcceleration * dimension.time;
            sizeAcceleration = Vector3.Zero;
            sizeVelocity *= sizeFriction;
            if (sizeVelocity.Length() < sizeFCritVel) sizeVelocity = Vector3.Zero;
            size += sizeVelocity * dimension.time;
        }
        public virtual void UpdateRotation(GameTime gameTime)
        {
            angularVelocity += angularAcceleration * dimension.time;
            angularAcceleration = Vector3.Zero;
            angularVelocity *= angularFriction;
            if (angularVelocity.Length() < angularFCritVel) angularVelocity = Vector3.Zero;
            angle += angularVelocity * dimension.time;
            spin = Quaternion.CreateFromAxisAngle(Vector3.Forward, angle.Z) *
                   Quaternion.CreateFromAxisAngle(Vector3.Up, angle.Y) *
                   Quaternion.CreateFromAxisAngle(Vector3.Right, angle.X);
        }
        public virtual void UpdateTranslation(GameTime gameTime)
        {
            velocity += acceleration * dimension.time;
            acceleration = Vector3.Zero;
            velocity *= friction;
            if (velocity.Length() < fCritVel) velocity = Vector3.Zero; //static friction
            position += velocity * dimension.time;
        }
        public virtual void UpdateWorldMatrix()
        {
            matrix = Matrix.CreateScale(size) *
                     Matrix.CreateFromQuaternion(spin) *
                     Matrix.CreateTranslation(position) * 
                     parent.matrix;
            matrix.Decompose(out scale, out rotation, out translation);
        }
        public void Halt()
        {
            velocity = Vector3.Zero;
            acceleration = Vector3.Zero;
            sizeVelocity = Vector3.Zero;
            sizeAcceleration = Vector3.Zero;
            angularVelocity = Vector3.Zero;
            angularAcceleration = Vector3.Zero;
        }
        public virtual void CollideWithGround(float minHeight)
        {
            if (position.Y < minHeight)
            {
                position.Y = minHeight;
                velocity.Y = 0;
            }
        }
        public virtual void BounceOffGround(float minHeight)
        {
            if (position.Y < minHeight)
            {
                position.Y = minHeight;
                velocity.Y = -velocity.Y;
            }
        }
        public virtual void Draw(GameTime gameTime)
        {
            //abstract classes must not have state switches here
            //non-abstract classes must have state switches here
        }
        public virtual void DrawShadow(GameTime gameTime)
        {
            Draw(gameTime);
        }
        //Utility
        static public void ObjectsCollideByRadius(Object3D ob1, Object3D ob2)
        {
            //use bool collision to determine if there was a collision
            float dist = Vector3.Distance(ob1.position, ob2.position) - ob1.radius - ob2.radius;
            if (dist < 0)
            {
                collision = true;
                return;
            }
            collision = false;
        }
        static public bool operator ==(Object3D a, Object3D b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }
            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            // Return true if the fields match:
            return a.ID == b.ID;
        }
        static public bool operator !=(Object3D a, Object3D b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Object3D so = obj as Object3D;
            if ((System.Object)so == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (ID == so.ID);
        }
        public override int GetHashCode()
        {
            return ID;
        }
        public virtual Object3D Clone()
        {
            throw new NotImplementedException();
        }
    }
}
