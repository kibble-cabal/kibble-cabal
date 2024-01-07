class_name LuaModFunctions extends Object


static func setup(lua: LuaAPI) -> void:
	lua.push_variant("print", LuaModFunctions._lua_print)
	lua.push_variant("prints", LuaModFunctions._lua_prints)


static func _lua_print(message: String) -> void:
	print(message)


static func _lua_prints(messages: Array[String]) -> void:
	print(" ".join(messages))
