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

var expose_lua_objects: Array[ExposeLuaAPI] = [
	MetaLuaAPI.new(),
	SaveLuaAPI.new(),
	GameModeLuaAPI.new(),
	SettingsLuaAPI.new(),
	LocationLuaAPI.new(),
	AnimalLuaAPI.new(),
	ItemLuaAPI.new(),
	DatetimeLuaAPI.new(),
	AILuaAPI.new(),
	AbilityLuaAPI.new(),
	ModLuaAPI.new(),
	SoundEffectLuaAPI.new(),
	MusicLuaAPI.new()
]


func create_environment() -> LuaAPI:
	var lua := LuaAPI.new()
	lua.bind_libraries(ALLOWED_LUA_LIBRARIES)
	lua.object_metatable.permissive = false
	for object in expose_lua_objects:
		object.expose(lua)
	lua.push_variant("Log", Log.new())
	lua.push_variant("UIConfig", UIConfig)
	return lua


func _to_string() -> String:
	return "LuaSystem"
