@tool
@icon("circle_container.svg")
class_name CircleContainer extends Container

enum Edge {
	LEFT,
	RIGHT,
	TOP,
	BOTTOM
}

@export var unsorted: Array[Control]:
	set(value):
		unsorted = value
		_sort_children()

@export var extra_margin := 0.0:
	set(value):
		extra_margin = value
		_sort_children()

@export var fit_children_inside := false:
	set(value):
		fit_children_inside = value
		_sort_children()

@export var degree_range := 360.0:
	set(value):
		degree_range = value
		_sort_children()

@export var degree_offset := 0.0:
	set(value):
		degree_offset = value
		_sort_children()

@export var allow_overlap := false:
	set(value):
		allow_overlap = value
		reset_size()
		_sort_children()

var center: Vector2:
	get: return size / 2

var radius: float:
	get: 
		var shortest_side = min(size.x, size.y)
		return shortest_side / 2 - extra_margin

var num_children: int:
	get: return get_child_count() - unsorted.size() if is_inside_tree() else 0

var degree_increment: float:
	get: return degree_range / float(num_children if num_children > 0 else 1)


func _notification(what:int) -> void:
	match what:
		NOTIFICATION_SORT_CHILDREN, NOTIFICATION_VISIBILITY_CHANGED, NOTIFICATION_RESIZED, NOTIFICATION_DRAW:
			_sort_children()


func get_controlled_children() -> Array[Control]:
	var children: Array[Control] = []
	for child in get_children():
		if child is Control and not child in unsorted: children.append(child)
	return children


func _sort_children() -> void:
	var children: Array[Control] = get_controlled_children()
	
	var index: int = 0
	for child in children:
		# if child is Control and not child in unsorted_nodes:
		child.position = _find_pos_for(child, index)
		index += 1


func _find_pos_for(node: Control, index: int) -> Vector2:
	var degree: float = (degree_increment * index) + degree_offset
	
	if not fit_children_inside:
		return _to_circle_pos(center, radius, degree) - (node.size / 2)
	
	else:
		# create a new radius, customized to the current node's size
		var temp_center: Vector2 = center - (node.size / 2)
		var temp_radius: float = radius - (node.size.x / 2)
		return _to_circle_pos(temp_center, temp_radius, degree)


func _to_circle_pos(center_val: Vector2, radius_val: float, degree: float) -> Vector2:
	return Vector2(
		center_val.x + radius_val * cos(degree * (PI / 180)),
		center_val.y + radius_val * sin(degree * (PI / 180)),
	)


func _get_minimum_size() -> Vector2:
	var min_width: float = 0.0
	var min_height: float = 0.0
	
	if not allow_overlap:
		var children := get_controlled_children()

		children.sort_custom(_sort_children_by_width)
		for i in range(0, mini(children.size(), 2)):
			min_width += children[i].get_minimum_size().x
		
		children.sort_custom(_sort_children_by_height)
		for i in range(0, mini(children.size(), 2)):
			min_height += children[i].get_minimum_size().y
		
	var side_length := maxf(min_width, min_height)
	return Vector2(side_length, side_length)


func _sort_children_by_width(a: Control, b: Control) -> bool:
	return a.get_minimum_size().x > b.get_minimum_size().x


func _sort_children_by_height(a: Control, b: Control) -> bool:
	return a.get_minimum_size().y > b.get_minimum_size().y
