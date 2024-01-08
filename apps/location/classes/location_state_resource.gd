class_name LocationStateResource extends ModdableResource

## Corresponds to [member LocationResource.name]
@export var location_name: String

## The collection of item instances that exist at this location
@export var inventory: InventoryResource


func get_location_resource() -> LocationResource:
	return LocationDB.find(location_name) if LocationDB else null


func lua_fields() -> Array[String]:
	return super() + ["location_name", "get_location_resource"]
