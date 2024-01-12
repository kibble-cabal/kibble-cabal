class_name QuestLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetQuestDB", func(): return QuestDB)
