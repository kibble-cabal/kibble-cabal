@tool
extends MeshInstance3D

@export var curve: Curve2D:
	set(value):
		curve = value
		update()

@export var curve_index: int = -1:
	set(value):
		curve_index = value
		update()

@export_group("Behavior")

@export var input_margin: float = 0.05:
	set(value):
		input_margin = value
		update()

@export var drop_mode := DraggableComponent3D.DropMode.ANYWHERE:
	set(value):
		drop_mode = value
		update()

@export var drop_areas: Array[DroppableArea3D] = []:
	set(value):
		drop_areas = value
		update()

@export_group("Appearance")

@export var size: float = 0.5:
	set(value):
		size = value
		update()

@export var modulate: Color = Color.WHITE:
	set(value):
		modulate = value
		update(true)

@export var dragging_modulate: Color = Color.WHITE:
	set(value):
		dragging_modulate = value
		update()

var history: History

var sphere_mesh: SphereMesh:
	get: return mesh as SphereMesh

var material: StandardMaterial3D:
	get: return sphere_mesh.material as StandardMaterial3D

@onready var draggable := $Draggable as DraggableComponent3D
@onready var collider := $Draggable/CollisionShape as CollisionShape3D


func _ready() -> void:
	draggable.drag_started.connect(_on_drag_started)
	draggable.drag_finished.connect(_on_drag_finished)
	draggable.position_changed.connect(_on_position_changed)
	update(true)


func _on_drag_started() -> void:
	if material:
		create_tween().tween_property(material, "albedo_color", dragging_modulate, 0.125)
		create_tween().tween_property(material, "emission", dragging_modulate, 0.125)


func _on_drag_finished() -> void:
	if material:
		create_tween().tween_property(material, "albedo_color", modulate, 0.125)
		create_tween().tween_property(material, "emission", modulate, 0.125)


func _on_position_changed(new_position: Vector3, old_position: Vector3) -> void:
	if history: history.merge_add(self,
		"Move Point",
		move_point.bind(new_position),
		move_point.bind(old_position),
		func can_merge(a: History.Item, b: History.Item) -> bool:
			return a.caller == b.caller
	)
	else: move_point(new_position)


func move_point(global_pos: Vector3) -> void:
	if curve and curve_index >= 0 and curve.point_count > curve_index:
		curve.set_point_position(curve_index, Vec2.from(global_pos))


func update(override_modulate := false) -> void:
	sphere_mesh.radius = size
	sphere_mesh.height = size
	if material and override_modulate:
		material.albedo_color = modulate
		material.emission = modulate
	if collider:
		(collider.shape as SphereShape3D).radius = size + input_margin
	if curve and curve_index >= 0 and curve.point_count > curve_index:
		var position_2d := curve.get_point_position(curve_index)
		position = Vector3(position_2d.x, 0, position_2d.y)
	if draggable:
		draggable.drop_mode = drop_mode
		draggable.drop_areas = drop_areas
		draggable.start_position = position
