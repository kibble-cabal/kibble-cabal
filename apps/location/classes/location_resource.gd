class_name LocationResource extends ModdableResource

@export var name: String
@export var map: PackedScene
@export var player_spawn_position: Vector2
@export var music_id: String


func get_music() -> MusicResource:
	if not music_id.is_empty(): return MusicDB.find(music_id)
	return null


func lua_fields() -> Array:
	return super() + ["name", "map", "player_spawn_position"]
