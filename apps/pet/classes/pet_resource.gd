class_name PetResource extends ModdableResource

@export var name: String
@export var birthdate: int

## Corresponds to [member AnimalResource.name]
@export var animal: String

@export var current_position: Vector2:
	set(value):
		current_position = value
		emit_changed()

## Temporary
@export var modulate: Color = Color(randf() + 0.3, randf() + 0.3, randf() + 0.3)


func get_animal_resource() -> AnimalResource:
	return AnimalDB.find(animal) if AnimalDB else null


func lua_fields() -> Array[String]:
	return super() + ["name", "birthday", "animal", "get_animal_resource"]
