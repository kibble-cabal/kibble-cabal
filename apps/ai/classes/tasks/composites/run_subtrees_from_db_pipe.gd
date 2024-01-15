@tool
extends BTPipe

@export var hook_key: StringName


func _generate_name() -> String:
	return "Run subtrees from DB for hook \"{0}\"".format([hook_key if hook_key else &"???"])


func _setup() -> void:
	if not hook_key: return
	var subtrees := SubtreeDB.find_by_key(hook_key)
	for tree in subtrees:
		add_child(tree.instantiate(agent, blackboard))
