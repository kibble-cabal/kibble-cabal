# GameModeSystem
extends Node

signal game_mode_entered(mode: GameModeResource)
signal game_mode_exited(mode: GameModeResource)

var current_mode: GameModeResource = null


func _ready() -> void:
	if not current_mode: get_tree().paused = true


func to(mode: GameModeResource) -> void:
	exit()
	enter(mode)


func enter(mode: GameModeResource) -> void:
	if mode:
		Log.from(self, "Entering game mode: " + mode.name)
		mode.before_enter()
		current_mode = mode
		get_tree().paused = mode.world_paused
		game_mode_entered.emit(mode)
		mode.after_enter()


func exit() -> void:
	if current_mode:
		Log.from(self, "Exiting game mode: " + current_mode.name)
		current_mode.before_exit()
		current_mode = null
		get_tree().paused = true
		game_mode_exited.emit(current_mode)
		current_mode.after_exit()


func _set_world_process_mode(value: ProcessMode) -> void:
	get_tree().paused = value != PROCESS_MODE_DISABLED


func _to_string() -> String:
	return "GameModeSystem"
