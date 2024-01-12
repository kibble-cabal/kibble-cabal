class_name PlayerLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetPlayerSystem", func(): return PlayerSystem)
