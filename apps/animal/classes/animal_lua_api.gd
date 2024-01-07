class_name AnimalLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("AnimalDB", AnimalDB)
