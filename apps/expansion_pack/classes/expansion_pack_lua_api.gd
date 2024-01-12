class_name ExpansionPackLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetExpansionPackDB", func(): return ExpansionPackDB)
	lua.push_variant("GetExpansionPackSystem", func(): return ExpansionPackSystem)
