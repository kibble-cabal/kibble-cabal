class_name PlayerResource extends ModdableResource

const DefaultSpriteScene := preload("res://expansions/core/player/scenes/player_sprite.tscn")

@export var name: String

## This scene should contain a script that extends [SpriteController].
## TODO: This is not a good way to do this. It shouldn't be part of EVERY player resource.
@export var sprite_scene: PackedScene = DefaultSpriteScene
