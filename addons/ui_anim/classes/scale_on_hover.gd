class_name UIScaleOnHover extends UIHoverBase

var start_scale: Vector2
var max_scale := Vector2(1.1, 1.1)


func _init(node: Control) -> void:
	await super._init(node)
	start_scale = node.scale
	node.pivot_offset = node.size / 2


func _get_default_duration() -> float:
	return 0.125


func _on_mouse_entered() -> void:
	tween_property("scale", start_scale * max_scale)


func _on_mouse_exited() -> void:
	if node.scale > start_scale and not is_hovering():
		tween_property("scale", start_scale)


func _on_mouse_exited_window() -> void:
	if node.scale > start_scale and not is_hovering():
		tween_property("scale", start_scale)
