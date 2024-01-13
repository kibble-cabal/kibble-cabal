class_name LocationStateResource extends ModdableResource

## Corresponds to [member LocationResource.name]
@export var location_name: String

## The collection of item instances that exist at this location
@export var inventory := InventoryResource.new():
	set(value):
		inventory = value
		_connect_subresource(inventory)

## The pets that live at this location
@export var pets: Array[PetResource] = []:
	set(value):
		pets = value
		_connect_all_subresources()


func get_location_resource() -> LocationResource:
	return LocationDB.find(location_name) if LocationDB else null


func lua_fields() -> Array:
	return super() + ["location_name", "get_location_resource"]


## Performs [method _connect_subresource] for all child resources
func _connect_all_subresources() -> void:
	for subresource in [inventory] + pets + subresources.values():
		_connect_subresource(subresource)
