class_name ExpansionPackResource extends Resource

@export var id: String
@export var display_name: String
@export_multiline var display_description: String
@export var icon: Texture2D
@export var version: String
@export var entry_script: GDScript


func lua_fields() -> Array[String]:
	return ["id", "display_name", "display_description", "icon", "version"]
