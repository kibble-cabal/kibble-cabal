class_name UiTween


static func duration() -> float:
	if SaveSystem.get_setting("reduce_motion", false): return 0.0
	return 0.25


static func is_hovering(node: Control) -> bool:
	var mouse_pos := node.get_global_mouse_position()
	var rect := node.get_global_rect()
	return rect.has_point(mouse_pos)


static func tween(node: Control) -> Tween:
	return node.create_tween().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_BACK)


static func hover_grow(node: Control, max_scale := Vector2(1.1, 1.1)) -> void:
	var start_scale := node.scale
	node.pivot_offset = node.size / 2.0
	node.mouse_entered.connect(
		func() -> void:
			UiTween.tween(node).tween_property(node, "scale", start_scale * max_scale, UiTween.duration())
	)
	node.mouse_exited.connect(
		func() -> void:
			if not UiTween.is_hovering(node):
				UiTween.tween(node).tween_property(node, "scale", start_scale, UiTween.duration())
	)
