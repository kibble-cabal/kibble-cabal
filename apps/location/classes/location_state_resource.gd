class_name LocationStateResource extends ModdableResource

signal spawners_changed

## Corresponds to [member LocationResource.name]
@export var location_name: String

## A list of things to spawn at this location.
## [br][b]Note:[/b] Should not be mutated directly. Use [method add_spawner] or [method remove_spawner] instead.
@export var spawners: Array[Spawner] = []:
	set(value):
		spawners = value
		spawners_changed.emit()
		_connect_all_subresources()


func add_spawner(spawner: Spawner) -> void:
	spawners.append(spawner)
	spawners_changed.emit()


func remove_spawner(spawner: Spawner) -> void:
	if spawner in spawners:
		spawners.erase(spawner)
		spawners_changed.emit()


func get_location_resource() -> LocationResource:
	return LocationDB.find(location_name) if LocationDB else null


func lua_fields() -> Array:
	return super() + ["location_name", "get_location_resource"]


## Performs [method _connect_subresource] for all child resources
func _connect_all_subresources() -> void:
	for subresource in spawners + subresources.values():
		_connect_subresource(subresource)
