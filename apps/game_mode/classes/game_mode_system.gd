# GameModeSystem
extends Node

signal game_mode_entered(mode: GameModeResource)
signal game_mode_exited(mode: GameModeResource)

var current_mode: GameModeResource = null


func to(mode: GameModeResource) -> void:
	exit()
	enter(mode)


func enter(mode: GameModeResource) -> void:
	if mode:
		Log.from(self, "Entering game mode: " + mode.name)
		mode.before_enter()
		current_mode = mode
		_set_world_process_mode(mode.world_process_mode)
		game_mode_entered.emit(mode)
		mode.after_enter()


func exit() -> void:
	if current_mode:
		Log.from(self, "Exiting game mode: " + current_mode.name)
		current_mode.before_exit()
		current_mode = null
		_set_world_process_mode(PROCESS_MODE_DISABLED)
		game_mode_exited.emit(current_mode)
		current_mode.after_exit()


func _set_world_process_mode(value: ProcessMode) -> void:
	for node in get_tree().get_nodes_in_group("world_root"):
		node.process_mode = value


func _to_string() -> String:
	return "GameModeSystem"
