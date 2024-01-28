class_name NeedsLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("NeedsConfig", NeedsConfig)
