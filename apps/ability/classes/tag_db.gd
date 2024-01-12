# TagDB
extends Node


signal tag_registered(tag: ATag)
signal tag_unregistered(tag: ATag)

signal tag_group_registered(tag_group: ATagGroup)
signal tag_group_unregistered(tag_group: ATagGroup)


var registered_tags: Array[ATag] = []
var registered_tag_groups: Array[ATagGroup] = []


func register_tag(tag: ATag) -> void:
	registered_tags.append(tag)
	tag_registered.emit(tag)


func unregister_tag(tag: ATag) -> void:
	registered_tags.erase(tag)
	tag_unregistered.emit(tag)


func find_tag(tag_id: String) -> ATag:
	for tag in registered_tags:
		if tag.identifier() == tag_id: return tag
	return null


func register_tag_group(tag_group: ATagGroup) -> void:
	registered_tag_groups.append(tag_group)
	tag_group_registered.emit(tag_group)


func unregister_tag_group(tag_group: ATagGroup) -> void:
	registered_tag_groups.erase(tag_group)
	tag_group_unregistered.emit(tag_group)


func find_tag_group(tag_group_id: String) -> ATagGroup:
	for tag_group in registered_tag_groups:
		if tag_group.identifier() == tag_group_id: return tag_group
	return null


func lua_fields() -> Array:
	return [
		"registered_tags", 
		"register_tag", 
		"unregister_tag", 
		"find_tag", 
		"registered_tag_groups", 
		"register_tag_group", 
		"unregister_tag_group", 
		"find_tag_group"
	]
