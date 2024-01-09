class_name SettingsLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("SettingDefinitionDB", SettingDefinitionDB)
