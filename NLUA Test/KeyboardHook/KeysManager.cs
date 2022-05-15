using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace NLUA_Test.KeyboardHook
{
    public static class KeysManager
    {
        public static HashSet<VirtualKeyCode> KeysDown=new();
        private static VirtualKeyCode[] CtrlAltDel=new VirtualKeyCode[] {VirtualKeyCode.LCONTROL,VirtualKeyCode.LMENU ,VirtualKeyCode.DELETE};
        private static GlobalKeyboardHook? hook;

        //public static event EventHandler? KeyStateChange;
        public static event EventHandler? KeyDownFirst;
        public static event EventHandler? KeyUp;
        public static event EventHandler? CtrlAltDeletePressed;
        public static void Hook()
        {
            if (hook != null)
            {
                hook.KeyboardPressed -= Hook_KeyboardPressed;
                hook.Dispose();
            }
            hook = new GlobalKeyboardHook();
            hook.KeyboardPressed += Hook_KeyboardPressed;
        }
        
        private static void Hook_KeyboardPressed(object? sender, GlobalKeyboardHookEventArgs e)
        {
            VirtualKeyCode key = (VirtualKeyCode)e.KeyboardData.VirtualCode;
            Debug.WriteLine(key);
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp)
            {
                KeysDown.Remove(key);
                KeyUp?.Invoke(key, e);
            }
            else if(!KeysDown.Contains(key))
            {
                KeysDown.Add(key);
                   KeyDownFirst?.Invoke(key, e);
            }
            if (CtrlAltDel.All(e => KeysDown.Contains(e)))
            {
                CtrlAltDeletePressed?.Invoke(null, EventArgs.Empty);
            }
        }
        
    }
}
