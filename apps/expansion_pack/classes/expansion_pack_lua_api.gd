class_name ExpansionPackLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetExpansionPackDB", func(): return ExpansionPackDB)
	lua.push_variant("GetExpansionPackSystem", func(): return ExpansionPackSystem)


func expose_hooks(lua: LuaAPI) -> void:
	ExpansionPackDB.pack_registered.connect(
		func(expansion_pack: ExpansionPackResource) -> void:
			lua.call_function("OnExpansionPackRegistered", [expansion_pack])
	)
	ExpansionPackDB.pack_unregistered.connect(
		func(expansion_pack: ExpansionPackResource) -> void:
			lua.call_function("OnExpansionPackUnregistered", [expansion_pack])
	)
	ExpansionPackSystem.pack_initialized.connect(
		func(expansion_pack: ExpansionPackResource) -> void:
			lua.call_function("OnExpansionPackInitialized", [expansion_pack])
	)
