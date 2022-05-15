using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using WindowsInput.Native;

namespace NLUA_Test
{
    public class KeybindClass
    {
        public event EventHandler? ToggleChangedEvent;
        private bool Enabled = true;
        public string Name, FunctionName;
        public int TickTime;
        public List<VirtualKeyCode> Keybind;
        public LuaFunctions.TickType TickType;
        private CancellationTokenSource? Token;
        public static KeybindClass Default()
        {
            var ret= new KeybindClass();
            ret.Name = "Unnamed";
            ret.FunctionName = "onTick";
            ret.Keybind = new List<VirtualKeyCode>() {VirtualKeyCode.LCONTROL,VirtualKeyCode.VK_Q };
            ret.TickType = LuaFunctions.TickType.Toggle;
            ret.TickTime = 500;
            return ret;
        }
        
        internal void Bind(bool bind)
        {
            Token?.Cancel();
            if (bind)
            {
                KeyboardHook.KeysManager.KeyUp += KeyEvent;
                KeyboardHook.KeysManager.KeyDownFirst += KeyEvent;
            }
            else
            {
                KeyboardHook.KeysManager.KeyUp -= KeyEvent;
                KeyboardHook.KeysManager.KeyDownFirst -= KeyEvent;
            }
        }
        private bool currentlyHolding = false;
        private bool currentlyToggled = false;
        private void KeyEvent(object? sender, EventArgs e)
        {
            if (Enabled)
            {
                var key = (VirtualKeyCode)sender;
                var gkhea = (GlobalKeyboardHookEventArgs)e;
                var isKeyDown = gkhea.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown;
                //Debug.WriteLine("Key Pressed: " + sender.ToString());
                var loose = looseBindMatch(KeyboardHook.KeysManager.KeysDown);
                if (isKeyDown)
                {
                    if (loose)
                    {
                        Token?.Cancel();
                        currentlyHolding = true;
                        if (TickType == LuaFunctions.TickType.Press)
                        {
                            RobotRunnerV2.CallFunctionDel(this);
                        }
                        else if (TickType == LuaFunctions.TickType.Toggle)
                        {
                            if (currentlyToggled)
                            {
                                currentlyToggled = false;
                                OverlaySettings.SetToggleDisplayMessage(Name, false);
                            }
                            else
                            {
                                currentlyToggled = true;
                                OverlaySettings.SetToggleDisplayMessage(Name, true);
                                Token = new CancellationTokenSource();
                                RobotRunnerV2.StartTickDelay(this, Token);
                            }
                            ToggleChangedEvent?.Invoke(currentlyToggled, EventArgs.Empty);

                        }else if (TickType == LuaFunctions.TickType.Hold)
                        {
                            currentlyHolding = true;
                            Token = new CancellationTokenSource();
                            RobotRunnerV2.StartTickDelay(this, Token);
                            ToggleChangedEvent?.Invoke(true, EventArgs.Empty);
                        }
                        else
                        {
                            currentlyHolding = true;
                        }
                      //  Debug.WriteLine("Toggle/HoldStart/Press Fool!");
                    }
                    else
                    {
                        currentlyHolding = false;
                    }
                }
                else if (!isKeyDown && !loose && Keybind.Contains(key) && currentlyHolding)
                {
                    switch (TickType)
                    {
                        case LuaFunctions.TickType.Release:
                            RobotRunnerV2.CallFunctionDel(this);
                            break;
                        case LuaFunctions.TickType.Hold:
                            currentlyHolding = false;
                            ToggleChangedEvent?.Invoke(false, EventArgs.Empty);
                            Token?.Cancel();
                            break;
                    }
                    //stop currently holding
                    currentlyHolding = false;
                    Debug.WriteLine("Release!!!");
                }
            }
        }

        internal void Stop()
        {
            if (Enabled)
            {
                SetEnabled(false);
                SetEnabled(true);
            }
        }

        private bool looseBindMatch(IEnumerable<VirtualKeyCode> keys)
        {
            return Keybind.All(e => keys.Contains(e));
        }
        public KeybindClass Clone()
        {
            var ret= new KeybindClass();
            ret.Name = Name;
            ret.FunctionName = FunctionName;
            ret.Keybind = Keybind.ToArray().ToList();
            ret.TickTime=TickTime;
            ret.TickType=TickType;
            ret.Enabled=Enabled;
            return ret;
        }

       

        public static KeybindClass fromString(string s)
        {
            var ret=new KeybindClass();
            var l = s.Split(",");
            ret.Name=l[0];
            ret.FunctionName=l[1];
            ret.TickTime=int.Parse(l[2]);
            ret.TickType=(LuaFunctions.TickType)int.Parse(l[3]);
            ret.Enabled = bool.Parse(l[4]);
            ret.Keybind = l[5].Split(" ").Select(e => (VirtualKeyCode)int.Parse(e)).ToList();
            return ret;
        }
        public override string ToString()
        {
            return string.Join(",",new object[] { Name, FunctionName, TickTime,(int)TickType, Enabled, string.Join(" ", Keybind.Cast<int>()) });
        }

        public void SetEnabled(bool value)
        {
            if (!value)
            {
                currentlyHolding = false;
                if (currentlyToggled)
                {
                    currentlyToggled = false;
                    ToggleChangedEvent?.Invoke(false, EventArgs.Empty);
                }
                Token?.Cancel();
            }
            Enabled = value;
        }
        public bool GetEnabled()
        {
            return Enabled;
        }
    }
}