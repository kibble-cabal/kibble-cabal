class_name PlayerResource extends ModdableResource

const DefaultSpriteScene := preload("res://expansions/core/player/scenes/player_sprite.tscn")

@export var name: String

## This scene should contain a script that extends [SpriteController].
## TODO: This is not a good way to do this. It shouldn't be part of EVERY player resource.
@export var sprite_scene: PackedScene = DefaultSpriteScene

## This is a temporary property that helps me differentiate save files.
@export var modulate: Color = Color(randf() + 0.5, randf() + 0.5, randf() + 0.5)

## Corresponds to [LocationResource.name]
@export var current_location: String:
	set(value):
		current_location = value
		emit_changed()

@export var current_position: Vector2:
	set(value):
		current_position = value
		emit_changed()
