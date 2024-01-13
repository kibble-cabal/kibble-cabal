@tool
class_name TransformableStyleBox extends StyleBox

@export var stylebox: StyleBox:
	set(value):
		stylebox = value
		if stylebox and not stylebox.changed.is_connected(emit_changed):
			stylebox.changed.connect(emit_changed)
		emit_changed()

@export_group("Transform")
@export var offset := Vector2.ZERO:
	set(value):
		offset = value
		emit_changed()

@export_range(0, 360, 1, "degrees", "or_greater", "or_less") var rotation := 0.0:
	set(value):
		rotation = value
		emit_changed()

@export var scale := Vector2.ONE:
	set(value):
		scale = value
		emit_changed()

@export_range(0, 1) var pivot_offset_x := 0.5:
	set(value):
		pivot_offset_x = value
		emit_changed()

@export_range(0, 1) var pivot_offset_y := 0.5:
	set(value):
		pivot_offset_y = value
		emit_changed()

@export var reset_transform_after_draw := true:
	set(value):
		reset_transform_after_draw = value
		emit_changed()

@export_group("Inset", "inset_")

@export var inset_top: float = 0.0:
	set(value):
		inset_top = value
		emit_changed()

@export var inset_right: float = 0.0:
	set(value):
		inset_right = value
		emit_changed()

@export var inset_bottom: float = 0.0:
	set(value):
		inset_bottom = value
		emit_changed()

@export var inset_left: float = 0.0:
	set(value):
		inset_left = value
		emit_changed()


@warning_ignore("unused_parameter")
func _draw(to_canvas_item: RID, rect: Rect2) -> void:
	var canvas_item := get_current_item_drawn()
	if stylebox and canvas_item:
		canvas_item.draw_set_transform_matrix(get_transform_matrix(rect))
		canvas_item.draw_style_box(stylebox, get_rect(rect))
		
		if reset_transform_after_draw:
			canvas_item.draw_set_transform_matrix(Transform2D.IDENTITY)


func get_rect(base_rect: Rect2) -> Rect2:
	return Rect2(base_rect).grow_individual(-inset_left, -inset_top, -inset_right, -inset_bottom)


func get_pivot_offset(base_rect: Rect2) -> Vector2:
	return base_rect.position + base_rect.size * Vector2(pivot_offset_x, pivot_offset_y)


func get_transform_matrix(base_rect: Rect2) -> Transform2D:
	var transform := _rotate_around_pivot(Transform2D.IDENTITY, deg_to_rad(rotation), get_pivot_offset(base_rect))
	transform.origin += offset
	return transform.scaled(scale)


func _rotate_around_pivot(transform: Transform2D, angle: float, pivot: Vector2) -> Transform2D:
	var t := transform
	t.origin -= pivot
	t = t.rotated(angle)
	t.origin += pivot
	return t
