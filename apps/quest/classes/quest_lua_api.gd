class_name QuestLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetQuestDB", func(): return QuestDB)


func expose_hooks(lua: LuaAPI) -> void:
	QuestDB.quest_registered.connect(
		func(quest: QuestResource) -> void:
			lua.call_function("OnQuestRegistered", [quest])
	)
	QuestDB.quest_unregistered.connect(
		func(quest: QuestResource) -> void:
			lua.call_function("OnQuestUnregistered", [quest])
	)
