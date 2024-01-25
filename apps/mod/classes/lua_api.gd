class_name ExposeLuaAPI extends Object


func expose(lua: LuaAPI) -> void:
	expose_variables(lua)
	expose_methods(lua)
	expose_constructors(lua)
	expose_hooks(lua)


## Override this method to expose variables to Lua by calling [method LuaAPI.push_variant]
func expose_variables(_lua: LuaAPI) -> void:
	pass


## Override this method to expose methods to Lua by calling [method LuaAPI.push_variant]
func expose_methods(_lua: LuaAPI) -> void:
	pass


## Override this method to expose methods to Lua by calling [method LuaAPI.expose_constructor]
func expose_constructors(_lua: LuaAPI) -> void:
	pass


## Override this method to expose signals to Lua
func expose_hooks(_lua: LuaAPI) -> void:
	pass


func load_json(lua: LuaAPI, path: String, json_loader: JsonLoader):
	var mod: ModResource = lua.call_function("GetCurrentMod", [])
	return mod.get_zipped(
		func (loader: ContentLoader) -> AnimalResource:
			return loader.load_json_with_loader(path, json_loader)
	)
