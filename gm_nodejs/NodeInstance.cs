using EdgeJs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gm_nodejs {
    public class NodeInstance {
        public enum JsCallType : uint {
            CallExportedMethod,
            GetMethods,
            GetExportedObject,
            SetExportedObject
        }
        public static NodeInstance InstantiateSync(string path, string name) {
            var func = Edge.Func(@"
function getObjectBasedOnPath(lib, path) {
    let currObj = lib;
    path.forEach(x => {
        if (!currObj) {
            throw new Error('tried to get property \'' + x + '\' from \'' + currObj + '\'')
        }
        currObj = currObj[x]
    })
    return currObj;
}

try {
    const lib = require(" + JsonConvert.SerializeObject(path) + @")
    return (arg, callback) => {
        let jsCallType = arg[0]
        arg.shift()
        switch (jsCallType) {
            case 0:
                const func = lib[arg[0]]
                if (func === undefined) {
                    callback('trying to call a non-existant method', null)
                    break;
                }
                if (typeof func !== 'function') {
                    callback('trying to call a non-function')
                    break;
                }
                arg.shift()
                callback(null, func(...arg));
                break;
            case 1:
                callback(null, Object.getOwnPropertyNames(lib).filter(x => typeof lib[x] === 'function'))
                break;
            case 2:
                try {
                    callback(null, JSON.stringify(getObjectBasedOnPath(lib, arg[0])))
                } catch (e) {
                    callback(e, null)
                }
                break;
            case 3:
                try {
                    getObjectBasedOnPath(lib, arg[0])[arg[1]] = arg[2]
                    callback(null, true)
                } catch (e) {
                    callback(e, null)
                }
                break;
            default:
                callback('this JsCallType is not defined yet', null)
                break;
        }
    }
} catch (e) {
    return (arg, callback) => {
        callback(e, null)
    }
}
");

            /*
            return new NodeInstance(new Func<string, object[], Task<object>>((string funcName, object[] args) => {
                return func(new object[] { funcName }.Concat(args).ToArray());
            }), path, name);
            */
            return new NodeInstance(func, path, name);
        }

        public static Task<NodeInstance> InstantiateAsync(string path, string name) {
            return Task<NodeInstance>.Factory.StartNew(() => {
                return InstantiateSync(path, name);
            });
        }
        private Func<object, object> rawFunc;
        private Func<string, object[], object> callExportedMethodFunc;
        private Func<string[]> getMethodsFunc;
        public string Name { get; }
        public string Path { get; }
        public string[] Methods { get; }
        public object Call(string funcName, object[] args) {
            return callExportedMethodFunc(funcName, args);
        }
        private NodeInstance(Func<object, Task<object>> rawFuncAsync, string path, string name) {
            rawFunc = new Func<object, object>((object arg) => {
                return rawFuncAsync(arg).GetAwaiter().GetResult();
            });
            callExportedMethodFunc = new Func<string, object[], object>((string funcName, object[] args) => {
                return rawFunc(new object[] { (int)JsCallType.CallExportedMethod, funcName }.Concat(args).ToArray());
            });
            getMethodsFunc = new Func<string[]>(() => {
                return ((object[])rawFunc(new object[] { (int)JsCallType.GetMethods })).Select(x => x.ToString()).ToArray();
            });
            Name = name;
            Path = path;
            Methods = getMethodsFunc();
        }
    }
}
