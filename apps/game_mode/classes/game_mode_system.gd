# GameModeSystem
extends Node

signal game_mode_changed
signal game_mode_entered(mode: GameModeResource)
signal game_mode_exited(mode: GameModeResource)

signal before_game_mode_entered(mode: GameModeResource)
signal before_game_mode_exited(mode: GameModeResource)

var current_mode: GameModeResource = null
var current_state: GameModeState = null


func _ready() -> void:
	if not current_mode: set_paused(true)


func to(mode: GameModeResource) -> void:
	_exit()
	_enter(mode)
	game_mode_changed.emit()


func _enter(mode: GameModeResource) -> void:
	if mode:
		Log.from(self, "Entering game mode: " + mode.name)
		before_game_mode_entered.emit(mode)
		mode.before_enter()
		current_mode = mode
		
		# Instantiate state
		if current_mode and current_mode.state:
			current_state = current_mode.state.new()
			add_child(current_state)
		
		# Set pause mode
		set_paused(mode.world_paused)
		
		game_mode_entered.emit(mode)
		mode.after_enter()


func _exit() -> void:
	if current_mode:
		Log.from(self, "Exiting game mode: " + current_mode.name)
		before_game_mode_exited.emit(current_mode)
		current_mode.before_exit()
		
		var prev_mode := current_mode
		current_mode = null
		
		# Remove state
		if current_state:
			current_state.queue_free()
			current_state = null
		
		# Set pause mode
		set_paused(true)
		
		game_mode_exited.emit(prev_mode)
		prev_mode.after_exit()


func _update_pause_ui() -> void:
	var pause_ui := get_tree().get_first_node_in_group("pause_ui_node")
	if pause_ui: pause_ui.queue_free()
	
	var create_ui = func() -> void:
		var ui_root := UIConfig.get_ui_root()
		if ui_root and UIConfig.PauseScene:
			pause_ui = UIConfig.PauseScene.instantiate()
			pause_ui.add_to_group("pause_ui_node")
			ui_root.add_child(pause_ui)
			ui_root.move_child(pause_ui, 0)
	
	if not current_mode: create_ui.call()
	elif current_mode.world_paused: create_ui.call()


func set_paused(value: bool) -> void:
	var world_root := get_tree().get_first_node_in_group("world_root")
	if world_root:
		world_root.process_mode = PROCESS_MODE_DISABLED if value else PROCESS_MODE_INHERIT
		_update_pause_ui()


func lua_fields() -> Array:
	return [
		"current_mode",
		"to"
	]


func _to_string() -> String:
	return "GameModeSystem"
