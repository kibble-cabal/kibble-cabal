class_name GameModeResource extends ModdableResource

@export var name: String
@export var icon: Texture2D
## If [code]true[/code], all pets, background stuff will be paused during this game mode
@export var should_pause_world: bool
@export var camera_scene: PackedScene
## Should contain an enter method and an exit method
@export var transition_script: Script
@export var enter_method: StringName
@export var exit_method: StringName
