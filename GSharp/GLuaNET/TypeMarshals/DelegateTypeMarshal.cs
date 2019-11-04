using GSharp.Generated.NativeClasses;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GSharp.GLuaNET.TypeMarshals {
    public class DelegateTypeMarshal : ILuaTypeMarshal {
        public object Get(GLua GLua, int stackPos = -1) {
            throw new NotImplementedException();
        }

        public void Push(GLua GLua, object obj) {
            if (obj is Delegate dele) {
                CFunc cfunc = (IntPtr L) => {
                    List<object> args = new List<object>();
                    foreach (var param in dele.Method.GetParameters()) {
                        args.Add(GLua.Get(param.ParameterType, param.Position + 1));
                    }
                    var rtn = dele.DynamicInvoke(args.ToArray());
                    if (rtn != null) {
                        GLua.Push(rtn, rtn.GetType());
                        return 1;
                    }
                    return 0;
                };
                GCHandle gch = GCHandle.Alloc(cfunc);
                GLua.AddLuaReference(gch);
                GLua.PushCFunction(cfunc);
            }
            else {
                throw new Exception("wrapper func must be castable to delegate");
            }
        }
    }
}
