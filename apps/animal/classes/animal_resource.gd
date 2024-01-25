class_name AnimalResource extends ModdableResource

const SchemaPath := "res://apps/animal/animal_resource.schema.json"

@export var name: StringName
@export_range(0, 1) var speed: float = 0.5
@export var sprite_scene: PackedScene
@export var collision_radius: float = 20
@export var detection_radius: float = 2000

func lua_fields() -> Array:
	return super() + [
		"name", 
		"speed", 
		"sprite_scene", 
		"collision_radius", 
		"detection_radius"
	]


static func make_loader() -> JsonLoader:
	return JsonLoader.new().set_validator(make_validator()).set_output(AnimalResource.new())


static func make_validator() -> JPropertyValidator:
	return JsonSchemaLoader.new(SchemaPath).load_validator()
