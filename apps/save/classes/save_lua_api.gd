class_name SaveLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetSaveSystem", func(): return SaveSystem)


func expose_hooks(lua: LuaAPI) -> void:
	SaveSystem.save_opened.connect(
		func(save: SaveResource) -> void:
			lua.call_function("OnSaveOpened", [save])
	)
	SaveSystem.save_closed.connect(
		func(save: SaveResource) -> void:
			lua.call_function("OnSaveClosed", [save])
	)
	SaveSystem.before_saved.connect(
		func() -> void:
			lua.call_function("OnBeforeSaved", [])
	)
	SaveSystem.saved.connect(
		func() -> void:
			lua.call_function("OnSaved", [])
	)
