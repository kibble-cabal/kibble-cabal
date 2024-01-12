class_name AbilityLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("AbilityDB", AbilityDB)
	lua.push_variant("AttributeDB", AttributeDB)
	lua.push_variant("EffectDB", EffectDB)
	lua.push_variant("TagDB", TagDB)
