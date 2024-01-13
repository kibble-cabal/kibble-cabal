class_name ItemLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetItemDB", func(): return ItemDB)


func expose_hooks(lua: LuaAPI) -> void:
	ItemDB.item_registered.connect(
		func(item: ItemResource) -> void:
			lua.call_function("OnItemRegistered", [item])
	)
	ItemDB.item_unregistered.connect(
		func(item: ItemResource) -> void:
			lua.call_function("OnItemUnregistered", [item])
	)
