extends VBoxContainer

const SettingScene := preload("res://apps/settings/scenes/setting_scene.tscn")

func _ready() -> void:
	SettingDefinitionDB.registered_settings.map(render_setting)


func render_setting(setting_definition: SettingDefinitionResource) -> void:
	var scene := SettingScene.instantiate()
	add_child(scene)
	scene.render(setting_definition)
