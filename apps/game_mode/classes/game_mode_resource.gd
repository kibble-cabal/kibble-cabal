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
	_call_method(before_enter_method)


func after_enter() -> void:
	_call_method(after_enter_method)


func before_exit() -> void:
	_call_method(before_exit_method)


func after_exit() -> void:
	_call_method(after_exit_method)


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


func _call_method(method_name: StringName) -> void:
	if transition_script and method_name:
		var script_value = transition_script.new()
		if script_value.has_method(method_name):
			script_value[method_name].call()
		else:
			Log.warning("Attempted to call method \"{0}\" on script \"{1}\", but that method doesn't exist".format([
				method_name,
				transition_script.resource_path
			]))
