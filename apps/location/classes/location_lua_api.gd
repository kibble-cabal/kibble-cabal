class_name LocationLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("LocationDB", LocationDB)
	lua.push_variant("LocationSystem", LocationSystem)
