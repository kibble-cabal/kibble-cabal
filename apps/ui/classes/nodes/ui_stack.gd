class_name UIStack extends Control

enum Mode {
	REPLACE,
	APPEND
}

@export var mode: Mode = Mode.REPLACE

var stack: Array[Control] = []

var current: Control:
	get: return stack[-1] if stack.size() > 0 else null


func push(scene: Control) -> void:
	# Remove previous scene
	if current and current.is_inside_tree() and mode == Mode.REPLACE:
		remove_child(current)
	# Add current scene
	stack.push_back(scene)
	add_child(scene)


func pop(n: int = 1) -> void:
	# Remove n scenes
	for i in range(n):
		if current:
			current.queue_free()
			stack.pop_back()
	# Add previous scene
	if current and not current.is_inside_tree():
		add_child(current)


func clear() -> void:
	# Free all children
	var children := get_children()
	children.reverse()
	for child in children:
		child.queue_free()
	stack.clear()
