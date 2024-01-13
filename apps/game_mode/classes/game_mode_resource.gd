class_name GameModeResource extends ModdableResource

@export var name: String
@export var icon: Texture2D
@export var world_paused: bool
@export var transition_script: Script
@export var before_enter_method: StringName
@export var before_exit_method: StringName
@export var after_enter_method: StringName
@export var after_exit_method: StringName


func before_enter() -> void:
	call_subscript(transition_script, before_enter_method)


func after_enter() -> void:
	call_subscript(transition_script, after_enter_method)


func before_exit() -> void:
	call_subscript(transition_script, before_exit_method)


func after_exit() -> void:
	call_subscript(transition_script, after_exit_method)


func lua_fields() -> Array:
	return super() + [
		"name",
		"icon",
		"world_paused",
		"transition_script",
		"before_enter_method",
		"after_enter_method",
		"before_exit_method",
		"after_exit_method",
		"before_enter",
		"after_enter",
		"before_exit",
		"after_exit"
	]
