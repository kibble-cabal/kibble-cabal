class_name GameModeResource extends ModdableResource

const UISceneGroupName := &"game_mode_ui_scene"

@export var name: String
@export var world_paused: bool
## Should extend [GameModeState].
@export var state: Script
@export var transition_script: Script
@export var before_enter_method: StringName
@export var before_exit_method: StringName
@export var after_enter_method: StringName
@export var after_exit_method: StringName

@export_group("UI", "ui_")
@export var ui_color: Color
@export var ui_scene: PackedScene
## If less than 0, will not show in menu.
@export var ui_menu_index: int = -1
@export var ui_icon: Texture2D

var ui_scene_instance: Node

func before_enter() -> void:
	# Add UI scene
	var ui_root := UIConfig.get_game_mode_ui_root()
	if ui_scene and ui_root: 
		var instance := ui_scene.instantiate()
		instance.add_to_group(UISceneGroupName)
		ui_root.add_child(instance)
	call_subscript(transition_script, before_enter_method)


func after_enter() -> void:
	call_subscript(transition_script, after_enter_method)


func before_exit() -> void:
	# Remove UI scene
	var ui_instance := Nodes.get_first_child_in_group(UIConfig.get_game_mode_ui_root(), UISceneGroupName)
	if ui_instance: ui_instance.queue_free()
	call_subscript(transition_script, before_exit_method)


func after_exit() -> void:
	call_subscript(transition_script, after_exit_method)


func lua_fields() -> Array:
	return super() + [
		"name",
		"ui_icon",
		"ui_color",
		"ui_scene",
		"ui_menu_index",
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
