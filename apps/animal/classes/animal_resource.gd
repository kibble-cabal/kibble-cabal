class_name AnimalResource extends ModdableResource

@export var name: StringName
@export_range(0, 1) var speed: float = 0.5
@export var sprite_scene: PackedScene
@export var collision_radius: float = 20


func lua_fields() -> Array:
	return ["name", "speed", "sprite_scene", "collision_radius"] + super()
