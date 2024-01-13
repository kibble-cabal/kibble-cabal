class_name GameModeResource extends ModdableResource

@export var name: String
@export var icon: Texture2D
@export var world_paused: bool
## Should contain an enter method and an exit method
@export var transition_script: Script
@export var before_enter_method: StringName
@export var before_exit_method: StringName
@export var after_enter_method: StringName
@export var after_exit_method: StringName


func before_enter() -> void:
	if transition_script and transition_script.has_method(before_enter_method):
		transition_script[before_enter_method].call()


func after_enter() -> void:
	if transition_script and transition_script.has_method(after_enter_method):
		transition_script[after_enter_method].call()


func before_exit() -> void:
	if transition_script and transition_script.has_method(before_exit_method):
		transition_script[before_exit_method].call()


func after_exit() -> void:
	if transition_script and transition_script.has_method(after_exit_method):
		transition_script[after_exit_method].call()
