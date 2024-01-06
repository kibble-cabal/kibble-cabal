extends Node

const PlayerScene := preload("res://apps/player/scenes/player_scene.tscn")

@export var player: PlayerResource = PlayerResource.new()
var player_node: PlayerBody2D = null


func _ready() -> void:
	spawn()


func spawn() -> void:
	if player_node:
		player_node.queue_free()
		player_node = null
	if player:
		player_node = PlayerScene.instantiate()
		player_node.set_resource(player)
		get_tree().current_scene.add_child(player_node)
		player_node.position = Vector2(100, 100)
	print("Player spawned!")
