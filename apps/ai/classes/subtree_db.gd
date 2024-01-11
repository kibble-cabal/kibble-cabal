# SubtreeDB
extends Node


signal subtree_registered(subtree: SubtreeResource)
signal subtree_unregistered(subtree: SubtreeResource)


var registered_subtrees: Array[SubtreeResource] = []


func register(subtree: SubtreeResource) -> void:
	registered_subtrees.append(subtree)
	subtree_registered.emit(subtree)


func unregister(subtree: SubtreeResource) -> void:
	registered_subtrees.erase(subtree)
	subtree_unregistered.emit(subtree)


func find_by_key(key: StringName) -> Array[BehaviorTree]:
	var trees: Array[BehaviorTree] = []
	for subtree in registered_subtrees:
		if subtree.key == key: trees.append(subtree.subtree)
	return trees


func lua_fields() -> Array[String]:
	return ["registered_subtrees", "register", "unregister", "find_by_key"]
