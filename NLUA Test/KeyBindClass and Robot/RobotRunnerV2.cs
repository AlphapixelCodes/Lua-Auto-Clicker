using NLua;
using NLUA_Test.Lua_Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NLUA_Test
{
    public static class RobotRunnerV2
    {
        public static Lua State;
        private static List<CancellationTokenSource> Tokens=new List<CancellationTokenSource>();
        delegate void CallFnc(LuaFunction? fnc, string name, LuaFunctions.TickType type);
        private static Stack<Tuple<string, LuaFunctions.TickType, string>> luaFunctionStack = new();
        private static CancellationTokenSource? StackManagerToken;
        static bool currentlyRunningCode = false;
        public static void LoadScript(LuaRobotV2 r)
        {
            Stop();
            State?.Close();
            luaFunctionStack.Clear();
            StackManagerToken=new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(new WaitCallback(StackManagerRun), StackManagerToken);
            State = new Lua();
            LuaFunctions.Init(State);
            State.DoString(r.LuaCode);

        }

        private static void StackManagerRun(object? state)
        {
            var token = (CancellationTokenSource)state;
            while (!token.IsCancellationRequested)
            {
                if (luaFunctionStack.Count > 0)
                {
                    if (luaFunctionStack.Count > 200)
                        luaFunctionStack.Clear();
                    //Console.WriteLine(luaFunctionStack.Count);
                    var f = luaFunctionStack.Pop();
                    try
                    {
                        if (State != null && f != null)
                            State.GetFunction(f.Item1)?.Call(f.Item2, f.Item3);
                    }catch (Exception e)
                    {

                    }
                }
            }
        }

        public static void StartTickDelay(KeybindClass kbc,CancellationTokenSource token)
        {
            Tokens.Add(token);
            ThreadPool.QueueUserWorkItem(new WaitCallback(TickDelay), new Tuple<CancellationTokenSource,KeybindClass>(token,kbc));
        }

        private static void TickDelay(object state)
        { 
            var tuple=(Tuple<CancellationTokenSource, KeybindClass>) state;
            var token = tuple.Item1.Token;
            var kbc=tuple.Item2;
            
            while (!token.IsCancellationRequested)
            {
                CallFunctionDel(kbc);
                Thread.Sleep(kbc.TickTime);
            }
        }

        public static void CallFunctionDel(KeybindClass kbc)
        {
            luaFunctionStack.Push(new Tuple<string, LuaFunctions.TickType, string>(kbc.FunctionName, kbc.TickType, kbc.Name));
            //LuaFunction? fnc = State.GetFunction(kbc.FunctionName);
            //new CallFnc(CallFunction).Invoke(fnc,kbc.Name, kbc.TickType);
        }

        /* private static void CallFunction(LuaFunction? fnc, string name, LuaFunctions.TickType type)
         {
             try
             {
                 Console.WriteLine("currentlyRunningCode: " + currentlyRunningCode);
                 while(currentlyRunningCode)
                     Thread.Sleep(5);
                 currentlyRunningCode = true;
                 fnc?.Call(type, name);
                 currentlyRunningCode = false;
             }
             catch (Exception ex)
             {

                 LuaFunctions.Print("Error: RobotRunnerV2.CallFunction: "+ex.Message);
             }
         }*/

        internal static void Stop()
        {
            StackManagerToken?.Cancel();
            Tokens.ForEach(e => e.Cancel());
            Tokens.Clear();
        }
    }
}
