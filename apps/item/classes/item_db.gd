# ItemDB
extends Node

signal item_registered(item: ItemResource)
signal item_unregistered(item: ItemResource)


var registered_items: Array[ItemResource] = []


func register(item: ItemResource) -> void:
	registered_items.append(item)
	item_registered.emit(item)


func unregister(item: ItemResource) -> void:
	registered_items.erase(item)
	item_unregistered.emit(item)


func find_by_name(item_name: String) -> ItemResource:
	for item in registered_items:
		if item.display_name == item_name: return item
	return null


func find_by_id(item_id: String) -> ItemResource:
	for item in registered_items:
		if item.id == item_id: return item
	return null


func lua_fields() -> Array[String]:
	return ["registered_items", "register", "unregister", "find_by_name", "find_by_id"]
