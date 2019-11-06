require("nodejs")
function responseObjectContruction(ptr, path, options, boundObjects)
    if (type(ptr) == "string") then return nil, ptr end
    local luaInstanceGetHandler = nil
    luaInstanceGetHandler = function (arg)
        return luaInstanceGetHandler, ""
    end
    local res = {
        ptr = ptr,
        methods = {}
    }
    for _, v in pairs(util.JSONToTable(__NODEJS__WRAPPER__TABLE__.getInstanceMethods(res.ptr))) do
        res.methods[v] = function(...)
            local rawRes = __NODEJS__WRAPPER__TABLE__.callInstanceMethod(res.ptr, v, util.TableToJSON({...}), luaInstanceGetHandler)
            local errorPrefix = "Error: "
            if (rawRes:sub(0, errorPrefix:len()) == errorPrefix) then
                return nil, rawRes
            end
            local returnValue = nil
            local result = util.JSONToTable("[" .. rawRes .. "]")[1]
            for _, resultValue in pairs(result) do
                if(resultValue.type == "hello there") then
                    returnValue = resultValue.result
                elseif(resultValue.type == "LuaInstance") then
                    local obj = nil
                    if(resultValue.result.index == -1) then
                        obj = _G
                    else 
                        obj = boundObjects[resultValue.result.index + 1]
                    end
                    for _, pathEntry in pairs(resultValue.result.path) do
                        if(pathEntry.todo == "get") then
                            obj = obj[pathEntry.property]
                        elseif(pathEntry.todo == "set") then
                            obj[pathEntry.property] = pathEntry.value
                        elseif(pathEntry.todo == "call") then
                            obj = obj[pathEntry.property](unpack(pathEntry.arguments))
                        end
                    end
                end
            end
        end
    end

    res.remove = function()
        __NODEJS__WRAPPER__TABLE__.removeInstance(res.ptr)
        res = nil
    end

    return res
end
function argParse(options, boundObjects)
    options = options or {}
    boundObjects = boundObjects or {}
    options.name = options.name or "untitled"
    return options, boundObjects
end
function newNodeInstanceAsync(path, options, boundObjects, cb, errorCb)
    options, boundObjects = argParse(options, boundObjects)
    local ptr = __NODEJS__WRAPPER__TABLE__.instantiateAsync(path, options.name)
    local hookID = "__NODEJS__HOOK__IDENTIFIER__FOR__NEW__NODE__INSTANCE__ASYNC__" ..  options.name .. "__" .. path .. "__" .. ptr
    hook.Add("Tick", hookID, function()
        if(__NODEJS__WRAPPER__TABLE__.checkForNewAsyncInstances(ptr)) then
            cb(responseObjectContruction(ptr, path, options, boundObjects))
            hook.Remove("Tick", hookID)
        end
        local err = __NODEJS__WRAPPER__TABLE__.checkForNewAsyncInstancesExceptions(ptr)
        if(type(err) == "string") then
            errorCb(err)
            hook.Remove("Tick", hookID)
        end
    end)
end
function newNodeInstanceSync(path, options, boundObjects)
    options, boundObjects = argParse(options, boundObjects)
    local ptr = __NODEJS__WRAPPER__TABLE__.instantiateSync(path, options.name)
    return responseObjectContruction(ptr, path, options, boundObjects)
end

