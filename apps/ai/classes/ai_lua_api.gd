class_name AILuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetSubtreeDB", func(): return SubtreeDB)


func expose_hooks(lua: LuaAPI) -> void:
	SubtreeDB.subtree_registered.connect(
		func(subtree: SubtreeResource) -> void:
			lua.call_function("OnSubtreeRegistered", [subtree])
	)
	SubtreeDB.subtree_unregistered.connect(
		func(subtree: SubtreeResource) -> void:
			lua.call_function("OnSubtreeUnregistered", [subtree])
	)
