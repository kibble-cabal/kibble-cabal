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
	lua.bind_libraries(ALLOWED_LUA_LIBRARIES)	
	LuaModFunctions.setup(lua)
	print("Lua is set up!")
	
