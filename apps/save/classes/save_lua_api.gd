class_name SaveLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetSaveSystem", func(): return SaveSystem)
