require("nodejs")
function newNodeInstance(path, options, boundObjects)
    options = options or {}
    boundObjects = boundObjects or {}
    if(type(ptr)=="string") then
        return nil, ptr
    end
    local res = {
        methods = {},
        ptr = __NODEJS__WRAPPER__TABLE__.instantiateSync(path, options.name or "untitled"),
    }
    if(type(res.ptr)=="string") then
        return nil, res.ptr
    end

    
    for _, v in pairs(util.JSONToTable(__NODEJS__WRAPPER__TABLE__.getInstanceMethods(res.ptr))) do
        res.methods[v] = function(...) 
            __NODEJS__WRAPPER__TABLE__.callInstanceMethod(res.ptr, v, util.TableToJSON({...}))
        end
    end
    

    return res
end


local instance, err = newNodeInstance("E:\\Libraries\\Desktop\\test\\test.js")
print(err)
instance.methods.hello();