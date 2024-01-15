class_name ItemResource extends ModdableResource

@export var id: String
@export var display_name: String
@export var description: String
@export var icon: Texture2D

@export_category("More Data")
@export var physics_resource: ItemPhysicsResource
@export var retail_resource: ItemRetailResource
@export var ability_state: AbilitySystemComponentState


func instantiate() -> ItemInstanceResource:
	var instance := ItemInstanceResource.new()
	instance.item_id = id
	instance.ability_state = ability_state.duplicate(false)
	return instance


func lua_fields() -> Array:
	return super() + [
		"id",
		"display_name", 
		"description", 
		"icon", 
		"physics_resource", 
		"retail_resource"
	]
