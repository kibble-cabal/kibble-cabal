# PlayerSystem
extends Node

const PlayerScene := preload("res://apps/player/scenes/player_scene.tscn")

@export var player: PlayerResource = PlayerResource.new()
var player_node: PlayerBody2D = null


func _ready() -> void:
	LocationSystem.location_entered.connect(
		func(location: LocationResource) -> void:
			spawn(location.player_spawn_location)
	)


func spawn(spawn_location: Vector2) -> void:
	if player_node:
		player_node.queue_free()
		player_node = null
	if player:
		player_node = PlayerScene.instantiate()
		get_tree().current_scene.add_child(player_node)
		player_node.position = spawn_location
	print("Player spawned!")
