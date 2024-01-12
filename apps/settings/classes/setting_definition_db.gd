# SettingDefinitionDB
extends Node

signal setting_registered(setting: SettingDefinitionResource)
signal setting_unregistered(setting: SettingDefinitionResource)


var registered_settings: Array[SettingDefinitionResource] = []


func register(setting: SettingDefinitionResource) -> void:
	registered_settings.append(setting)
	setting_registered.emit(setting)


func unregister(setting: SettingDefinitionResource) -> void:
	registered_settings.erase(setting)
	setting_unregistered.emit(setting)


func find_by_name(setting_name: String) -> SettingDefinitionResource:
	for setting in registered_settings:
		if setting.display_name == setting_name: return setting
	return null


func find_by_id(setting_id: String) -> SettingDefinitionResource:
	for setting in registered_settings:
		if setting.id == setting_id: return setting
	return null


func lua_fields() -> Array:
	return [
		"registered_settings",
		"register",
		"find_by_name",
		"find_by_id",
	]
