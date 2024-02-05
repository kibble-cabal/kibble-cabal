@tool
class_name DroppableArea3D extends Area3D

enum SnapMode {
	NONE,
	CUSTOM_MESH,
	POINTS,
}

signal dropped(draggable: DraggableComponent3D, drop_position: Vector3)


## If [code]true[/code], the dropped node will be reparented to this node.
@export var reparent_on_drop: bool = false

## If [code]true[/code], the dropped node will be disabled.
@export var disable_on_drop: bool = false

## If a node is intersecting multiple drop areas, this property decides which area
## will handle the dropped node. Higher priority means this area will be chosen.
@export var drop_priority: int = 0

@export_group("Snapping", "snap_")
@export var snap_mode := SnapMode.NONE:
	set(value):
		snap_mode = value
		notify_property_list_changed()
@export var snap_threshold: float = 0.1

@export_subgroup("Debug", "snap_debug_")
@export var snap_debug_enabled: bool = false
@export var snap_debug_color: Color = Color.RED

var snap_mesh: Mesh
var snap_points: PackedVector3Array


func _init() -> void:
	collision_layer = Bit.Physics.UI_DROP
	collision_mask = Bit.Physics.UI_DRAG
	monitoring = false
	input_ray_pickable = false


func _get_property_list() -> Array[Dictionary]:
	var snap_mesh_property := {
		name = "snap_mesh",
		type = TYPE_OBJECT,
		hint = PROPERTY_HINT_RESOURCE_TYPE,
		hint_string = "Mesh",
		usage = PROPERTY_USAGE_NO_EDITOR,
	}
	var snap_points_property := {
		name = "snap_points",
		type = TYPE_PACKED_VECTOR3_ARRAY,
		usage = PROPERTY_USAGE_NO_EDITOR,
	}
	match snap_mode:
		SnapMode.CUSTOM_MESH: snap_mesh_property.usage = PROPERTY_USAGE_DEFAULT
		SnapMode.POINTS: snap_points_property.usage = PROPERTY_USAGE_DEFAULT
	return [snap_mesh_property, snap_points_property]


func _process(_delta: float) -> void:
	if snap_debug_enabled:
		match snap_mode:
			SnapMode.POINTS:
				var points := snap_points.duplicate()
				for i in range(points.size()):
					points[i] = points[i] * transform.affine_inverse()
				DebugDraw3D.draw_points(points, DebugDraw3D.POINT_TYPE_SQUARE, 0.1, snap_debug_color)
			SnapMode.CUSTOM_MESH: if snap_mesh: Vec3.debug_draw_mesh(snap_mesh, transform, 0.01, snap_debug_color)


func _snap_to_points(global_pos: Vector3) -> Vector3:
	var dist: float = INF
	var local_pos := to_local(global_pos)
	var current_point := local_pos
	for point in snap_points:
		var current_dist := absf(point.distance_to(local_pos))
		if current_dist < dist:
			dist = current_dist
			current_point = point
	return current_point


func _snap_to_mesh(global_pos: Vector3) -> Vector3:
	if snap_mesh: 
		_debug_draw_snap_face(global_pos)
		return Vec3.get_closest_point_on_mesh(snap_mesh, to_local(global_pos))
	return global_pos


func _debug_draw_snap_face(global_pos: Vector3) -> void:
	if snap_debug_enabled:
		var face := Vec3.get_closest_face_on_mesh(snap_mesh, to_local(global_pos))
		for i in range(face.size()):
			face[i] = face[i] * transform.affine_inverse()
		DebugDraw3D.draw_points(face, DebugDraw3D.POINT_TYPE_SQUARE, 0.2, snap_debug_color * 1.1)


func snap(global_pos: Vector3) -> Vector3:
	var snap_pos := global_pos
	match snap_mode:
		SnapMode.POINTS: snap_pos = to_global(_snap_to_points(global_pos))
		SnapMode.CUSTOM_MESH: snap_pos = to_global(_snap_to_mesh(global_pos))
	if absf(snap_pos.distance_to(global_pos)) <= snap_threshold:
		return snap_pos
	return global_pos


func drop(draggable: DraggableComponent3D, drop_position: Vector3) -> void:
	if reparent_on_drop:
		draggable.node.reparent(self)
	if disable_on_drop:
		draggable.process_mode = PROCESS_MODE_DISABLED
	dropped.emit(draggable, drop_position)
