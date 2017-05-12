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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
namespace Netsky_Engine.World
{
    //This is similar to the ListOfScreenObjects but in true three dimensions
    public abstract class ListOfObject3Ds : Object3D, IEnumerable<Object3D>
    {
        protected List<Object3D> Object3DList = new List<Object3D>();
        protected List<Object3D> DeleteList = new List<Object3D>();

        public IEnumerator<Object3D> GetEnumerator()
        {
            return Object3DList.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public virtual void Clear()
        {
            Object3DList.Clear();
        }
        public override void LoadContent()
        {
            base.LoadContent();
            state = STATE_ACTIVE;
        }
        public override void UnloadContent()
        {
            foreach (Object3D o in Object3DList)
            {
                o.UnloadContent();
            }
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            BringOutYourDead();
            foreach (Object3D so in Object3DList)
            {
                so.Update(gameTime);
            }
        }
        public override void Draw(GameTime gameTime)
        {
            foreach (Object3D o in Object3DList)
            {
                o.Draw(gameTime);
            }
        }
        public override void DrawShadow(GameTime gameTime)
        {
            foreach (Object3D o in Object3DList)
            {
                o.DrawShadow(gameTime);
            }
        }
        public virtual void Add(Object3D o)
        {
            if (Object3DList.Contains(o)) return;
            o.parent = this;
            o.OnDeath += new FiredEvent(Remove);
            Object3DList.Add(o);
        }
        public virtual void AddList(ListOfObject3Ds list)
        {
            foreach (Object3D o in list.Object3DList)
            {
                Add(o);
            }
        }
        public virtual void Remove(object o)
        {
            //This needs a seperate list because we can't delete while the list is iterating
            //occurs when an object in the list dies
            DeleteList.Add((Object3D)o);
        }
        public virtual void RemoveList(object list)
        {
            //This needs a seperate list because we can't delete while the list is iterating
            //occurs when an object in the list dies
            DeleteList.AddRange(((ListOfObject3Ds)list).Object3DList);
        }
        protected virtual void BringOutYourDead()
        {
            //clear out any inactive objects
            if (DeleteList.Count > 0)
            {
                Object3DList = (List<Object3D>)(Object3DList.Except(DeleteList)).ToList();
                DeleteList.Clear();
            }
        }
    }
}
