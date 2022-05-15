using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WindowsInput.Native;

namespace NLUA_Test.Lua_Editor
{
    public class LuaRobotV2
    {
        public string LuaCode, Name, Description = "";
        public List<KeybindClass> Keybinds = new List<KeybindClass>();
        
        private static string delimiterString = "ThisIsANewLine8675309";
        public LuaRobotV2(string name, string luaCode)
        {
            LuaCode = luaCode;
            Name = name;
        }

        public string Save()
        {
            string ret = Name;
            ret += "\n" + Description.Replace(Environment.NewLine, delimiterString);
            ret += "\n" + LuaCode.Replace(Environment.NewLine, delimiterString);
            ret+="\n"+string.Join("\n", Keybinds);
            return ret;
        }
        public static LuaRobotV2 LoadFromString(string s)
        {
            var l = s.Split("\n");
            var ret = new LuaRobotV2(l[0], l[2].Replace(delimiterString, Environment.NewLine));
            ret.Description = l[1].Replace(delimiterString, Environment.NewLine);
            ret.Keybinds = new List<KeybindClass>();
            for (int i = 3; i < l.Length; i++)
            {
                ret.Keybinds.Add(KeybindClass.fromString(l[i]));
            }
            return ret;
        }
        public static string getBindingString(List<VirtualKeyCode> keys)
        {
            if (keys == null)
            {
                return "Not Set";
            }
            return string.Join(" + ", keys.Select(e => (Key)KeyInterop.KeyFromVirtualKey((int)e)).OrderByDescending(e => e.ToString().Length));
        }
        internal LuaRobotV2 Clone()
        {
            return new LuaRobotV2(Name, LuaCode)
            {
                Description = Description,
                Keybinds = Keybinds.Select(e => e.Clone()).ToList()
            };
        }

        internal static LuaRobotV2 Default()
        {
            return new LuaRobotV2("Default Auto Clicker", Properties.Resources.Default_Lua_Code)
            {
              Description="Default",
              Keybinds=new List<KeybindClass>() {KeybindClass.Default() }
            };
        }
    }
}