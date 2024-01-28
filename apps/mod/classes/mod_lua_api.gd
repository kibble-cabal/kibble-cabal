class_name ModLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetModDB", func(): return ModDB)
	lua.push_variant("GetModSystem", func(): return ModSystem)


func expose_constructors(lua: LuaAPI) -> void:
	lua.push_variant("ZIPReader", func(): return CustomZIPReader.new())


func expose_hooks(lua: LuaAPI) -> void:	
	ModDB.mod_registered.connect(
		func(mod: ModResource) -> void:
			lua.call_function("OnModRegistered", [mod])
	)
	ModDB.mod_unregistered.connect(
		func(mod: ModResource) -> void:
			lua.call_function("OnModUnregistered", [mod])
	)
	ModSystem.mod_initialized.connect(
		func(mod: ModResource) -> void:
			lua.call_function("OnModInitialized", [mod])
	)
	ModSystem.all_mods_initialized.connect(
		func() -> void:
			lua.call_function("OnAllModsInitialized", [])
	)
