using EdgeJs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gm_nodejs {
    public class NodeInstance {
        public static NodeInstance InstantiateSync(string path, string name) {
            Dictionary<string, Func<object[], Task<object>>> res = new Dictionary<string, Func<object[], Task<object>>>();
            var func = Edge.Func(@"
try{
    const lib = require("+ JsonConvert.SerializeObject(path) + @")
    return (arg, callback) => {
        if(arg.length === 0){
            callback(null, Object.getOwnPropertyNames(lib))
        }
        const func = lib[arg[0]]
        if(func === undefined){
            callback('trying to call a non-existant method', null)
            return;
        }
        arg.shift()
        callback(null, func(...arg));
    }
}
catch(e){
    return (arg, callback) => {
        callback(e, null)
    }
}
");
            var a = (object[])func(new object[] { }).GetAwaiter().GetResult();
            var b = a.Select(x => x.ToString()).ToArray();
            return new NodeInstance(new Func<string, object[], Task<object>>((string funcName, object[] args) => {
                return func(new object[] { funcName }.Concat(args).ToArray());
            }), path, name, b);
        }

        public static Task<NodeInstance> InstantiateAsync(string path, string name) {
            return Task<NodeInstance>.Factory.StartNew(() => {
                return InstantiateSync(path, name);
            });
        }
        private Func<string, object[], Task<object>> func;
        public string Name { get; }
        public string Path { get; }
        public string[] Methods { get; }
        public object Call(string funcName, object[] args) {
            return func(funcName, args).GetAwaiter().GetResult();
        }
        private NodeInstance(Func<string, object[], Task<object>> func, string path, string name, string[] methods) {
            this.func = func;
            Name = name;
            Path = path;
            Methods = methods;
        }
    }
}
