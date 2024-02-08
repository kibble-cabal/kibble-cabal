extends Node2D

@export var building: Building
@export var line_color: Color = Color.ORANGE
@export var handles_color: Color = Color.GRAY
@export var snap_tolerance: float = 100

var current_wall_index: int:
	get: return building.wall_count - 1 if building else -1

var current_wall: WallRef:
	get: return building.get_wall(current_wall_index) if building else null

var curve := Curve2D.new()

enum Mode {
	POINTS,
	DRAW,
}

var mode := Mode.DRAW



func _ready() -> void:
	if building:
		building.changed.connect(queue_redraw)
		building.add_wall()


func _handle_switch_mode() -> void:
	curve = Curve2D.new()
	building = Building.new()
	building.changed.connect(queue_redraw)
	building.add_wall()


func _input(event: InputEvent) -> void:
	if event is InputEventKey and event.is_pressed():
		match event.keycode:
			KEY_D: mode = Mode.DRAW
			KEY_P: mode = Mode.POINTS
		_handle_switch_mode()
		queue_redraw()
	
	match mode:
		Mode.DRAW: _handle_draw_input(event)
		Mode.POINTS: _handle_points_input(event)


func _handle_draw_input(event: InputEvent) -> void:
	if event.is_action_pressed("click"):
		curve = Curve2D.new()
	
	if event is InputEventScreenDrag:
		curve.add_point(event.position)
		queue_redraw()


func _handle_points_input(event: InputEvent) -> void:
	var wall := current_wall
	if not wall: return
	if event.is_action_pressed("click"):
		var snapped_position = building.snap_to_nearest_wall(event.position, snap_tolerance)
		if wall.has_start(): wall.end = snapped_position
		else: wall.start = snapped_position
		queue_redraw()
	
	if event.is_action_released("click"):
		if wall.is_valid(): building.add_wall()
	
	if event is InputEventScreenDrag:
		if wall.has_end():
			wall.end_handle += event.relative
		elif wall.has_start():
			wall.start_handle += event.relative
		queue_redraw()


func _draw() -> void:
	if not building: return
	_draw_walls()
	_draw_editor()
	
	if mode == Mode.DRAW and curve.point_count >= 2:
		curve = Tessellator.new().smoothed_simplified(curve, 100)
		draw_curve(curve)


func draw_curve(line: Curve2D, points: bool = true, handles: bool = true) -> void:
	draw_polyline(curve.tessellate(6), line_color, 2, true)
	if points:
		for i in range(line.point_count):
			var point := line.get_point_position(i)
			circle(point, line_color, 5)
			if handles:
				var in_handle := line.get_point_in(i)
				var out_handle := line.get_point_out(i)
				circle(point + in_handle, handles_color, 3)
				circle(point + out_handle, handles_color, 3)
				draw_line(point + in_handle, point, handles_color, 2)
				draw_line(point + out_handle, point, handles_color, 2)


func _draw_walls() -> void:
	for i in range(building.wall_count):
		if building.is_wall_valid(i):
			draw_polyline(building.tessellate_wall(i), line_color, 2, true)


func _draw_editor() -> void:
	var wall := current_wall
	if wall:
		if wall.has_start():
			draw_dashed_line(wall.start, wall.start + wall.start_handle, handles_color, 2, 8)
		if wall.has_end():
			draw_dashed_line(wall.end, wall.end + wall.end_handle, handles_color, 2, 8)
		for point in [wall.start + wall.start_handle, wall.end + wall.end_handle]:
			if point.is_finite(): circle(point, handles_color)
		for point in [wall.start, wall.end]:
			if point.is_finite(): circle(point, line_color)


func circle(pos: Vector2, color := Color.WHITE, size := 5) -> void:
	draw_arc(pos, size, 0, deg_to_rad(360), 8, color, 2, true)
