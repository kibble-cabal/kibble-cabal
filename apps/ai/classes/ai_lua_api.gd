class_name AILuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("SubtreeDB", SubtreeDB)
