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

namespace Netsky_Engine
{
    public abstract class ScreenObject : DynamicObject, GameObject
    {
        //diagnostics
        public Stopwatch stopwatch = new Stopwatch();
        public TimeSpan test1;
        public TimeSpan test2;
        public TimeSpan test3;
        public TimeSpan test4;
        //state
        public const int STATE_INACTIVE = 0;
        public const int STATE_DEATH = 1;
        public const int STATE_BIRTH = 2;
        public const int STATE_ACTIVE = 3;
        private int _state;
        public int stateFrames;
        public virtual int state
        {
            get { return _state; }
            set { _state = value; stateFrames = 0; }
        }
        //kinetics
        public Matrix matrix;
        public float scale;
        public float radius;
        public Vector2 origin;
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 acceleration;
        public float angularAcceleration;
        public float angularVelocity;
        public float angle;
        public float mass;
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public float friction;
        public float fCritVel;
        //game interaction
        public Rectangle rectangle;
        public ScreenObject parent;
        public float layer;
        public float relativeLayer;
        public Vector2 relativePosition;
        public float relativeScale;
        public float relativeAngle;
        public Color color;
        public bool hitWall;
        public SpriteEffects spriteEffects;
        static public bool collision = false;
        public Action<ScreenObject, ScreenObject> CollisionMethod; //sets collision
        //events
        public delegate void FiredEvent(object sender);
        public FiredEvent OnDeath;
        public virtual void Death(object sender)
        {
            if (OnDeath != null)
            {
                OnDeath(this);
            }
        }
        //dynamic properties
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
        //functions
        public virtual void LoadContent()
        {
            state = STATE_INACTIVE;

            origin = Vector2.Zero;
            relativePosition = Vector2.Zero;
            position = Vector2.Zero;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            angularAcceleration = 0;
            angularVelocity = 0;
            angle = 0;
            mass = 1f;
            friction = 1f;
            fCritVel = 0f;

            scale = 1f;
            relativeScale = 1f;
            radius = 100f;
            Width = Height = (int)radius;
            rectangle = new Rectangle(0, 0, Width, Height);
            parent = null;
            layer = 0.5f;
            color = Color.White;
            hitWall = false;
            spriteEffects = SpriteEffects.None;
            CollisionMethod = ObjectsCollideByRectangle;
        }
        public virtual void UnloadContent()
        { }
        public virtual void LoadCopy(ScreenObject so)
        {
            state = so.state;

            origin = so.origin;
            relativePosition = so.relativePosition;
            position = so.position;
            velocity = so.velocity;
            acceleration = so.acceleration;
            angularAcceleration = so.angularAcceleration;
            angularVelocity = so.angularVelocity;
            angle = so.angle;
            mass = so.mass;
            friction = so.friction;
            fCritVel = so.fCritVel;

            scale = so.scale;
            relativeScale = so.relativeScale;
            radius = so.radius;
            Width = Height = so.Width;
            rectangle = so.rectangle;
            parent = so.parent;
            layer = so.layer;
            color = so.color;
            hitWall = so.hitWall;
            spriteEffects = so.spriteEffects;
            CollisionMethod = so.CollisionMethod;
        }
        public virtual void Update(GameTime gameTime)
        {
            //UpdateKinetics(gameTime);
            stateFrames++;
        }
        public virtual void Draw(GameTime gameTime)
        {
            //abstract classes must not have state switches here
            //non-abstract classes must have state switches here
        }
        public virtual void drawSelection(GameTime gameTime)
        {
        }
        public virtual void drawHighlight(GameTime gameTime)
        {
        }
        public virtual void drawPoke(GameTime gameTime)
        {
        }
        public virtual void BounceInsideParent()
        {
            hitWall = false;
            if (position.X > parent.Width - radius)
            {
                position.X = parent.Width - radius;
                velocity.X = Math.Abs(velocity.X) * -1;
                hitWall = true;
            }
            if (position.X < 0 + radius)
            {
                position.X = 0 + radius;
                velocity.X = Math.Abs(velocity.X);
                hitWall = true;
            }

            if (position.Y > parent.Height - radius)
            {
                position.Y = parent.Height - radius;
                velocity.Y = Math.Abs(velocity.Y) * -1;
                hitWall = true;
            }
            if (position.Y < 0 + radius)
            {
                position.Y = 0 + radius;
                velocity.Y = Math.Abs(velocity.Y);
                hitWall = true;
            }
        }
        public virtual void UpdateKinetics(GameTime gameTime)
        {
            //abstract classes must not have state switches
            //non-abstract classes must have state switches
            angularVelocity += angularAcceleration;
            angle += angularVelocity;
            velocity += acceleration;
            velocity *= friction;
            if (velocity.Length() < fCritVel) velocity = Vector2.Zero; //static friction
            position += velocity;

            //adjust relativity properties
            if (parent == null)
            {
                relativeScale = scale;
                relativePosition = position;
                relativeAngle = angle;
                relativeLayer = layer;
            }
            else
            {
                setScreenLayer();
                relativeAngle = angle + parent.relativeAngle;
                relativeScale = scale * parent.relativeScale;
                relativePosition = Vector2.Transform(position, parent.matrix);
            }

            //The information necessary for relativity to this object as it appears on screen
            matrix = Matrix.CreateTranslation(-origin.X, -origin.Y, 0) *
                     Matrix.CreateRotationZ(relativeAngle) *
                     Matrix.CreateScale(relativeScale) *
                     Matrix.CreateTranslation(relativePosition.X, relativePosition.Y, 0);
        }
        private void setScreenLayer()
        {
            //The actual layer for this object will be an amalgamation of its parents layers.
            //An object with layer .2 with a parent with layer .4 with a parent with layer .3
            //will have the layer .342
            ScreenObject pa = parent;
            relativeLayer = layer;
            do
            {
                relativeLayer = pa.layer + relativeLayer * .1f;
                pa = pa.parent;
            } while (pa != null);
        }
        public Vector2 getRelativeMouse()
        {
            return Vector2.Transform(new Vector2(Global.mouseState.X, Global.mouseState.Y),
                Matrix.Invert(matrix));
        }
        private float pulser = 1f;
        private bool set = false;
        private float oscale;
        public void pulsate()
        {
            if (!set)
            {
                oscale = scale;
                set = true;
            }
            pulser += .1f;
            scale = oscale * (float)(.2 * Math.Sin(pulser) + 1);
        }
        static public void ObjectsCollideByRectangle(ScreenObject ob1, ScreenObject ob2)
        {
            //use bool collision to determine if there was a collision
            if (ob1.rectangle.Intersects(ob2.rectangle))
            {
                collision = true;
                return;
            }
            collision = false;
        }
        static public void ObjectsCollideByRadius(ScreenObject ob1, ScreenObject ob2)
        {
            //use bool collision to determine if there was a collision
            float dist = Vector2.Distance(ob1.position, ob2.position) - ob1.radius - ob2.radius;
            if (dist < 0)
            {
                collision = true;
                return;
            }
            collision = false;
        }

        static public bool operator ==(ScreenObject a, ScreenObject b)
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
        static public bool operator !=(ScreenObject a, ScreenObject b)
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
            ScreenObject so = obj as ScreenObject;
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
    }
}
