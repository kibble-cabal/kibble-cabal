class_name UIAnimBase extends RefCounted

var node: Control


func _init(node: Control) -> void:
	self.node = node
	if not node.is_node_ready():
		await node.ready
	if node.is_inside_tree():
		_connect_signals()


func tween() -> Tween:
	return node.create_tween().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_BACK)


func tween_property(path: NodePath, value: Variant, duration_time := duration()) -> Tween:
	var tweener := tween()
	tweener.tween_property(node, path, value, duration_time)
	return tweener


func duration() -> float:
	return _get_default_duration()
	#if SaveSystem.get_setting("reduce_motion", false): return 0.0
	#return _get_default_duration()


func is_hovering() -> bool:
	var mouse_pos := node.get_global_mouse_position()
	var rect := node.get_global_rect()
	return rect.has_point(mouse_pos)


## Virtual method.
func _get_default_duration() -> float:
	return 0.45


## Virtual method.
func _get_signals_to_connect() -> Array[Signal]:
	return []


func _connect_signals() -> void:
	for sig in _get_signals_to_connect():
		var callable_name := "_on_" + sig.get_name()
		if has_method(callable_name):
			sig.connect(self[callable_name])
