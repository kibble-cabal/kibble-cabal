# GameModeDB
extends Node


signal game_mode_registered(game_mode: GameModeResource)
signal game_mode_unregistered(game_mode: GameModeResource)


var registered_game_modes: Array[GameModeResource] = []


func register(game_mode: GameModeResource) -> void:
	registered_game_modes.append(game_mode)
	game_mode_registered.emit(game_mode)


func unregister(game_mode: GameModeResource) -> void:
	registered_game_modes.erase(game_mode)
	game_mode_unregistered.emit(game_mode)


func find(game_mode_name: String) -> GameModeResource:
	for game_mode in registered_game_modes:
		if game_mode.name == game_mode_name: return game_mode
	return null


func lua_fields() -> Array:
	return ["register", "unregister", "find", "registered_game_modes"]
