class_name DatetimeLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetDatetimeSystem", func(): return DatetimeSystem)
	lua.push_variant("DatetimeHelper", DatetimeHelper.new())


func expose_hooks(lua: LuaAPI) -> void:
	DatetimeSystem.ticked.connect(
		func() -> void:
			lua.call_function("OnDatetimeTicked", [])
	)
