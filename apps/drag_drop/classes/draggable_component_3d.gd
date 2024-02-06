@tool
class_name DraggableComponent3D extends Area3D

enum DropMode {
	ANYWHERE, ## This node can be dropped anywhere, regardless of colliding [DroppableArea3D]s.
	ANY_DROP_AREA, ## This node can be dropped into any colliding [DroppableArea3D].
	SOME_DROP_AREAS, ## This node can be dropped into only [DroppableArea3D] specified in [member drop_areas].
}

signal drag_started
signal drag_finished
signal attempted_drop(drop_position: Vector3)
signal dropped(drop_area: DroppableArea3D, drop_position: Vector3)
signal position_changed(new_position: Vector3, old_position: Vector3)

@export_group("Drop")
@export var drop_mode := DropMode.ANY_DROP_AREA
@export var drop_areas: Array[DroppableArea3D] = []

@export_group("Snapping")
@export var enable_snapping := true

@export_group("Axis Lock", "axis_lock_")
@export var axis_lock_x: bool = false
@export var axis_lock_y: bool = false
@export var axis_lock_z: bool = false

@export_group("Raycasting")
## If [code]true[/code], all user input will be projected to the floor plane. Otherwise, there must
## be a collision object in the scene with layer ui_physics_ray.
@export var simple_raycast: bool = true
## Only applicable if [member simple_raycast] is [code]false[/code].
@export var ray_length: float = 100
@export var debug_show_ray: bool = false

@onready var node: Node3D = get_parent()
@onready var viewport := get_viewport()

var start_position: Vector3
var dragging := Toggle.new(false)

var query := PhysicsRayQueryParameters3D.new()
var query_result := {}


func _init() -> void:
	collision_layer = Bit.Physics.UI_DRAG
	collision_mask = Bit.Physics.UI_DROP
	if not Engine.is_editor_hint():
		input_event.connect(_on_input_event)
		drag_started.connect(_on_drag_started)
		drag_finished.connect(_on_drag_finished)
		dragging.toggled_on.connect(func(): drag_started.emit())
		dragging.toggled_off.connect(func(): drag_finished.emit())


func _ready() -> void:
	start_position = node.position
	
	if not Engine.is_editor_hint():
		query.collide_with_areas = true
		query.collision_mask = Bit.Physics.UI_PHYSICS_RAY


func _unhandled_input(event: InputEvent) -> void:
	if not Engine.is_editor_hint() and dragging.is_true():
		if viewport: viewport.set_input_as_handled()
		if event is InputEventMouseMotion and "position" in query_result:
			set_global_pos(query_result.position)

		if event.is_action_released("click"):
			dragging.to(false)


func _on_input_event(
	_camera: Node, 
	event: InputEvent, 
	_event_position: Vector3, 
	_normal: Vector3, 
	_shape_idx: int
) -> void:
	if not Engine.is_editor_hint():
		if event.is_action_pressed("click"):
			if viewport: viewport.set_input_as_handled()
			dragging.to(true)
		if event.is_action_released("click"):
			if viewport: viewport.set_input_as_handled()
			dragging.to(false)


func _process(_delta: float) -> void:
	if debug_show_ray and dragging.is_true():
		DebugDraw3D.draw_arrow(query.from + Vector3(0.1, 0, 0), query.to, Color.RED, 0.1)


func _physics_process(_delta: float) -> void:
	# Do raycasting
	if not Engine.is_editor_hint() and dragging.is_true():
		var camera := viewport.get_camera_3d()
		var mouse_position := viewport.get_mouse_position()
		query.from = camera.global_position
		if simple_raycast: 
			query.to = Vec3.project_position_to_floor(camera, mouse_position)
			query_result.position = query.to
		else: 
			query.to = camera.project_position(mouse_position, ray_length)
			query_result = get_world_3d().direct_space_state.intersect_ray(query)
	
	# Lerp back to start position when drag failed
	if dragging.is_false() and node.is_node_ready() and not Engine.is_editor_hint():
		node.position = node.position.lerp(start_position, 0.1)


func _on_drag_started() -> void:
	if node: start_position = node.position


func _on_drag_finished() -> void:
	if can_drop():
		var old_position := start_position
		start_position = node.position
		var drop_area := get_drop_area()
		if drop_area:
			drop_area.drop(self, node.position)
		dropped.emit(drop_area, node.position)
		position_changed.emit(node.position, old_position)
	else:
		attempted_drop.emit(node.position)


func snap() -> void:
	if not enable_snapping: return
	var areas := get_drop_areas()
	areas.reverse()
	for area in areas:
		node.global_position = locked(area.snap(node.global_position))


func locked(global_pos: Vector3) -> Vector3:
	if axis_lock_x: global_pos.x = node.global_position.x
	if axis_lock_y: global_pos.y = node.global_position.y
	if axis_lock_z: global_pos.z = node.global_position.z
	return global_pos


func set_global_pos(pos: Vector3) -> void:
	node.global_position = locked(pos)
	snap()


func can_drop() -> bool:
	match drop_mode:
		DropMode.ANYWHERE:
			return true
		DropMode.ANY_DROP_AREA, DropMode.SOME_DROP_AREAS:
			return not get_drop_areas().is_empty()
	return false


func get_drop_area() -> DroppableArea3D:
	if drop_mode != DropMode.ANYWHERE:
		return get_drop_areas().pop_front()
	return null


func get_drop_areas() -> Array[DroppableArea3D]:
	var areas: Array[DroppableArea3D] = []
	for area in get_overlapping_areas(): 
		if area is DroppableArea3D: 
			match drop_mode:
				DropMode.ANY_DROP_AREA: areas.append(area)
				DropMode.SOME_DROP_AREAS:
					if area in drop_areas: areas.append(area)
	areas.sort_custom(
		func(a: DroppableArea3D, b: DroppableArea3D) -> bool:
			return a.drop_priority > b.drop_priority
	)
	return areas
