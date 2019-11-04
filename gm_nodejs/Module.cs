using EdgeJs;
using GSharp;
using GSharp.Generated.LuaLibraries;
using GSharp.Generated.NativeClasses;
using GSharp.GLuaNET;
using GSharp.Native;
using GSharp.Native.Classes;
using GSharp.Native.Classes.VCR;
using Libraria.Native;
using Newtonsoft.Json;
using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace gm_nodejs {
    public unsafe static class Module {
        private static GLua GLua;
        private const string WRAPPER_OBJECT_NAME = "__NODEJS__WRAPPER__TABLE__";
        private static List<NodeInstance> instances = new List<NodeInstance>();

        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(ref lua_State state) {
            GLua = GLua.Get(state);

            GLua.CreateTable();

            GLua.Push(new Func<string, string, object>((string path, string name) => {
                int res = instances.Count;
                try {
                    instances.Add(NodeInstance.InstantiateSync(path, name));
                    return res;
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    return e.ToString();
                }
            }));
            GLua.SetField(-2, "instantiateSync");

            GLua.Push(new Func<int, string>((int instance) => {
                return JsonConvert.SerializeObject(instances[instance].Methods);
            }));
            GLua.SetField(-2, "getInstanceMethods");

            GLua.Push(new Func<int, string, string, string>((int instance, string funcName, string args) => {
                object result = instances[instance].Call(funcName, JsonConvert.DeserializeObject<object[]>(args));
                if (result == null) {
                    return null;
                }
                return JsonConvert.SerializeObject(result);
            }));

            GLua.SetField(-2, "callInstanceMethod");

            GLua.Push(new Func<double>(() => {
                return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            }));
            GLua.SetField(-2, "getUnixTime");

            GLua.SetField(GLua.LUA_GLOBALSINDEX, WRAPPER_OBJECT_NAME);
            return 0;
        }

        public static string DoAThing(string str, int num) {
            return string.Concat(Enumerable.Repeat(str, num));
        }




        [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
        public static int Close(IntPtr L) {
            return 0;
        }
    }
}
