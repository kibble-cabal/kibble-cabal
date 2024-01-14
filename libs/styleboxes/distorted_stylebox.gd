@tool
class_name DistortedStyleBox extends StyleBox


@export var max_distortion_amount: float = 4:
	set(value):
		max_distortion_amount = value
		emit_changed()

@export var bg_color: Color:
	set(value):
		bg_color = value
		emit_changed()
		
@export var seed = 0:
	set(value):
		seed = value
		_update_generator()
		emit_changed()

@export var state = 0:
	set(value):
		state = value
		_update_generator()
		emit_changed()

var generator = RandomNumberGenerator.new()


func _init() -> void:
	_update_generator()


func _draw(to_canvas_item: RID, rect: Rect2) -> void:
	var canvas := get_current_item_drawn()
	canvas.draw_colored_polygon(
		PackedVector2Array([
			rect.position + _rand_point(),
			Vector2(rect.position.x, rect.end.y) + _rand_point(),
			rect.end + _rand_point(),
			Vector2(rect.end.x, rect.position.x) + _rand_point()
		]),
		bg_color
	)


func _rand() -> float:
	return generator.randf_range(-max_distortion_amount, max_distortion_amount)


func _rand_point() -> Vector2:
	return Vector2(_rand(), _rand())


func _update_generator() -> void:
	generator.seed = seed
	generator.state = state
