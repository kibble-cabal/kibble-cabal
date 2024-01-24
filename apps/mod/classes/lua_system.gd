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


func create_environment() -> LuaAPI:
	var lua := LuaAPI.new()
	lua.bind_libraries(ALLOWED_LUA_LIBRARIES)
	lua.object_metatable.permissive = false
	SaveLuaAPI.new().expose(lua)
	GameModeLuaAPI.new().expose(lua)
	SettingsLuaAPI.new().expose(lua)
	LocationLuaAPI.new().expose(lua)
	AnimalLuaAPI.new().expose(lua)
	ItemLuaAPI.new().expose(lua)
	DatetimeLuaAPI.new().expose(lua)
	PlayerLuaAPI.new().expose(lua)
	AILuaAPI.new().expose(lua)
	ExpansionPackLuaAPI.new().expose(lua)
	AbilityLuaAPI.new().expose(lua)
	ModLuaAPI.new().expose(lua)
	SoundEffectLuaAPI.new().expose(lua)
	MusicLuaAPI.new().expose(lua)
	ActionLuaAPI.new().expose(lua)
	lua.push_variant("Log", Log.new())
	lua.push_variant("UIConfig", UIConfig)
	return lua


func _to_string() -> String:
	return "LuaSystem"
