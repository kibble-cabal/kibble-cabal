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


func find(item_name: String) -> ItemResource:
	for item in registered_items:
		if item.name == item_name: return item
	return null
