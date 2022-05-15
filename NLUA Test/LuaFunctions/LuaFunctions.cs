using NLua;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
namespace NLUA_Test
{
    public static class LuaFunctions
    {
        public static event EventHandler? PrintEvent;
        private static InputSimulator inputSimulator = new();
        public enum TickType {Press=0,Toggle=1,Release=2,Hold=3}
        public static void Init(Lua state)
        {
#pragma warning disable CS8974 // Converting method group to non-delegate type
            state.DoString("TickType={Press=0,Toggle=1,Release=2,Hold=3}");
            //keyboard
            state.DoString("Keyboard={}");
            state.DoString(@"Key={LBUTTON = 1,RBUTTON = 2,CANCEL = 3,MBUTTON = 4,XBUTTON1 = 5,XBUTTON2 = 6,BACK = 8,TAB = 9,CLEAR = 12,RETURN = 13,SHIFT = 0x10,CONTROL = 17,MENU = 18,PAUSE = 19,CAPITAL = 20,KANA = 21,HANGEUL = 21,HANGUL = 21,JUNJA = 23,FINAL = 24,HANJA = 25,KANJI = 25,ESCAPE = 27,CONVERT = 28,NONCONVERT = 29,ACCEPT = 30,MODECHANGE = 0x1F,SPACE = 0x20,PRIOR = 33,NEXT = 34,END = 35,HOME = 36,LEFT = 37,UP = 38,RIGHT = 39,DOWN = 40,SELECT = 41,PRINT = 42,EXECUTE = 43,SNAPSHOT = 44,INSERT = 45,DELETE = 46,HELP = 47,VK_0 = 48,VK_1 = 49,VK_2 = 50,VK_3 = 51,VK_4 = 52,VK_5 = 53,VK_6 = 54,VK_7 = 55,VK_8 = 56,VK_9 = 57,VK_A = 65,VK_B = 66,VK_C = 67,VK_D = 68,VK_E = 69,VK_F = 70,VK_G = 71,VK_H = 72,VK_I = 73,VK_J = 74,VK_K = 75,VK_L = 76,VK_M = 77,VK_N = 78,VK_O = 79,VK_P = 80,VK_Q = 81,VK_R = 82,VK_S = 83,VK_T = 84,VK_U = 85,VK_V = 86,VK_W = 87,VK_X = 88,VK_Y = 89,VK_Z = 90,LWIN = 91,RWIN = 92,APPS = 93,SLEEP = 95,NUMPAD0 = 96,NUMPAD1 = 97,NUMPAD2 = 98,NUMPAD3 = 99,NUMPAD4 = 100,NUMPAD5 = 101,NUMPAD6 = 102,NUMPAD7 = 103,NUMPAD8 = 104,NUMPAD9 = 105,MULTIPLY = 106,ADD = 107,SEPARATOR = 108,SUBTRACT = 109,DECIMAL = 110,DIVIDE = 111,F1 = 112,F2 = 113,F3 = 114,F4 = 115,F5 = 116,F6 = 117,F7 = 118,F8 = 119,F9 = 120,F10 = 121,F11 = 122,F12 = 123,F13 = 124,F14 = 125,F15 = 126,F16 = 0x7F,F17 = 0x80,F18 = 129,F19 = 130,F20 = 131,F21 = 132,F22 = 133,F23 = 134,F24 = 135,NUMLOCK = 144,SCROLL = 145,LSHIFT = 160,RSHIFT = 161,LCONTROL = 162,RCONTROL = 163,LMENU = 164,RMENU = 165,BROWSER_BACK = 166,BROWSER_FORWARD = 167,BROWSER_REFRESH = 168,BROWSER_STOP = 169,BROWSER_SEARCH = 170,BROWSER_FAVORITES = 171,BROWSER_HOME = 172,VOLUME_MUTE = 173,VOLUME_DOWN = 174,VOLUME_UP = 175,MEDIA_NEXT_TRACK = 176,MEDIA_PREV_TRACK = 177,MEDIA_STOP = 178,MEDIA_PLAY_PAUSE = 179,LAUNCH_MAIL = 180,LAUNCH_MEDIA_SELECT = 181,LAUNCH_APP1 = 182,LAUNCH_APP2 = 183,OEM_1 = 186,OEM_PLUS = 187,OEM_COMMA = 188,OEM_MINUS = 189,OEM_PERIOD = 190,OEM_2 = 191,OEM_3 = 192,OEM_4 = 219,OEM_5 = 220,OEM_6 = 221,OEM_7 = 222,OEM_8 = 223,OEM_102 = 226,PROCESSKEY = 229,PACKET = 231,ATTN = 246,CRSEL = 247,EXSEL = 248,EREOF = 249,PLAY = 250,ZOOM = 251,NONAME = 252,PA1 = 253,OEM_CLEAR = 254}");
            state["Keyboard.Down"] = KeyDown;
            state["Keyboard.Up"] = KeyUp;
            state["Keyboard.Press"] = KeyPress;
            state["Keyboard.PressList"] = KeyPressList;
            state["Keyboard.TextEntry"] = TextEntry;
            state["Keyboard.ModifiedKeyStroke"] = ModifiedKeyStroke;
            state["Keyboard.ModifiedKeyStrokeList"] = ModifiedKeyStrokeList;
            

            //mouse
            state.DoString(@"Mouse={}
            MouseButton={Left=0,Right=1}");
            state["Mouse.Down"] = MouseDown;
            state["Mouse.Up"] = MouseUp;
            state["Mouse.Click"] = MouseClick;
            state["Mouse.DoubleClick"] = MouseDoubleClick;
            state["Mouse.VerticalScroll"] = VerticalScroll;
            state["Mouse.HorizontalScroll"] = HorizontalScroll;
            state["Mouse.MoveTo"] = MoveTo;
            state["Mouse.MoveBy"] = MoveBy;

            //extras
            state["getPixelColor"] = GetPixelColor;
            state["ColorsEqual"] = ColorsEqual;
            state["ColorAtXYEquals"] = ColorAtXYEquals;
            /*state.DoString(@"function ColorAtXYEquals(x,y,color)
              color =table.concat(color)
              local c=getPixelColor(x,y)
                Print(c)
                Print(color)
                Print(c==color)
                for i=0,2,1 do
                    if (c[i]~=color[i]) then
                        return false
                    end
                end
              return true
            end");*/
            state["getCursorPosition"] = GetCursorPosition;
            state["Print"] = Print;
            state["Sleep"] = Sleep;
        }
       
        public static void Print(object s)
        {
            
            var msg = s;
            if (s is int[])
            {
                int[] arr = (int[])s;
                msg="Int Array {" + String.Join(", ", arr) + "}";
            }
            PrintEvent?.Invoke(msg.ToString(), EventArgs.Empty);
            Console.WriteLine(msg);
        }
        public static void KeyDown(int key)
        {
            
            inputSimulator.Keyboard.KeyDown((VirtualKeyCode)key);
        }
        public static void KeyUp(int key)
        {
            inputSimulator.Keyboard.KeyUp((VirtualKeyCode)key);
        }
        public static void KeyPress(int key)
        {
            inputSimulator.Keyboard.KeyPress((VirtualKeyCode)key);
        }
        public static void KeyPressList(int[] keys)
        {
            inputSimulator.Keyboard.KeyPress(keys.Select(e => (VirtualKeyCode)e).ToArray());
        }
        public static void TextEntry(string text)
        {
            inputSimulator.Keyboard.TextEntry(text);
        }
        /// <summary>
        /// for things like Ctrl + S
        /// </summary>
        /// <param name="ModifiedKey"></param>
        /// <param name="keycode"></param>
        public static void ModifiedKeyStroke(int ModifiedKey,int keycode)
        {
            inputSimulator.Keyboard.ModifiedKeyStroke((VirtualKeyCode)ModifiedKey,(VirtualKeyCode)keycode);
        }
        public static void ModifiedKeyStrokeList(int ModifiedKey, int[] keys)
        {
            inputSimulator.Keyboard.ModifiedKeyStroke((VirtualKeyCode)ModifiedKey, keys.Select(e => (VirtualKeyCode)e));
        }
        public static void Sleep(int ms)
        {
            inputSimulator.Keyboard.Sleep(ms);
        }

        /////MOUSE STUFF
        public enum MouseButton {Left=0,Right=1}; 
        public static void MouseDown(int mouseButton)
        {
            switch ((MouseButton)mouseButton)
            {
                case MouseButton.Left:
                    inputSimulator.Mouse.LeftButtonDown();
                break;
                case MouseButton.Right:
                    inputSimulator.Mouse.RightButtonDown();
                break;
            }
        }
        public static void MouseUp(int mouseButton)
        {
            switch ((MouseButton)mouseButton)
            {
                case MouseButton.Left:
                    inputSimulator.Mouse.LeftButtonUp();
                    break;
                case MouseButton.Right:
                    inputSimulator.Mouse.RightButtonUp();
                    break;
            }
        }
        public static void MouseClick(int mouseButton)
        {
            switch ((MouseButton)mouseButton)
            {
                case MouseButton.Left:
                    inputSimulator.Mouse.LeftButtonClick();
                    break;
                case MouseButton.Right:
                    inputSimulator.Mouse.RightButtonClick();
                    break;
            }
        }
        public static void MouseDoubleClick(int mouseButton)
        {
            switch ((MouseButton)mouseButton)
            {
                case MouseButton.Left:
                    inputSimulator.Mouse.LeftButtonDoubleClick();
                    break;
                case MouseButton.Right:
                    inputSimulator.Mouse.RightButtonDoubleClick();
                    break;
            }
        }
        public static void VerticalScroll(int amount)
        {
            inputSimulator.Mouse.VerticalScroll(amount);
        }
        public static void HorizontalScroll(int amount)
        {
            inputSimulator.Mouse.HorizontalScroll(amount);
        }
        [DllImport("User32.Dll")] private static extern long SetCursorPos(int x, int y);
        public static void MoveTo(int x,int y)
        {
            //SetCursorPos(x, y);
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            var X = x * 65535 / screenWidth;
            var Y = y * 65535 / screenHeight;
            inputSimulator.Mouse.MoveMouseTo(X, Y);

          //  Cursor.Position = new Point(x, y);

            //inputSimulator.Mouse.MoveMouseTo(x, y);
            /*inputSimulator.Mouse.MoveMouseTo(Convert.ToDouble(x * 65535 / 1600),Convert.ToDouble(y * 65535 / 900));
            Console.WriteLine("Mouse Move To Current pos: " +String.Join(",",GetCursorPosition()));
            Console.WriteLine($"Mouse Move To Wanted pos: {x},{y}");*/
        }
        public static void MoveBy(int x,int y)
        {
            inputSimulator.Mouse.MoveMouseBy(x, y);
        }
        public static int[] GetPixelColor(int x, int y)
        {
            System.Drawing.Color a = ColorAtXY.GetPixelColor(x, y);
            return new int[] { a.R, a.G, a.B };
        }
        public static bool ColorsEqual(object[] c1,object[] c2)
        {
            return (c1.All(e => c2.Contains(e)));
        }
        public static bool ColorAtXYEquals(int x,int y,int r,int g,int b)
        {
            Console.WriteLine($"X,Y: {x},{y} Color: [{r},{g},{b}]");
            Console.WriteLine($"Color at xy: " + String.Join(",", GetPixelColor(x, y)));
            return ColorsEqual(GetPixelColor(x, y).Cast<Object>().ToArray(), new object[] {r,g,b});
        }
        public static int[] GetCursorPosition()
        {
            var p = CursorPosition.GetCursorPosition();
            return new int[] { p.X-1, p.Y-1 };
        }
        private static int[] ObjectArrayToInt(object[] a)
        {
            return a.Select(x => (int)x).ToArray();
        }
    }
}
