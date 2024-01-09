@tool
@icon("pill_range_icon.svg")
class_name PillRange extends Range

enum Direction {
	LEFT_TO_RIGHT,
	RIGHT_TO_LEFT,
	TOP_TO_BOTTOM,
	BOTTOM_TO_TOP,
}

@export var direction: Direction = Direction.LEFT_TO_RIGHT:
	set(direction_value):
		direction = direction_value
		queue_redraw()

@export var max_pill_count: int = 100:
	set(max_pill_count_value):
		max_pill_count = max_pill_count_value
		queue_redraw()

@export_group("Theme Overrides", "theme_override")

@export_subgroup("StyleBoxes", "theme_override_stylebox")
@export var theme_override_stylebox_empty: StyleBox:
	set(override_value):
		theme_override_stylebox_empty = override_value
		queue_redraw()

@export var theme_override_stylebox_filled: StyleBox:
	set(override_value):
		theme_override_stylebox_filled = override_value
		queue_redraw()

@export_subgroup("Constants", "theme_override_constant")
@export var theme_override_constant_v_separation: int = -1:
	set(override_value):
		theme_override_constant_v_separation = override_value
		queue_redraw()

@export var theme_override_constant_h_separation: int = -1:
	set(override_value):
		theme_override_constant_h_separation = override_value
		queue_redraw()

@export var theme_override_constant_pill_min_width: int = -1:
	set(override_value):
		theme_override_constant_pill_min_width = override_value
		queue_redraw()

@export var theme_override_constant_pill_min_height: int = -1:
	set(override_value):
		theme_override_constant_pill_min_height = override_value
		queue_redraw()

var _theme_stylebox_empty: StyleBox:
	get: return get_theme_stylebox("empty", "PillRange")
var _theme_stylebox_filled: StyleBox:
	get: return get_theme_stylebox("filled", "PillRange")
var _theme_constant_h_separation: int:
	get: return get_theme_constant("h_separation", "PillRange")
var _theme_constant_v_separation: int:
	get: return get_theme_constant("v_separation", "PillRange")
var _theme_constant_pill_min_width: int:
	get: return get_theme_constant("pill_min_width", "PillRange")
var _theme_constant_pill_min_height: int:
	get: return get_theme_constant("pill_min_height", "PillRange")

var num_pills: int:
	get:
		if is_zero_approx(step): return max_pill_count
		var num_steps := ceili(abs((max_value - min_value) / step))
		return mini(num_steps, max_pill_count)

var num_pills_filled: int:
	get: 
		if is_zero_approx(step): return max_pill_count
		var num_steps := ceili(abs((value - min_value) / step))
		return mini(num_steps, max_pill_count)

# Theme

var stylebox_empty: StyleBox:
	get:
		if theme_override_stylebox_empty: return theme_override_stylebox_empty
		return _theme_stylebox_empty

var stylebox_filled: StyleBox:
	get:
		if theme_override_stylebox_filled: return theme_override_stylebox_filled
		return _theme_stylebox_filled

var h_separation: int:
	get:
		if theme_override_constant_h_separation > 0: return theme_override_constant_h_separation
		if _theme_constant_h_separation: return _theme_constant_h_separation
		return 0

var v_separation: int:
	get:
		if theme_override_constant_v_separation > 0: return theme_override_constant_v_separation
		if _theme_constant_v_separation: return _theme_constant_v_separation
		return 0

var pill_min_width: int:
	get:
		if theme_override_constant_pill_min_width > 0: return theme_override_constant_pill_min_width
		if _theme_constant_pill_min_width: return _theme_constant_pill_min_width
		return 0

var pill_min_height: int:
	get:
		if theme_override_constant_pill_min_height > 0: return theme_override_constant_pill_min_height
		if _theme_constant_pill_min_height: return _theme_constant_pill_min_height
		return 0

var min_size: Vector2:
	get:
		var pills_size := Vector2(pill_min_width, pill_min_height)
		match direction:
			Direction.LEFT_TO_RIGHT, Direction.RIGHT_TO_LEFT:
				return pills_size * Vector2(num_pills, 1) + Vector2(h_separation, 0) * maxi(0, num_pills - 1)
			_: return pills_size * Vector2(1, num_pills) + Vector2(0, v_separation) * maxi(0, num_pills - 1)

var pill_size: Vector2:
	get: match direction:
			Direction.LEFT_TO_RIGHT, Direction.RIGHT_TO_LEFT: return Vector2(
				(size.x / float(num_pills)) - (float(h_separation) / 2),
				size.y
			)
			_: return Vector2(
				size.x,
				(size.y / float(num_pills)) - (float(v_separation) / 2)
			)


func _draw() -> void:
	for index in num_pills:
		if _is_filled(index):
			if stylebox_filled: draw_style_box(stylebox_filled, _get_pill_rect(index))
		else:
			if stylebox_empty: draw_style_box(stylebox_empty, _get_pill_rect(index))


func _get_minimum_size() -> Vector2:
	return min_size


func _get_pill_position(index: int) -> Vector2:
	var pill_pos := Vector2.ZERO
	match direction:
		Direction.LEFT_TO_RIGHT, Direction.RIGHT_TO_LEFT:
			pill_pos.x = (size.x / float(num_pills)) * index + (float(h_separation) / 4)
		_: pill_pos.y = (size.y / float(num_pills)) * index + (float(v_separation) / 4)
	return pill_pos


func _get_pill_rect(index: int) -> Rect2:
	return Rect2(_get_pill_position(index), pill_size)


func _is_filled(index: int) -> bool:
	match direction:
		Direction.LEFT_TO_RIGHT, Direction.TOP_TO_BOTTOM: return index < num_pills_filled
		Direction.RIGHT_TO_LEFT, Direction.BOTTOM_TO_TOP: return index >= num_pills_filled
	return false
