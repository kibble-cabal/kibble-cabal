class_name LocationResource extends ModdableResource

@export var name: String
@export var map: PackedScene
@export var player_spawn_position: Vector3
@export var music_id: String


func get_music() -> MusicResource:
	if not music_id.is_empty(): return MusicDB.find(music_id)
	return null


## Returns the state of this location within this save file, if it exists. 
## Otherwise, creates a new state for this location and returns it.
func get_or_create_state() -> LocationStateResource:
	if SaveSystem.current_save:
		return SaveSystem.current_save.get_or_create_location_state(name)
	return null


func lua_fields() -> Array:
	return super() + ["name", "map", "player_spawn_position", "get_music", "get_or_create_state"]
