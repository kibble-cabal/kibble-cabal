class_name ItemInstanceResource extends ModdableResource

## Should correspond to a [member ItemResource.id]
@export var item_id: String
@export var creation_time: int

@export_category("Additional data")

## Only applicable if [member item_id] has [member consumable_resources].
## [br]May split this into separate resource later...
@export var uses: int = 0


func get_item_resource() -> ItemResource:
	return ItemDB.find_by_id(item_id) if ItemDB else null


func lua_fields() -> Array[String]:
	return super() + ["item_id", "creation_time", "uses", "get_item_resource"]
