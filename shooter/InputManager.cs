using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Shapes;

namespace shooter
{
    public class InputManager
    {
        public bool IsShootPressed;
        public bool IsRightPressed;
        public bool IsLeftPressed;
        public bool IsUpPressed;
        public bool IsDownPressed;

        public bool IsKey1Pressed;
        public bool IsKey2Pressed;
        public bool IsKey3Pressed;
        public bool IsKey4Pressed;

        //Cheat codes
        public bool IsKeyF2Pressed; // Unlock All Weapons
        public bool IsKeyF3Pressed; // Skip Level
        public bool IsKeyF4Pressed; // God Mode

        public bool IsKeyEscPressed;

        private Point _mousePosition;

        public Point MousePosition
        {
            get
            {
                return this._mousePosition;
            }

            set
            {
                this._mousePosition = value;
            }
        }

        public void OnKeyPressed(Key key)
        {
            if (key == Key.Space) IsShootPressed = true;

            if (key == Key.D1 || key == Key.NumPad1) IsKey1Pressed = true;
            if (key == Key.D2 || key == Key.NumPad2) IsKey2Pressed = true;
            if (key == Key.D3 || key == Key.NumPad3) IsKey3Pressed = true;
            if (key == Key.D4 || key == Key.NumPad4) IsKey4Pressed = true;

            if (key == Key.Right || key == Key.D) IsRightPressed = true;
            if (key == Key.Left || key == Key.Q) IsLeftPressed = true;
            if (key == Key.Up || key == Key.Z) IsUpPressed = true;
            if (key == Key.Down || key == Key.S) IsDownPressed = true;

            if (key == Key.F4) IsKeyF4Pressed = true;
            if (key == Key.F2) IsKeyF2Pressed = true;
            if (key == Key.F3) IsKeyF3Pressed = true;

            if (key == Key.Escape) IsKeyEscPressed = true;
        }

        public void OnKeyUp(Key key)
        {
            if (key == Key.Space) IsShootPressed = false;

            if (key == Key.D1 || key == Key.NumPad1) IsKey1Pressed = false;
            if (key == Key.D2 || key == Key.NumPad2) IsKey2Pressed = false;
            if (key == Key.D3 || key == Key.NumPad3) IsKey3Pressed = false;
            if (key == Key.D4 || key == Key.NumPad4) IsKey4Pressed = false;

            if (key == Key.Right || key == Key.D) IsRightPressed = false;
            if (key == Key.Left || key == Key.Q) IsLeftPressed = false;
            if (key == Key.Up || key == Key.Z) IsUpPressed = false;
            if (key == Key.Down || key == Key.S) IsDownPressed = false;

            if (key == Key.F4) IsKeyF4Pressed = false;
            if (key == Key.F2) IsKeyF2Pressed = false;
            if (key == Key.F3) IsKeyF3Pressed = false;

            if (key == Key.Escape) IsKeyEscPressed = false;
        }
    }
}
