@tool
class_name DashedBorderStyleBox extends StyleBox

enum LineCap {
	ROUND,
	FLAT,
}

enum BorderPosition {
	INSET,
	OUTSET,
	CENTER
}

@export var bg_color := Color.GRAY:
	set(value):
		bg_color = value
		emit_changed()

@export var draw_center: bool = true:
	set(value):
		draw_center = value
		emit_changed()

@export var dash_size: float = 10.0:
	set(value):
		dash_size = maxf(value, 0.1)
		emit_changed()

@export var gap_size: float = 10.0:
	set(value):
		gap_size = maxf(value, 0.1)
		emit_changed()

@export var border_color := Color.WHITE:
	set(value):
		border_color = value
		emit_changed()

@export var line_caps := LineCap.FLAT:
	set(value):
		line_caps = value
		emit_changed()

@export var border_position := BorderPosition.CENTER:
	set(value):
		border_position = value
		emit_changed()


@export_group("Border Width", "border_width_")

@export var border_width_top: float = 0.0:
	set(value):
		border_width_top = value
		emit_changed()

@export var border_width_right: float = 0.0:
	set(value):
		border_width_right = value
		emit_changed()

@export var border_width_bottom: float = 0.0:
	set(value):
		border_width_bottom = value
		emit_changed()

@export var border_width_left: float = 0.0:
	set(value):
		border_width_left = value
		emit_changed()

@export_group("Border Radius", "border_radius_")

@export var border_radius_top_left: float = 0.0:
	set(value):
		border_radius_top_left = value
		emit_changed()

@export var border_radius_top_right: float = 0.0:
	set(value):
		border_radius_top_right = value
		emit_changed()

@export var border_radius_bottom_left: float = 0.0:
	set(value):
		border_radius_bottom_left = value
		emit_changed()

@export var border_radius_bottom_right: float = 0.0:
	set(value):
		border_radius_bottom_right = value
		emit_changed()


func _draw(_to_canvas_item: RID, rect: Rect2) -> void:
	var canvas_item := get_current_item_drawn()
	
	var base_rect := rect.grow_individual(-border_width_left / 2, -border_width_top / 2, -border_width_right / 2, -border_width_bottom / 2)
	
	if draw_center: 
		match border_position:
			BorderPosition.CENTER: canvas_item.draw_rect(base_rect, bg_color)
			BorderPosition.OUTSET: canvas_item.draw_rect(rect.grow_individual(-border_width_left, -border_width_top, -border_width_right, -border_width_bottom), bg_color)
			BorderPosition.INSET: canvas_item.draw_rect(rect, bg_color)
	
	var num_dashes_x := ceili(base_rect.size.x / (dash_size + gap_size))
	var num_dashes_tl := ceili(border_radius_top_left / (dash_size + gap_size) * (PI / 2))
	var num_dashes_bl := ceili(border_radius_bottom_left / (dash_size + gap_size) * (PI / 2))
	
	for i in num_dashes_x:
		if i >= ceili(float(num_dashes_tl) / 2):
			draw_dash(canvas_item, i, i == num_dashes_x, base_rect.position, Vector2(base_rect.end.x, base_rect.position.y), border_width_top)
		draw_dash(canvas_item, i, i == num_dashes_x, Vector2(base_rect.position.x, base_rect.end.y), base_rect.end, border_width_bottom)
	
	var num_dashes_y := ceili(base_rect.size.y / (dash_size + gap_size))

	for i in num_dashes_y:
		draw_dash(canvas_item, i, i == num_dashes_y, base_rect.position, Vector2(base_rect.position.x, base_rect.end.y), border_width_left)
		draw_dash(canvas_item, i, i == num_dashes_y, Vector2(base_rect.end.x, base_rect.position.y), base_rect.end, border_width_right)

	if num_dashes_tl > 0:
		var offset := border_radius_top_left + border_width_top / 2
		draw_arc_dashes(canvas_item, base_rect.position + Vector2(offset, offset), border_radius_top_left, deg_to_rad(-180), num_dashes_tl, border_width_top)
	if num_dashes_bl > 0:
		var offset := border_radius_bottom_left + border_width_bottom / 2
		draw_arc_dashes(canvas_item, Vector2(base_rect.position.x + offset, base_rect.end.y - offset), border_radius_bottom_left, deg_to_rad(-270), num_dashes_bl, border_width_bottom)


func draw_dash(canvas_item: CanvasItem, i: int, is_last: bool, start_position: Vector2, end_position: Vector2, border_width: float) -> void:
	var delta: float = (dash_size + gap_size) * i
	var direction := start_position.direction_to(end_position).abs()
	var p1 := start_position + direction * delta
	var p2 := p1 + direction * dash_size
	
	# Make sure dash does not extend past end
	p2.x = minf(p2.x, end_position.x)
	p2.y = minf(p2.y, end_position.y)
	
	# Make sure line caps do not extend past edges
	if line_caps != LineCap.FLAT and dash_size >= border_width / 2:
		if i == 0: p1 += direction * border_width * 0.5
		if is_last: p2 -= direction * border_width * 0.5
	
	canvas_item.draw_line(p1, p2, border_color, border_width)
	
	# Draw rounded line caps
	match line_caps:
		LineCap.ROUND:
			canvas_item.draw_circle(p1, border_width / 2, border_color)
			canvas_item.draw_circle(p2, border_width / 2, border_color)


func draw_arc_dashes(canvas_item: CanvasItem, center: Vector2, radius: float, start_angle: float, num_dashes: int, border_width: float) -> void:
	var ratio := gap_size / dash_size

	# Adds a gap between start of arc and border
	var start_angle_inset := (deg_to_rad(90) / num_dashes) * ratio * 0.5
	start_angle += start_angle_inset
	
	var total_angle_interval := (deg_to_rad(90) - start_angle_inset * 2) / num_dashes

	for i in num_dashes:
		var angle1 := start_angle + total_angle_interval * i
		var angle2 := angle1 + total_angle_interval - total_angle_interval * ratio * 0.5
		canvas_item.draw_arc(center, radius, angle1, angle2, 2, border_color, border_width)

		match line_caps:
			LineCap.ROUND:
				canvas_item.draw_circle(Vec2.get_point_on_circle(center, radius, angle1), border_width / 2, border_color)
				canvas_item.draw_circle(Vec2.get_point_on_circle(center, radius, angle2), border_width / 2, border_color)
