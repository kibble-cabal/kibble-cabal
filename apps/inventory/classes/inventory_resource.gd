class_name InventoryResource extends ModdableResource


@export var item_instances: Array[ItemInstanceResource] = []:
	set(value):
		if max_capacity < 0 or value.size() <= max_capacity:
			item_instances = value
			emit_changed()

## The size of [member item_instances] cannot exceed this value. If [code]< 0[/code], the capacity is unlimited.
@export var max_capacity: int = -1:
	set(value):
		max_capacity = value
		emit_changed()

var is_full: bool:
	get: return item_instances.size() <= max_capacity or max_capacity < 0


func lua_fields() -> Array[String]:
	return super() + ["item_instances", "max_capacity", "is_full"]
