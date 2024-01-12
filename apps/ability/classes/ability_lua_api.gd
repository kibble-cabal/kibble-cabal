class_name AbilityLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetAbilityDB", func(): return AbilityDB)
	lua.push_variant("GetAttributeDB", func(): return AttributeDB)
	lua.push_variant("GetEffectDB", func(): return EffectDB)
	lua.push_variant("GetTagDB", func(): return TagDB)
