class_name LocationLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetLocationDB", func(): return LocationDB)
	lua.push_variant("GetLocationSystem", func(): return LocationSystem)
