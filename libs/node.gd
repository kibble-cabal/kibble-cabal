class_name Nodes


static func get_flattened_node_2d_size(node: Node2D) -> Vector2:
	var size := Vector2()
	
	if node is Sprite2D:
		size = node.get_rect().size
	
	elif node is AnimatedSprite2D:
		var texture: Texture2D = node.sprite_frames.get_frame_texture(node.animation, node.frame)
		size = texture.get_size()
	
	elif node is TileMap:
		size = node.get_used_rect().size
	
	for child in node.get_children():
		if child is Node2D:
			var child_size := get_flattened_node_2d_size(child)
			size.x = max(size.x, child_size.x)
			size.y = max(size.y, child_size.y)
	
	return size


static func get_children_in_group(parent: Node, group_name: StringName) -> Array[Node]:
	return parent.get_tree().get_nodes_in_group(group_name).filter(func(node): return parent.is_ancestor_of(node))


static func get_first_child_in_group(parent: Node, group_name: StringName) -> Node:
	return parent.get_tree().get_nodes_in_group(group_name).filter(func(node): return parent.is_ancestor_of(node)).pop_front()
