# TagDB
extends Node


signal tag_registered(tag: Tag)
signal tag_unregistered(tag: Tag)


var registered_tags: Array[Tag] = []


func register(tag: Tag) -> void:
	registered_tags.append(tag)
	tag_registered.emit(tag)


func unregister(tag: Tag) -> void:
	registered_tags.erase(tag)
	tag_unregistered.emit(tag)


func find(identifier: StringName) -> Tag:
	for tag in registered_tags:
		if tag.identifier == identifier: return tag
	return null


func lua_fields() -> Array:
	return [
		"registered_tags", 
		"register", 
		"unregister", 
		"find",
	]
