class_name SettingsResource extends ModdableResource

## [Dictionary][[String], [Variant]]
## [br]Pairs of ([member SettingDefinitionResource.id], [Variant] value)
## [br][b]Important[/b]: To modify this dictionary, always use [method set_setting], because otherwise changes won't be saved automatically.
@export var settings: Dictionary:
	set(value):
		settings = value
		emit_changed()


func set_setting(key: String, value) -> void:
	settings[key] = value
	emit_changed()


func lua_fields() -> Array[String]:
	return super() + [
		"settings",
		"set_setting"
	]
