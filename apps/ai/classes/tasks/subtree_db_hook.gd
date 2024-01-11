@tool
extends BTSequence

@export var hook_key: StringName


func _generate_name() -> String:
	return "Run Subtrees from DB: \"{0}\"".format([hook_key if hook_key else "???"])


func _setup() -> void:
	if not hook_key: return
	var subtrees := SubtreeDB.find_by_key(hook_key)
	for tree in subtrees:
		add_child(tree.root_task.clone())
