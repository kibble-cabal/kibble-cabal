# PlayerSystem
extends Node

const PlayerScene := preload("res://apps/player/scenes/player_scene.tscn")

var player: PlayerResource:
	get:
		if SaveSystem.current_save and SaveSystem.current_save.player:
			return SaveSystem.current_save.player
		return null

var player_node: PlayerBody2D = null


func _ready() -> void:
	LocationSystem.location_exited.connect(despawn)
	LocationSystem.location_entered.connect(spawn)


func spawn(location: LocationResource) -> void:
	if player:
		player_node = PlayerScene.instantiate()
		get_tree().current_scene.add_child(player_node)
		player_node.position = get_spawn_position(location)
		player.current_position = player_node.position
		player.current_location = location.name
		Log.from(self, "Player spawned!")


func despawn() -> void:
	if player_node:
		player_node.queue_free()
		player_node = null


func get_spawn_position(location: LocationResource) -> Vector2:
	if player:
		if not location: return player.location
		if player.current_location == location.name: return player.current_position
		else: return location.player_spawn_position
	return Vector2.ZERO


func lua_fields() -> Array[String]:
	return ["player", "player_node"]


func _to_string() -> String:
	return "PlayerSystem"
