using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLUA_Test
{
    public static class OverlaySettings
    {
        public enum ScreenSide { Left,Center, Right }
        
        //Toggle overlay
        public static ScreenSide OverlaySide;
        public static bool ToggleOverlayEnabled=true;
        public static int ToggleDisplayTime = 1300;
        private static StateChangedOverlay OverlayWindow=new();
        public static void SetOverlaySide(ScreenSide side)
        {
            OverlaySide = side;
            OverlayWindow.SettingsUpdated();
        }
        public static void setToggleOverlayEnabled(bool status)
        {
            ToggleOverlayEnabled=status;
            OverlayWindow.SettingsUpdated();
        }
        public static void SetToggleDisplayMessage(string msg, bool value)
        {
            if(ToggleOverlayEnabled)
                OverlayWindow.DisplayMessage(msg, value);
        }

        public static void Save()
        {
            //throw new NotImplementedException();
        }
        public static void Init()
        {
            OverlayWindow.Show();
        }
    }
}
