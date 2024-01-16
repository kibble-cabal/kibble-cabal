class_name UIScaleOnPress extends UIPressBase

var start_scale: Vector2
var start_alpha: float
var min_scale := Vector2(0.95, 0.95)
var min_alpha := 0.8


func _init(node: BaseButton) -> void:
	await super._init(node)
	start_scale = node.scale
	start_alpha = node.modulate.a
	node.pivot_offset = node.size / 2


func _on_button_down() -> void:
	tween_property("scale", start_scale * min_scale)
	tween_property("modulate:a", start_alpha * min_alpha)


func _on_button_up() -> void:
	var tweener := tween().set_ease(Tween.EASE_OUT)
	if not node.scale.is_equal_approx(start_scale):
		tweener.tween_property(node, "scale", start_scale, duration())
	if not is_equal_approx(node.modulate.a, start_alpha):
		tweener.tween_property(node, "modulate:a", start_alpha, duration())


func _on_mouse_released() -> void:
	_on_button_up()


func _get_default_duration() -> float:
	return 0.125
