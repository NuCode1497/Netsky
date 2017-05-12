using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Netsky_Engine
{
    public abstract class PoolOfScreenObjects : ListOfScreenObjects
    {
        public int numberOfObjects;
        public int lastObject;
        public List<ScreenObject> ActivatedList = new List<ScreenObject>();
        public GhostObject ghost = new GhostObject();

        public override void LoadContent()
        {
            base.LoadContent();
            lastObject = 0;
        }
        protected override void BringOutYourDead()
        {
            if (DeleteList.Count > 0)
            {
                ActivatedList = (List<ScreenObject>)(ActivatedList.Except(DeleteList)).ToList();
                DeleteList.Clear();
            }
        }
        public ScreenObject GetNextObject()
        {
            lastObject = (lastObject + 1) % ScreenObjectList.Count;
            return ScreenObjectList[lastObject];
        }
        public ScreenObject GetNextInactiveObject()
        {
            for (int i = 0; i < ScreenObjectList.Count; i++)
            {
                lastObject = (lastObject + 1) % ScreenObjectList.Count();
                if (ScreenObjectList[lastObject].state == STATE_INACTIVE)
                {
                    return ScreenObjectList[lastObject];
                }
            }
            return ghost;
        }
        public virtual void KillThemAll()
        {
            foreach (ScreenObject so in ScreenObjectList)
            {
                so.state = ScreenObject.STATE_DEATH;
            }
        }
    }
}
