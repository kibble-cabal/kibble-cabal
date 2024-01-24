class_name AbilityLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetAbilityDB", func(): return AbilityDB)
	lua.push_variant("GetAttributeDB", func(): return AttributeDB)
	lua.push_variant("GetEffectDB", func(): return EffectDB)
	lua.push_variant("GetTagDB", func(): return TagDB)


func expose_hooks(lua: LuaAPI) -> void:
	AbilityDB.ability_registered.connect(
		func(ability: Ability) -> void:
			lua.call_function("OnAbilityRegistered", [ability])
	)
	AbilityDB.ability_unregistered.connect(
		func(ability: Ability) -> void:
			lua.call_function("OnAbilityUnregistered", [ability])
	)
	AttributeDB.attribute_registered.connect(
		func(attribute: Attribute) -> void:
			lua.call_function("OnAttributeRegistered", [attribute])
	)
	AttributeDB.attribute_unregistered.connect(
		func(attribute: Attribute) -> void:
			lua.call_function("OnAttributeUnregistered", [attribute])
	)
	EffectDB.effect_registered.connect(
		func(effect: Effect) -> void:
			lua.call_function("OnEffectRegistered", [effect])
	)
	EffectDB.effect_unregistered.connect(
		func(effect: Effect) -> void:
			lua.call_function("OnEffectUnregistered", [effect])
	)
	TagDB.tag_registered.connect(
		func(tag: Tag) -> void:
			lua.call_function("OnTagRegistered", [tag])
	)
	TagDB.tag_unregistered.connect(
		func(tag: Tag) -> void:
			lua.call_function("OnTagUnregistered", [tag])
	)
