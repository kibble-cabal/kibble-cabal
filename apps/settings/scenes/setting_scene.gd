extends HBoxContainer

var setting_ui: Node


func render(setting_definition: SettingDefinitionResource) -> void:
	%DisplayNameLabel.text = setting_definition.display_name
	%DisplayDescriptionLabel.text = setting_definition.display_description
	if setting_ui:
		setting_ui.queue_free()
	if setting_definition.ui:
		setting_ui = setting_definition.ui.instantiate()
		add_child(setting_ui)
