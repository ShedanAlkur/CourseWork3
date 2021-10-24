using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    partial class GameMain
    {
        public class GameInput
        {
            private List<Key> KeysDown;
            private List<Key> KeysDownLast;
            private List<MouseButton> ButtonsDown;
            private List<MouseButton> ButtonsDownLast;
            private Vector2 clickPositionLast;
            public float DeltaPrecise { get; private set; }
            public Vector2 ClickPositionLast
            {
                get
                {
                    return clickPositionLast;
                }
            }

            public GameInput(GameWindow game)
            {
                KeysDown = new List<Key>();
                KeysDownLast = new List<Key>();
                ButtonsDown = new List<MouseButton>();
                ButtonsDownLast = new List<MouseButton>();

                game.KeyDown += Game_KeyDown;
                game.KeyUp += Game_KeyUp;
                game.MouseDown += Game_MouseDown;
                game.MouseUp += Game_MouseUp;
                game.MouseWheel += Game_MouseWheel;

                DeltaPrecise = 0;
            }

            private void Game_MouseWheel(object sender, MouseWheelEventArgs e)
            {
                DeltaPrecise = e.DeltaPrecise;
            }
            private void Game_MouseUp(object sender, MouseButtonEventArgs e)
            {
                while (ButtonsDown.Contains(e.Button))
                    ButtonsDown.Remove(e.Button);
            }
            private void Game_MouseDown(object sender, MouseButtonEventArgs e)
            {
                ButtonsDown.Add(e.Button);
                clickPositionLast = new Vector2(e.X, e.Y);
            }
            private void Game_KeyUp(object sender, KeyboardKeyEventArgs e)
            {
                while (KeysDown.Contains(e.Key))
                    KeysDown.Remove(e.Key);
            }
            private void Game_KeyDown(object sender, KeyboardKeyEventArgs e)
            {
                KeysDown.Add(e.Key);
            }

            public void Update()
            {
                DeltaPrecise = 0;
                KeysDownLast = new List<Key>(KeysDown);
                ButtonsDownLast = new List<MouseButton>(ButtonsDown);
            }

            public bool KeyPress(Key key)
            {
                return (KeysDown.Contains(key) && !KeysDownLast.Contains(key));
            }

            public bool KeyRelease(Key key)
            {
                return (!KeysDown.Contains(key) && KeysDownLast.Contains(key));
            }

            public bool KeyDown(Key key)
            {
                return KeysDown.Contains(key);
            }

            public bool MousePress(MouseButton button)
            {
                return (ButtonsDown.Contains(button) && !ButtonsDownLast.Contains(button));
            }

            public bool MouseRelease(MouseButton button)
            {
                return (!ButtonsDown.Contains(button) && ButtonsDownLast.Contains(button));
            }

            public bool MouseDown(MouseButton button)
            {
                return ButtonsDown.Contains(button);
            }

        }
    }
}
