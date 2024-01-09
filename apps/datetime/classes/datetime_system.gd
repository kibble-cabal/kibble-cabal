# DatetimeSystem
extends Node

signal ticked
signal unpaused
signal paused

var timer := Timer.new()

var resource: DatetimeResource:
	get: 
		if SaveSystem.current_save and SaveSystem.current_save.datetime:
			return SaveSystem.current_save.datetime
		return null

var is_paused: bool = false:
	set(value):
		match [is_paused, value]:
			[true, false]: unpaused.emit()
			[false, true]: paused.emit()
		is_paused = value


func _ready() -> void:
	timer.wait_time = get_wait_time()
	timer.autostart = true
	timer.timeout.connect(_on_timeout)
	add_child(timer)
	
	SaveSystem.save_opened.connect(_on_save_changed)
	SaveSystem.save_closed.connect(_on_save_changed)


## How long the timer should wait before incrementing the current time ([member DatetimeResource.current_time])
func get_wait_time() -> float:
	return DatetimeResource.TIME_SPEED * (resource.time_speed_multiplier if resource else 1.0)


func pause() -> void:
	is_paused = true


func unpause() -> void:
	is_paused = false


func lua_fields() -> Array[String]:
	return ["get_wait_time", "is_paused", "pause", "unpause"]


func _on_timeout() -> void:
	if resource: 
		resource.current_time += 1
		ticked.emit()


func _on_save_changed(_save: SaveResource) -> void:
	timer.wait_time = get_wait_time()
