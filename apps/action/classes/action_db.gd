# actionDB
extends Node

signal action_registered(action: ActionMenuItem)
signal action_unregistered(action: ActionMenuItem)


var registered_actions: Array[ActionMenuItem] = []


func register(action: ActionMenuItem) -> void:
	registered_actions.append(action)
	action_registered.emit(action)


func unregister(action: ActionMenuItem) -> void:
	registered_actions.erase(action)
	action_unregistered.emit(action)


func find_by_menu(menu_identifier: String) -> Array[ActionMenuItem]:
	var actions: Array[ActionMenuItem] = []
	for action in registered_actions:
		if menu_identifier in action.get_menu_identifiers(): actions.append(action)
	return actions


func lua_fields() -> Array:
	return ["registered_actions", "register", "unregister", "find"]
