class_name DatetimeLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("DatetimeSystem", DatetimeSystem)
	lua.push_variant("DatetimeHelper", DatetimeHelper.new())
