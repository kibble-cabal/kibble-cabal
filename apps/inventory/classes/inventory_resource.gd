class_name InventoryResource extends ModdableResource


@export var item_instances: Array[ItemInstanceResource] = []:
	set(value):
		item_instances = value
		emit_changed()
