class_name AbilityLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetAbilityDB", func(): return AbilityDB)
	lua.push_variant("GetAttributeDB", func(): return AttributeDB)
	lua.push_variant("GetEffectDB", func(): return EffectDB)
	lua.push_variant("GetTagDB", func(): return TagDB)


func expose_hooks(lua: LuaAPI) -> void:
	AbilityDB.ability_registered.connect(
		func(ability: AAbility) -> void:
			lua.call_function("OnAbilityRegistered", [ability])
	)
	AbilityDB.ability_unregistered.connect(
		func(ability: AAbility) -> void:
			lua.call_function("OnAbilityUnregistered", [ability])
	)
	AbilityDB.stage_registered.connect(
		func(stage: AAbilityStage) -> void:
			lua.call_function("OnAbilityStageRegistered", [stage])
	)
	AbilityDB.stage_unregistered.connect(
		func(stage: AAbilityStage) -> void:
			lua.call_function("OnAbilityStageUnregistered", [stage])
	)
	AttributeDB.attribute_registered.connect(
		func(attribute: AAttribute) -> void:
			lua.call_function("OnAttributeRegistered", [attribute])
	)
	AttributeDB.attribute_unregistered.connect(
		func(attribute: AAttribute) -> void:
			lua.call_function("OnAttributeUnregistered", [attribute])
	)
	AttributeDB.attribute_table_template_registered.connect(
		func(template: AAttributeTableTemplate) -> void:
			lua.call_function("OnAttributeTableTemplateRegistered", [template])
	)
	AttributeDB.attribute_table_template_unregistered.connect(
		func(template: AAttributeTableTemplate) -> void:
			lua.call_function("OnAttributeTableTemplateUnregistered", [template])
	)
	EffectDB.effect_registered.connect(
		func(effect: AEffect) -> void:
			lua.call_function("OnEffectRegistered", [effect])
	)
	EffectDB.effect_unregistered.connect(
		func(effect: AEffect) -> void:
			lua.call_function("OnEffectUnregistered", [effect])
	)
	TagDB.tag_registered.connect(
		func(tag: ATag) -> void:
			lua.call_function("OnTagRegistered", [tag])
	)
	TagDB.tag_unregistered.connect(
		func(tag: ATag) -> void:
			lua.call_function("OnTagUnregistered", [tag])
	)
	TagDB.tag_group_registered.connect(
		func(tag_group: ATagGroup) -> void:
			lua.call_function("OnTagGroupRegistered", [tag_group])
	)
	TagDB.tag_group_unregistered.connect(
		func(tag_group: ATagGroup) -> void:
			lua.call_function("OnTagGroupUnregistered", [tag_group])
	)
