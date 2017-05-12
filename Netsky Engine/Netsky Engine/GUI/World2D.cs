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
    //Creates an interactive, self-contained plane of existance for a set of objects.
    //Objects in the Map have no action on the cursor, the frame, or anything outside of the Map.
    //The current object under the cursor can be found with mouseOverObject, however it may be better
    //to construct listeners in the case that a Map contains another Map.
    public class World2D : ListOfScreenObjects
    {
        #region variables
        #region states
        public const int STATE_NOSELECT = 4;
        public const int STATE_HIGHLIGHT = 5;
        public const int STATE_FOCUS = 6;
        public const int STATE_FOCUS2 = 13;
        public const int STATE_SEL_HIGH = 7;
        public const int STATE_SELECT = 8;
        public const int STATE_DESELECT = 9;
        public const int STATE_SELECT_PAN = 10;
        public const int STATE_PAN = 11;
        public const int STATE_DRAG = 12;
        //actions
        public Action NoSelectMethod;   //These are customizable states.
        public Action HighlightMethod;  //Choose the appropriate method
        public Action FocusMethod;      //depending on the object type.
        public Action SelHighMethod;
        public Action SelectMethod;
        public Action DeselectMethod;
        public Action SelectPanMethod;
        public Action PanMethod;
        public Action DragMethod;
        //supported types
        public enum SupportedSelectionTypes
        {
            Standard,
            Button,
            Icon,
            None,
        }
        #endregion
        #region cursor
        public GameCursor cursor;
        public dynamic ghost; //instead of using null, use a does-nothing object
        //mouse state
        Vector2 prevMouse;
        public dynamic mouseOverObject; //set iff mouse over object. null otherwise.
        public dynamic selection; //set when mouse clicked over object. null when mouse clicked outside.
        #endregion
        public Frame frame;
        #endregion
        public void setSize(int width, int height)
        {
            origin = new Vector2(width / 2, height / 2);
            radius = ((float)(Math.Sqrt(height * height + width + width) / 2));
            rectangle = new Rectangle(0, 0, width, height);
            Width = width;
            Height = height;
            setFrame(width, height);
        }
        public void setSize(int width, int height, Color c)
        {
            origin = new Vector2(width / 2, height / 2);
            radius = ((float)(Math.Sqrt(height * height + width + width) / 2));
            rectangle = new Rectangle(0, 0, width, height);
            Width = width;
            Height = height;
            setFrame(width, height, c);
        }
        private void setFrame(int width, int height)
        {
            frame = new Frame();
            frame.texture = SpriteObject.CreateTexture(width, height, Color.Black);
            frame.LoadContent();
            frame.origin = Vector2.Zero;
            frame.layer = 0;
            frame.parent = this;
            frame.hlColor = frame.hlOverColor;
            frame.state = Frame.STATE_ACTIVE;
        }
        private void setFrame(int width, int height, Color c)
        {
            frame = new Frame();
            frame.texture = SpriteObject.CreateTexture(width, height, c);
            frame.LoadContent();
            frame.origin = Vector2.Zero;
            frame.layer = 0;
            frame.parent = this;
            frame.hlColor = frame.hlOverColor;
            frame.state = Frame.STATE_ACTIVE;
        }
        public override void LoadContent()
        {
            base.LoadContent();
            Sort();
            #region states
            NoSelectMethod = StandardNoselect;
            HighlightMethod = StandardHighlight;
            FocusMethod = IconFocus;
            SelHighMethod = StandardSelHigh;
            SelectMethod = StandardSelect;
            DeselectMethod = StandardDeselect;
            SelectPanMethod = StandardSelectPan;
            PanMethod = StandardPan;
            DragMethod = IconDrag;
            #endregion
            #region cursor
            cursor = Global.currentGame.cursor;
            ghost = new GhostObject();
            ghost.LoadContent();
            mouseOverObject = ghost;
            selection = ghost;
            prevMouse = Vector2.Zero;
            #endregion
            setFrame(Global.windowWidth, Global.windowHeight);
            relativePosition = position;
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                #region standard states
                case STATE_INACTIVE:
                    break;
                case STATE_DEATH:
                    base.Clear();
                    state = STATE_INACTIVE;
                    break;
                case STATE_BIRTH:
                    state = STATE_ACTIVE;
                    UpdateKinetics(gameTime);
                    base.Update(gameTime);
                    frame.UpdateKinetics(gameTime);
                    frame.Update(gameTime);
                    break;
                case STATE_ACTIVE:
                    //Non-interactive
                    base.Update(gameTime);
                    break;
                #endregion
                #region selection states
                case STATE_NOSELECT:
                    NoSelectMethod();
                    UpdateKinetics(gameTime);
                    base.Update(gameTime);
                    frame.UpdateKinetics(gameTime);
                    frame.Update(gameTime);
                    break;
                case STATE_HIGHLIGHT:
                    HighlightMethod();
                    UpdateKinetics(gameTime);
                    base.Update(gameTime);
                    frame.UpdateKinetics(gameTime);
                    frame.Update(gameTime);
                    break;
                case STATE_FOCUS:
                    Focus();
                    UpdateKinetics(gameTime);
                    base.Update(gameTime);
                    frame.UpdateKinetics(gameTime);
                    frame.Update(gameTime);
                    break;
                case STATE_FOCUS2:
                    FocusMethod();
                    UpdateKinetics(gameTime);
                    base.Update(gameTime);
                    frame.UpdateKinetics(gameTime);
                    frame.Update(gameTime);
                    break;
                case STATE_SEL_HIGH:
                    SelHighMethod();
                    UpdateKinetics(gameTime);
                    base.Update(gameTime);
                    frame.UpdateKinetics(gameTime);
                    frame.Update(gameTime);
                    break;
                case STATE_SELECT:
                    SelectMethod();
                    UpdateKinetics(gameTime);
                    base.Update(gameTime);
                    frame.UpdateKinetics(gameTime);
                    frame.Update(gameTime);
                    break;
                case STATE_DESELECT:
                    DeselectMethod();
                    UpdateKinetics(gameTime);
                    base.Update(gameTime);
                    frame.UpdateKinetics(gameTime);
                    frame.Update(gameTime);
                    break;
                case STATE_SELECT_PAN:
                    SelectPanMethod();
                    UpdateKinetics(gameTime);
                    base.Update(gameTime);
                    frame.UpdateKinetics(gameTime);
                    frame.Update(gameTime);
                    break;
                case STATE_PAN:
                    PanMethod();
                    UpdateKinetics(gameTime);
                    base.Update(gameTime);
                    frame.UpdateKinetics(gameTime);
                    frame.Update(gameTime);
                    break;
                case STATE_DRAG:
                    DragMethod();
                    UpdateKinetics(gameTime);
                    base.Update(gameTime);
                    frame.UpdateKinetics(gameTime);
                    frame.Update(gameTime);
                    break;
                #endregion
            }
        }
        public override void Draw(GameTime gameTime)
        {
            switch (state)
            {
                case STATE_INACTIVE:
                    break;
                case STATE_DEATH:
                    break;
                case STATE_BIRTH:
                    break;
                case STATE_ACTIVE:
                    base.Draw(gameTime);
                    break;
                case STATE_NOSELECT:
                    base.Draw(gameTime);
                    frame.Draw(gameTime);
                    break;
                case STATE_HIGHLIGHT:
                    base.Draw(gameTime);
                    frame.Draw(gameTime);
                    mouseOverObject.drawHighlight(gameTime);
                    break;
                case STATE_FOCUS2:
                case STATE_FOCUS:
                    base.Draw(gameTime);
                    frame.Draw(gameTime);
                    selection.drawPoke(gameTime);
                    break;
                case STATE_SEL_HIGH:
                    base.Draw(gameTime);
                    frame.Draw(gameTime);
                    mouseOverObject.drawHighlight(gameTime);
                    selection.drawSelection(gameTime);
                    break;
                case STATE_SELECT:
                    base.Draw(gameTime);
                    frame.Draw(gameTime);
                    selection.drawSelection(gameTime);
                    break;
                case STATE_DESELECT:
                    base.Draw(gameTime);
                    frame.Draw(gameTime);
                    selection.drawSelection(gameTime);
                    break;
                case STATE_SELECT_PAN:
                    base.Draw(gameTime);
                    frame.Draw(gameTime);
                    selection.drawSelection(gameTime);
                    break;
                case STATE_PAN:
                    base.Draw(gameTime);
                    frame.Draw(gameTime);
                    break;
                case STATE_DRAG:
                    base.Draw(gameTime);
                    frame.Draw(gameTime);
                    selection.drawSelection(gameTime);
                    break;
            }
        }
        public void Add(dynamic so)
        {
            if (!(so is ScreenObject)) return;
            base.Add((ScreenObject)so);
            //assign the selection type for faster identification
            if (so is Player)
            {
                so.selectionType = SupportedSelectionTypes.Icon;
            }
            else
            {
                so.selectionType = SupportedSelectionTypes.None;
            }
        }
        public bool cursorOverObject(ScreenObject so)
        {
            //use the ScreenObject's variable method CollisionMethod
            so.CollisionMethod(cursor, so);
            return ScreenObject.collision;
        }
        public void setMouseOverObject()
        {
            //Find the first object (by layer if sorted) under the cursor
            //and stop immediately
            foreach (ScreenObject so in ScreenObjectList)
            {
                if (cursorOverObject(so))
                {
                    mouseOverObject = so;
                    return;
                }
            }
            mouseOverObject = ghost;
        }
        protected void Zoom()
        {
            float differ;
            if (Global.mouseScrollChange != 0)
            {
                differ = (Global.mouseScrollChange / 120) * .02f;
                scale += differ * scale;
            }
        }
        public void DragObject(ScreenObject so)
        {
            Vector2 currentMouse = so.parent.getRelativeMouse();
            so.position += currentMouse - prevMouse;
            prevMouse = so.parent.getRelativeMouse();
        }

        #region Custom State Methods
        //to do
        //Standard
        //Icon
        //MultiIcon
        //Unit
        //MultiUnit
        protected void StandardNoselect()
        {
            Zoom();
            //see if the mouse is over an object
            setMouseOverObject();
            if (mouseOverObject != ghost)
            {
                //the mouse is over an object
                //highlight
                state = STATE_HIGHLIGHT;
                //check for a mouse click
                if (Global.mouseState.LeftButton == ButtonState.Pressed)
                {
                    //The mouse is clicking on an object.
                    //This is a good place to select objects or perform actions.
                    selection = mouseOverObject;
                    state = STATE_FOCUS;
                }
            }
            else
            {
                //there is nothing under the mouse
                //check for a mouse click
                if (Global.mouseState.LeftButton == ButtonState.Pressed)
                {
                    state = STATE_PAN;
                    //prepare for pan
                    prevMouse = parent.getRelativeMouse();
                }
            }
        }
        protected void StandardHighlight()
        {
            Zoom();
            //see if the mouse is over an object
            setMouseOverObject();
            if (mouseOverObject != ghost)
            {
                //the mouse is over an object
                //highlight
                //check for a mouse click
                if (Global.mouseState.LeftButton == ButtonState.Pressed)
                {
                    //The mouse is clicking on an object.
                    //This is a good place to select objects or perform actions.
                    //This is a good place to modify the Actions of this map depending on
                    //  the type of the object selected
                    selection = mouseOverObject;
                    state = STATE_FOCUS;
                    prevMouse = getRelativeMouse();
                }
            }
            else
            {
                state = STATE_NOSELECT;
            }
        }
        private void Focus()
        {
            switch ((SupportedSelectionTypes)selection.selectionType)
            {
                case SupportedSelectionTypes.Standard:
                    FocusMethod = StandardFocus;
                    break;
                case SupportedSelectionTypes.Icon:
                    FocusMethod = IconFocus;
                    break;
                case SupportedSelectionTypes.None:
                    FocusMethod = NoFocus;
                    break;
            }
            FocusMethod();
            state = STATE_FOCUS2;
        }
        protected void StandardFocus()
        {
            if (cursorOverObject(selection))
            {
                //mouse is still pressed over object
                //check for mouse release
                if (Global.mouseState.LeftButton == ButtonState.Released)
                {
                    state = STATE_SEL_HIGH;
                    //Perform focus action when button released.
                    //For buttons, this is where to put the button action.
                    //If you want instant action buttons, put it in StandardHighlight instead.
                    //For draggable objects, check for mouse movement here and change state to drag.

                }
            }
            else
            {
                //mouse has left object but is still pressed
                //check for mouse release
                if (Global.mouseState.LeftButton == ButtonState.Released)
                {
                    //cancel focus
                    state = STATE_NOSELECT;
                }
            }
        }
        protected void StandardSelHigh()
        {
            Zoom();
            //see if the mouse is over an object
            setMouseOverObject();
            if (mouseOverObject != ghost)
            {
                //the mouse is over an object
                //highlight
                //check for a mouse click
                if (Global.mouseState.LeftButton == ButtonState.Pressed)
                {
                    //The mouse is clicking on an object.
                    //This is a good place to select objects or perform actions.
                    selection = mouseOverObject;
                    state = STATE_FOCUS;
                    prevMouse = getRelativeMouse();
                }
            }
            else
            {
                //there is nothing under the mouse
                state = STATE_SELECT;
            }
        }
        protected void StandardSelect()
        {
            Zoom();
            //see if the mouse is over an object
            setMouseOverObject();
            if (mouseOverObject != ghost)
            {
                //the mouse is over an object
                //highlight
                state = STATE_SEL_HIGH;
                //check for a mouse click
                if (Global.mouseState.LeftButton == ButtonState.Pressed)
                {
                    //The mouse is clicking on an object.
                    //This is a good place to select objects or perform actions.
                    selection = mouseOverObject;
                    state = STATE_FOCUS;
                    prevMouse = getRelativeMouse();
                }
            }
            else
            {
                //there is nothing under the mouse
                //check for a mouse click
                if (Global.mouseState.LeftButton == ButtonState.Pressed)
                {
                    state = STATE_DESELECT;
                    //prepare for pan
                    prevMouse = parent.getRelativeMouse();
                }
            }
        }
        protected void StandardDeselect()
        {
            //check for movement
            Vector2 currentMouse = parent.getRelativeMouse();
            if (prevMouse.Equals(currentMouse))
            {
                //no movement
                //check for click
                if (Global.mouseState.LeftButton == ButtonState.Released)
                {
                    //mouse released and no movement
                    //deselect
                    state = STATE_NOSELECT;
                    selection = ghost;
                }
            }
            else
            {
                //movement detected
                //keep selection and pan
                state = STATE_SELECT_PAN;
            }
        }
        protected void StandardSelectPan()
        {
            //ignore mouseovers
            //move the map by dragging it with the mouse
            //check for a mouse click
            if (Global.mouseState.LeftButton == ButtonState.Pressed)
            {
                //mouse is pressed
                //move map
                DragObject(this);
            }
            else
            {
                //mouse released
                state = STATE_SELECT;
            }
        }
        protected void StandardPan()
        {
            //ignore mouseovers
            //move the map by dragging it with the mouse
            //check for a mouse click
            if (Global.mouseState.LeftButton == ButtonState.Pressed)
            {
                //mouse is pressed
                //move map
                DragObject(this);
            }
            else
            {
                //mouse released
                state = STATE_NOSELECT;
            }
        }
        protected void StaticNoselect()
        {
            //see if the mouse is over an object
            setMouseOverObject();
            if (mouseOverObject != ghost)
            {
                //the mouse is over an object
                //highlight
                state = STATE_HIGHLIGHT;
                //check for a mouse click
                if (Global.mouseState.LeftButton == ButtonState.Pressed)
                {
                    //The mouse is clicking on an object.
                    //This is a good place to select objects or perform actions.
                    selection = mouseOverObject;
                    state = STATE_FOCUS;
                }
            }
        }
        protected void StaticHighlight()
        {
            //see if the mouse is over an object
            setMouseOverObject();
            if (mouseOverObject != ghost)
            {
                //the mouse is over an object
                //highlight
                //check for a mouse click
                if (Global.mouseState.LeftButton == ButtonState.Pressed)
                {
                    //The mouse is clicking on an object.
                    selection = mouseOverObject;
                    state = STATE_FOCUS;
                    prevMouse = getRelativeMouse();
                }
            }
            else
            {
                state = STATE_NOSELECT;
            }
        }
        protected void StaticSelHigh()
        {
            //see if the mouse is over an object
            setMouseOverObject();
            if (mouseOverObject != ghost)
            {
                //the mouse is over an object
                //highlight
                //check for a mouse click
                if (Global.mouseState.LeftButton == ButtonState.Pressed)
                {
                    //The mouse is clicking on an object.
                    selection = mouseOverObject;
                    state = STATE_FOCUS;
                    prevMouse = getRelativeMouse();
                }
            }
            else
            {
                //there is nothing under the mouse
                state = STATE_SELECT;
            }
        }
        protected void StaticSelect()
        {
            //see if the mouse is over an object
            setMouseOverObject();
            if (mouseOverObject != ghost)
            {
                //the mouse is over an object
                //highlight
                state = STATE_SEL_HIGH;
                //check for a mouse click
                if (Global.mouseState.LeftButton == ButtonState.Pressed)
                {
                    //The mouse is clicking on an object.
                    //This is a good place to select objects or perform actions.
                    selection = mouseOverObject;
                    state = STATE_FOCUS;
                    prevMouse = getRelativeMouse();
                }
            }
        }
        protected void IconFocus()
        {
            if (cursorOverObject(selection))
            {
                //mouse is still pressed over object
                //check for mouse release
                if (Global.mouseState.LeftButton == ButtonState.Released)
                {
                    state = STATE_SEL_HIGH;
                }
                else
                {
                    //check for movement past threshold
                    Vector2 currentMouse = getRelativeMouse();
                    if (Vector2.Distance(prevMouse, currentMouse) > 1)
                    {
                        state = STATE_DRAG;
                    }
                }
            }
            else
            {
                //mouse has left object but is still pressed
                //check for mouse release
                if (Global.mouseState.LeftButton == ButtonState.Released)
                {
                    //cancel focus
                    state = STATE_NOSELECT;
                }
            }
        }
        protected void IconDrag()
        {
            //ignore mouseovers
            //move the icon by dragging it with the mouse
            //check for a mouse click
            if (Global.mouseState.LeftButton == ButtonState.Pressed)
            {
                //mouse is pressed
                DragObject(selection);
            }
            else
            {
                //mouse released
                state = STATE_SEL_HIGH;
            }
        }
        protected void NoFocus()
        {
            if (Global.mouseState.LeftButton == ButtonState.Released)
            {
                //cancel focus
                state = STATE_NOSELECT;
            }
        }
        #endregion
    }
}
