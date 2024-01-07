class_name LocationResource extends ModdableResource

@export var name: String
@export var map: PackedScene
@export var player_spawn_location: Vector2


func lua_fields() -> Array[String]:
	return super() + ["name", "map", "player_spawn_location"]
