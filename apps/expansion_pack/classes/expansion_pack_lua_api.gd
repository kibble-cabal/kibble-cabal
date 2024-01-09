class_name ExpansionPackLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("ExpansionPackDB", ExpansionPackDB)
	lua.push_variant("ExpansionPackSystem", ExpansionPackSystem)
