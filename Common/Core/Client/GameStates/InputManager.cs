using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Orbit.Core.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Client.GameStates
{
    public class MouseMoveEvent
    {
        public Vector2 Position {get; set;}
        public Vector2 Change { get; set; }

        public MouseMoveEvent(Vector2 position, Vector2 change)
        {
            Position = position;
            Change = change;
        }
    }

    public enum MouseButtons
    {
        Left,
        Right,
        Middle
    }

    public class InputManager : IUpdatable
    {
        private MouseState previousState;
        private Dictionary<Keys, IKeyPressListener> keyActions;
        private List<IMouseMoveListener> moveListeners;
        private List<IMouseClickListener> clickListeners;

        public InputManager()
        {
            keyActions = new Dictionary<Keys, IKeyPressListener>();
            moveListeners = new List<IMouseMoveListener>();
            clickListeners = new List<IMouseClickListener>();
            previousState = Mouse.GetState();
        }

        public void Update(float tpf)
        {
            ProccessKeyboard();
            ProccessMouse();
        }

        private void ProccessKeyboard()
        {
            KeyboardState state = Keyboard.GetState();
            bool down = false;
            bool up = false;
            foreach (KeyValuePair<Keys, IKeyPressListener> values in keyActions)
            {
                down = state.IsKeyDown(values.Key);
                up = state.IsKeyUp(values.Key);
                if (up || down)
                    values.Value.OnKeyEvent(values.Key, down);
            }
        }

        private void ProccessMouse()
        {
            MouseState currentState = Mouse.GetState();
            int changeX = currentState.X - previousState.X;
            int changeY = currentState.Y - previousState.Y;

            int currentX = currentState.X;// +136;
            int currentY = currentState.Y;// +197;

            if (changeX > 0 || changeY > 0)
            {
                MouseMoveEvent e = new MouseMoveEvent(new Vector2(currentX, currentY), new Vector2(changeX, changeY));
                moveListeners.ForEach(l => l.OnMouseMove(e));
            }

            if(currentState.LeftButton == ButtonState.Pressed && previousState.LeftButton == ButtonState.Released)
                clickListeners.ForEach(l => l.OnCanvasClick(MouseButtons.Left, new Vector2(currentX, currentY), true));
            else if (currentState.LeftButton == ButtonState.Released && previousState.LeftButton == ButtonState.Pressed)
                clickListeners.ForEach(l => l.OnCanvasClick(MouseButtons.Left, new Vector2(currentX, currentY), false));

            if (currentState.MiddleButton == ButtonState.Pressed && previousState.MiddleButton == ButtonState.Released)
                clickListeners.ForEach(l => l.OnCanvasClick(MouseButtons.Middle, new Vector2(currentX, currentY), true));
            else if (currentState.MiddleButton == ButtonState.Released && previousState.MiddleButton == ButtonState.Pressed)
                clickListeners.ForEach(l => l.OnCanvasClick(MouseButtons.Middle, new Vector2(currentX, currentY), false));

            if (currentState.RightButton == ButtonState.Pressed && previousState.RightButton == ButtonState.Released)
                clickListeners.ForEach(l => l.OnCanvasClick(MouseButtons.Right, new Vector2(currentX, currentY), true));
            else if (currentState.RightButton == ButtonState.Released && previousState.RightButton == ButtonState.Pressed)
                clickListeners.ForEach(l => l.OnCanvasClick(MouseButtons.Right, new Vector2(currentX, currentY), false));

            previousState = currentState;
        }

        public void AddKeyListener(Keys key, IKeyPressListener listener)
        {
            keyActions.Add(key, listener);
        }

        public void AddClickListener(IMouseClickListener listener)
        {
            clickListeners.Add(listener);
        }

        public void AddMoveListener(IMouseMoveListener listener)
        {
            moveListeners.Add(listener);
        }

        public void Clear()
        {
            keyActions.Clear();
            clickListeners.Clear();
            moveListeners.Clear();
        }
    }
}
