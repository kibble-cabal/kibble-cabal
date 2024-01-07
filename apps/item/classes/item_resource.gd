class_name ItemResource extends ModdableResource

@export var id: String
@export var display_name: String
@export var description: String
@export var icon: Texture2D

@export_category("More Data")
@export var physics_resource: ItemPhysicsResource
@export var retail_resource: ItemRetailResource
@export var consumable_resource: ItemConsumableResource


func instantiate() -> ItemInstanceResource:
	var instance := ItemInstanceResource.new()
	instance.item = id
	return instance


func lua_fields() -> Array[String]:
	return super() + [
		"id",
		"display_name", 
		"description", 
		"icon", 
		"physics_resource", 
		"retail_resource", 
		"consumable_resource"
	]
