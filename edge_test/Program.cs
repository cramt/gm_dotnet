using EdgeJs;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace edge_test {
    class Program {
        public static Dictionary<string, Func<object[], Task<object>>> Instantiate(string path) {
            var a = (IDictionary<string, object>)Edge.Func(@"
return function(arg, callback){
    try {
        const raw = require(arg)
        let data = {}
        Object.getOwnPropertyNames(raw).forEach(x=>{
            data[x] = 'return ' + raw[x].toString()
        })
        callback(null, data)
    }
    catch(e){
        callback(e, null)
    }
}
")(path).GetAwaiter().GetResult();
            foreach (var funcData in a) {

            }
            /*
            Dictionary<string, Func<object[], Task<object>>> res = new Dictionary<string, Func<object[], Task<object>>>();
            var func = Edge.Func(@"
return function(arg, callback){
    try {
        const raw = require(arg)
        let data = {}
        Object.getOwnPropertyNames(raw).forEach(x=>{
            data[x] = (a) => raw[x](...a)
        })
        callback(null, data)
    }
    catch(e){
        callback(e, null)
    }
}
");
            var dyn = (IDictionary<string, object>)func(path).GetAwaiter().GetResult();
            dyn.ToList().ForEach(x => { 
                res.Add(x.Key, (Func<object[], Task<object>>)x.Value);
            });
            return res;
            */
            return null;
        }
        static void Main(string[] args) {
            var func = (Func<object, Task<object>>)Edge.Func(@"
return (arg, callback) => {
    callback(null, 'hello')
callback(null, 'hello2')
}
");


            Console.WriteLine(func("").GetAwaiter().GetResult());
            Thread.Sleep(-1);
        }
    }
}
