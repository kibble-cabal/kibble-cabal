# DatetimeSystem
extends Node

signal ticked

var timer := Timer.new()

var resource: DatetimeResource:
	get: 
		if SaveSystem.current_save and SaveSystem.current_save.datetime:
			return SaveSystem.current_save.datetime
		return null


func _ready() -> void:
	timer.wait_time = get_wait_time()
	timer.autostart = true
	timer.timeout.connect(_on_timeout)
	add_child(timer)
	
	SaveSystem.save_opened.connect(_on_save_changed)
	SaveSystem.save_closed.connect(_on_save_changed)
	GameModeSystem.game_mode_entered.connect(_on_game_mode_entered)
	GameModeSystem.game_mode_exited.connect(_on_game_mode_exited)


## How long the timer should wait before incrementing the current time ([member DatetimeResource.current_time])
func get_wait_time() -> float:
	return DatetimeResource.TIME_SPEED * (resource.time_speed_multiplier if resource else 1.0)


func lua_fields() -> Array:
	return ["get_wait_time"]


func _on_timeout() -> void:
	if resource: 
		resource.current_time += 1
		ticked.emit()


func _on_save_changed(_save: SaveResource) -> void:
	timer.wait_time = get_wait_time()


func _on_game_mode_entered(mode: GameModeResource) -> void:
	if mode != null: timer.paused = mode.world_paused
	else: timer.paused = true


func _on_game_mode_exited(_mode: GameModeResource) -> void:
	timer.paused = true
