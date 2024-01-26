class_name MetaLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetMeta", func(): return Meta)


func expose_hooks(lua: LuaAPI) -> void:
	Meta.databases_ready.connect(
		func() -> void: lua.call_function("OnAllDBsReady", [])
	)
	Meta.systems_ready.connect(
		func() -> void: lua.call_function("OnAllSystemsReady", [])
	)
