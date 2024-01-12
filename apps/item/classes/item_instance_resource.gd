class_name ItemInstanceResource extends ModdableResource

## Should correspond to a [member ItemResource.id]
@export var item_id: String
@export var creation_time: int

# May split these properties into separate resources later...
@export_category("Additional data")

## Only applicable if the corresponding item has [member ItemResource.physics_resource]
@export var location: Vector2 


func get_item_resource() -> ItemResource:
	return ItemDB.find_by_id(item_id) if ItemDB else null


func lua_fields() -> Array:
	return super() + ["item_id", "creation_time", "uses", "get_item_resource"]
