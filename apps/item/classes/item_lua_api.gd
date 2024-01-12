class_name ItemLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetItemDB", func(): return ItemDB)
