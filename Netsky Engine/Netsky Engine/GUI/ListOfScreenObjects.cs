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

namespace Netsky_Engine
{
    /// <summary>
    /// ByLayer is used to sort ScreenObjects by layer to simulate 3D
    /// </summary>
    public class ByLayer : IComparer<ScreenObject>
    {
        public int Compare(ScreenObject so1, ScreenObject so2)
        {
            return so1.relativeLayer.CompareTo(so2.relativeLayer);
        }
    }
    public abstract class ListOfScreenObjects : ScreenObject, IEnumerable<ScreenObject>
    {
        protected List<ScreenObject> ScreenObjectList = new List<ScreenObject>();
        protected List<ScreenObject> DeleteList = new List<ScreenObject>();
        public IEnumerator<ScreenObject> GetEnumerator()
        {
            return ScreenObjectList.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public virtual void Clear()
        {
            ScreenObjectList.Clear();
        }
        public override void LoadContent()
        {
            base.LoadContent();
            state = STATE_ACTIVE;
        }
        public override void UnloadContent()
        {
            foreach (ScreenObject so in ScreenObjectList)
            {
                so.UnloadContent();
            }
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            BringOutYourDead();
            //UpdateKinetics(gameTime); //should be done before base.Update();
            base.Update(gameTime);
            foreach (ScreenObject so in ScreenObjectList)
            {
                so.Update(gameTime);
            }
        }
        public void Sort()
        {
            ScreenObjectList.Sort(new ByLayer());
        }
        public override void Draw(GameTime gameTime)
        {
            foreach (ScreenObject so in ScreenObjectList)
            {
                so.Draw(gameTime);
            }
        }
        public virtual void Add(ScreenObject so)
        {
            if (ScreenObjectList.Contains(so)) return;
            so.parent = this;
            so.OnDeath += new FiredEvent(Remove);
            ScreenObjectList.Add(so);
        }
        public virtual void AddList(ListOfScreenObjects list)
        {
            foreach (ScreenObject so in list.ScreenObjectList)
            {
                Add(so);
            }
        }
        protected virtual void Remove(object so)
        {

            //This needs a seperate list because we can't delete while the list is iterating
            //occurs when an object in the list dies
            DeleteList.Add((ScreenObject)so);
        }
        protected virtual void RemoveList(object list)
        {
            //This needs a seperate list because we can't delete while the list is iterating
            //occurs when an object in the list dies
            DeleteList.AddRange(((ListOfScreenObjects)list).ScreenObjectList);
        }
        protected virtual void BringOutYourDead()
        {
            //clear out any inactive objects
            if (DeleteList.Count > 0)
            {
                ScreenObjectList = (List<ScreenObject>)(ScreenObjectList.Except(DeleteList)).ToList();
                DeleteList.Clear();
            }
        }
    }
}
