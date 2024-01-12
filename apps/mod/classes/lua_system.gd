extends Node

const LUA_LIBRARIES: Dictionary = {
	BASE = "base",
	COROUTINE = "coroutine",
	TABLE = "table",
	STRING = "string",
	MATH = "math",
	UTF8 = "utf8",
	PACKAGE = "package",
	IO = "io",
	OS = "os",
	DEBUG = "debug"
}

const ALLOWED_LUA_LIBRARIES: Array[String] = [
	LUA_LIBRARIES.BASE,
	LUA_LIBRARIES.COROUTINE,
	LUA_LIBRARIES.TABLE,
	LUA_LIBRARIES.STRING,
	LUA_LIBRARIES.MATH,
	LUA_LIBRARIES.UTF8,
]

var lua := LuaAPI.new()


func _ready() -> void:
	lua.object_metatable.permissive = false
	lua.bind_libraries(ALLOWED_LUA_LIBRARIES)	
	
	SaveLuaAPI.new().expose(lua)
	SettingsLuaAPI.new().expose(lua)
	LocationLuaAPI.new().expose(lua)
	AnimalLuaAPI.new().expose(lua)
	ItemLuaAPI.new().expose(lua)
	DatetimeLuaAPI.new().expose(lua)
	PlayerLuaAPI.new().expose(lua)
	AILuaAPI.new().expose(lua)
	ExpansionPackLuaAPI.new().expose(lua)
	AbilityLuaAPI.new().expose(lua)
	lua.push_variant("Log", Log.new())

	Log.from(self, "Lua is set up!")
	#var err: LuaError = lua.do_string("""
	#print("Lua is set up!")
	#""")
	#if err is LuaError:
		#Log.from(self, "ERROR %d: %s" % [err.type, err.message])


func _to_string() -> String:
	return "LuaSystem"
