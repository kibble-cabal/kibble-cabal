# SubtreeDB
extends Node

class RegisteredSubtree:
	var key: StringName
	var subtree: BehaviorTree
	
	func _init(key: StringName, subtree: BehaviorTree) -> void:
		self.key = key
		self.subtree = subtree
	
	func lua_fields() -> Array[String]:
		return ["key", "subtree"]


signal subtree_registered(subtree: RegisteredSubtree)
signal subtree_unregistered(subtree: RegisteredSubtree)


var registered_subtrees: Array[RegisteredSubtree] = []


func register(subtree: RegisteredSubtree) -> void:
	registered_subtrees.append(subtree)
	subtree_registered.emit(subtree)


func unregister(subtree: RegisteredSubtree) -> void:
	registered_subtrees.erase(subtree)
	subtree_unregistered.emit(subtree)


func find_by_key(key: StringName) -> Array[BehaviorTree]:
	var trees: Array[BehaviorTree] = []
	for subtree in registered_subtrees:
		if subtree.key == key: trees.append(subtree.subtree)
	return trees


func lua_fields() -> Array[String]:
	return ["registered_subtrees", "register", "unregister", "find_by_key"]
