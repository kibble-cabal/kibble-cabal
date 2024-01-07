class_name ItemResource extends ModdableResource

@export var name: String
@export var description: String
@export var icon: Texture2D

@export_category("More Data")
@export var physics_resource: ItemPhysicsResource
@export var retail_resource: ItemRetailResource
@export var consumable_resource: ItemConsumableResource
