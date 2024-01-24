class_name ActionLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetActionDB", func(): return ActionDB)


func expose_hooks(lua: LuaAPI) -> void:
	ActionDB.action_registered.connect(
		func(action: ActionMenuItem) -> void:
			lua.call_function("OnActionRegistered", [action])
	)
	ActionDB.action_unregistered.connect(
		func(action: ActionMenuItem) -> void:
			lua.call_function("OnActionUnregistered", [action])
	)
