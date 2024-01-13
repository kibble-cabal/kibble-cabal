class_name SettingsLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetSettingDefinitionDB", func(): return SettingDefinitionDB)


func expose_hooks(lua: LuaAPI) -> void:
	SettingDefinitionDB.setting_registered.connect(
		func(setting: SettingDefinitionResource) -> void:
			lua.call_function("OnSettingDefinitionRegistered", [setting])
	)
	SettingDefinitionDB.setting_unregistered.connect(
		func(setting: SettingDefinitionResource) -> void:
			lua.call_function("OnSettingDefinitionUnregistered", [setting])
	)
