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
            SetExportedObject,
            LuaInstanceGetReturn
        }
        public static NodeInstance InstantiateSync(string path, string name, int boundObjectAmount) {
            var func = Edge.Func(@"
const boundObjectAmount = " + boundObjectAmount + @"

const unpackProxy = Symbol('unpackProxy')
const isProxy = Symbol('isProxy')

function createProxy(index, path, luaInstance) {
    const thisPorxy = new Proxy(() => {}, {
        get(_target, prop) {
            switch (prop) {
                case isProxy:
                    return true;
                case unpackProxy:
                    return {
                        index,
                        path
                    };
                default:
                    path[path.length] = {
                        todo: 'get',
                        property: prop
                    }
                    const newProxy = createProxy(index, path, luaInstance)
                    luaInstance.proxies[luaInstance.proxies.indexOf(thisPorxy)] = newProxy
                    return newProxy
            }
        },
        set(_target, prop, value) {
            path[path.length] = {
                todo: 'set',
                property: prop,
                value: value
            }
            return true;
        },
        apply(_target, _thisArg, args) {
            path[path.length] = {
                todo: 'call',
                arguments: args
            }
            const newProxy = createProxy(index, path, luaInstance)
            luaInstance.proxies[luaInstance.proxies.indexOf(thisPorxy)] = newProxy
            return newProxy
        }
    })
    return thisPorxy;
}

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

class LuaInstance {

    constructor() {
        this.additionalReturn = null
        this.proxies = []
    }
    get(index) {
        if (index === 'global') {
            index = -1
        }
        const proxy = createProxy(index, [], this)
        this.proxies[this.proxies.length] = proxy
        return proxy
    }
}

try {
    const lib = require(" + JsonConvert.SerializeObject(path) + @")({
        LuaInstance: LuaInstance
    })
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
                const result = func(...arg)
                if (result instanceof LuaInstance) {

                    callback(null, [{
                        result: result.additionalReturn,
                        type: 'return value'
                    }].concat(result.proxies.map(x => ({
                        type: 'LuaInstance',
                        result: x[unpackProxy]
                    }))));
                    break;
                }
                callback(null, [{
                    result: result,
                    type: 'return value'
                }]);
                break;
            case 1:
                callback(null, Object.getOwnPropertyNames(lib).filter(x => typeof lib[x] === 'function'));
                break;
            case 2:
                try {
                    callback(null, JSON.stringify(getObjectBasedOnPath(lib, arg[0])));
                } catch (e) {
                    callback(e, null);
                }
                break;
            case 3:
                try {
                    getObjectBasedOnPath(lib, arg[0])[arg[1]] = arg[2];
                    callback(null, true);
                } catch (e) {
                    callback(e, null);
                }
                break;
            default:
                callback('this JsCallType is not defined yet', null);
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

        public static Task<NodeInstance> InstantiateAsync(string path, string name, int boundObjectAmount) {
            return Task<NodeInstance>.Factory.StartNew(() => {
                return InstantiateSync(path, name, boundObjectAmount);
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
