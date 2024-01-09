class_name SettingDefinitionResource extends ModdableResource

@export var id: String
@export var display_name: String
@export var display_description: String
@export var ui: PackedScene


func lua_fields() -> Array[String]:
	return super() + [
		"id",
		"display_name",
		"display_description",
		"ui"
	]
