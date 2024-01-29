class_name Nodes


static func get_children_in_group(parent: Node, group_name: StringName) -> Array[Node]:
	return parent.get_tree().get_nodes_in_group(group_name).filter(func(node): return parent.is_ancestor_of(node))


static func get_first_child_in_group(parent: Node, group_name: StringName) -> Node:
	return parent.get_tree().get_nodes_in_group(group_name).filter(func(node): return parent.is_ancestor_of(node)).pop_front()


static func can_queue_free(node: Node) -> bool:
	return node and node.is_inside_tree() and not node.is_queued_for_deletion()
