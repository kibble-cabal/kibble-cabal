# PlayerSystem
extends Node

signal player_spawned(node: PlayerBody2D)
signal player_despawned()
signal before_player_spawned()
signal before_player_despawned(node: PlayerBody2D)

const PlayerScene := preload("res://apps/player/scenes/player_scene.tscn")

var player: PlayerResource:
	get:
		if SaveSystem.current_save and SaveSystem.current_save.player:
			return SaveSystem.current_save.player
		return null

var player_node: PlayerBody2D = null


func spawn(location: LocationResource) -> void:
	if player:
		before_player_spawned.emit()
		player_node = PlayerScene.instantiate()
		var world_root := get_tree().get_first_node_in_group("world_root")
		if world_root: 
			world_root.add_child(player_node)
			player_node.position = get_spawn_position(location)
			player.current_position = player_node.position
			player.current_location = location.name
			Log.from(self, "Player spawned!")
			player_spawned.emit(player_node)


func despawn() -> void:
	if player_node:
		before_player_despawned.emit(player_node)
		player_node.queue_free()
		player_node = null
		player_despawned.emit()


func get_spawn_position(location: LocationResource) -> Vector2:
	if player:
		if not location: return player.location
		if player.current_location == location.name: return player.current_position
		else: return location.player_spawn_position
	return Vector2.ZERO


func lua_fields() -> Array:
	return ["player", "player_node"]


func _to_string() -> String:
	return "PlayerSystem"
