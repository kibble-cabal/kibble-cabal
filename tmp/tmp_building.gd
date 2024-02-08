extends Node2D

@export var building: Building
@export var line_color: Color = Color.ORANGE
@export var handles_color: Color = Color.GRAY
@export var fill_color: Color = Color.DIM_GRAY
@export var snap_tolerance: float = 100

var current_wall: WallRef:
	get: return building.get_wall(building.wall_count - 1) if building else null

var current_floor: FloorRef:
	get: return building.get_floor(building.floor_count - 1) if building else null

var curve := Curve2D.new()

enum Mode {
	WALLS,
	FLOORS,
	CURVE,
}

var mode := Mode.CURVE


func _ready() -> void:
	if building:
		building.changed.connect(queue_redraw)
		building.add_wall()


func _handle_switch_mode() -> void:
	curve = Curve2D.new()
	building = Building.new()
	building.changed.connect(queue_redraw)
	building.add_wall()
	building.add_floor()


func _input(event: InputEvent) -> void:
	if event is InputEventKey and event.is_pressed():
		match event.keycode:
			KEY_C: mode = Mode.CURVE
			KEY_W: mode = Mode.WALLS
			KEY_F: mode = Mode.FLOORS
		_handle_switch_mode()
		queue_redraw()
	
	match mode:
		Mode.CURVE: _handle_curve_input(event)
		Mode.WALLS: _handle_walls_input(event)
		Mode.FLOORS: _handle_floors_input(event)


func _handle_curve_input(event: InputEvent) -> void:
	if event.is_action_pressed("click"):
		curve = Curve2D.new()
	
	if event is InputEventScreenDrag:
		curve.add_point(event.position)
		queue_redraw()


func _handle_walls_input(event: InputEvent) -> void:
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


func _handle_floors_input(event: InputEvent) -> void:
	var floor := current_floor
	if not floor: return
	if event.is_action_pressed("click"):
		var snapped_position = building.snap_to_nearest_wall(event.position, snap_tolerance)
		floor.add_point(snapped_position)
		queue_redraw()
	
	if event is InputEventScreenDrag:
		var delta: Vector2 = event.position - floor.get_position(floor.point_count - 1)
		floor.set_handles(floor.point_count - 1, delta.rotated(PI), delta)
	
	if event is InputEventKey:
		if event.is_pressed() and event.keycode == KEY_ENTER:
			building.add_floor()


func _draw() -> void:
	match mode: 
		Mode.CURVE: 
			if curve.point_count >= 2:
				curve = Tessellator.new().smoothed_simplified(curve, 200, PI/16)
				draw_curve(curve)
		Mode.WALLS:
			if building:
				_draw_walls()
				_draw_walls_editor()
		Mode.FLOORS:
			if building:
				_draw_floors()
				_draw_floors_editor()
			


func draw_curve(line: Curve2D, points: bool = true, handles: bool = true) -> void:
	draw_polyline(curve.tessellate(6), line_color, 2, true)
	if points:
		for i in range(line.point_count):
			var point := line.get_point_position(i)
			if handles:
				var in_handle := line.get_point_in(i)
				var out_handle := line.get_point_out(i)
				draw_point_and_handles(point, in_handle, out_handle)
			else:
				circle(point, line_color, 5)


func _draw_walls() -> void:
	for i in range(building.wall_count):
		if building.is_wall_valid(i):
			draw_polyline(building.tessellate_wall(i), line_color, 2, true)


func _draw_walls_editor() -> void:
	var wall := current_wall
	if wall:
		draw_point_and_handles(wall.start, wall.start_handle, Vector2.INF)
		draw_point_and_handles(wall.end, wall.end_handle, Vector2.INF)


func draw_point_and_handles(point: Vector2, in_handle: Vector2, out_handle: Vector2) -> void:
	if point.is_finite(): 
		if in_handle.is_finite(): 
			draw_dashed_line(point, point + in_handle, handles_color, 2, 8)
			circle(point + in_handle, handles_color)
		if out_handle.is_finite(): 
			draw_dashed_line(point, point + out_handle, handles_color, 2, 8)
			circle(point + out_handle, handles_color)
		circle(point, line_color)


func _draw_floors() -> void:
	for i in range(building.floor_count):
		var floor: FloorRef = building.get_floor(i)
		if floor.point_count < 2: continue
		if building.is_floor_valid(i):
			draw_colored_polygon(floor.tessellate(true), fill_color)
		else:
			draw_polyline(floor.tessellate(true), Color.RED, 2, true)	


func _draw_floors_editor() -> void:
	var floor := current_floor
	if floor and floor.point_count > 0:
		for i in range(floor.point_count):
			draw_point_and_handles(
				floor.get_position(i),
				floor.get_in_handle(i), 
				floor.get_out_handle(i)
			)
		for point in floor.get_point_positions():
			circle(point, line_color)


func circle(pos: Vector2, color := Color.WHITE, size := 5) -> void:
	draw_arc(pos, size, 0, deg_to_rad(360), 8, color, 2, true)
