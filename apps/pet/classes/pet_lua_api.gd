class_name PetLuaAPI extends ExposeLuaAPI


func expose_properties(lua: LuaAPI) -> void:
	lua.push_variant("PetSystem", PetSystem)
