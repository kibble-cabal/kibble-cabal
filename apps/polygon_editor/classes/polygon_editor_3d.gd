class_name PolygonEditor3D extends Node3D

const PointScene := preload("../scenes/polygon_point_3d.tscn")

@export var curve: Curve2D:
	set(value):
		curve = value
		_on_curve_changed()

@export_group("Behavior")

@export var input_margin: float = 0.05:
	set(value):
		input_margin = value
		update_point_nodes()

@export var drop_mode := DraggableComponent3D.DropMode.ANYWHERE:
	set(value):
		drop_mode = value
		update_point_nodes()

@export var drop_areas: Array[DroppableArea3D] = []:
	set(value):
		drop_areas = value
		update_point_nodes()

@export_group("Appearance")

@export var size: float = 0.5:
	set(value):
		size = value
		update_point_nodes()

@export var modulate: Color = Color.WHITE:
	set(value):
		modulate = value
		update_point_nodes()

@export var dragging_modulate: Color = Color.WHITE:
	set(value):
		dragging_modulate = value
		update_point_nodes()


var points: Array[Node3D] = []


func _ready() -> void:
	_on_curve_changed()
	update_point_list()


func update_point_list() -> void:
	if not curve: return
	for i in range(curve.point_count):
		if points.size() > i:
			points[i].curve_index = i
		else:
			var scene := PointScene.instantiate()
			scene.curve_index = i
			points.append(scene)
			add_child(scene)
	
	if points.size() > curve.point_count:
		for i in range(curve.point_count, points.size()):
			points[i].queue_free()
		points.resize(curve.point_count)
	
	update_point_nodes()


func update_point_nodes() -> void:
	for point in points:
		point.curve = curve
		point.size = size
		point.modulate = modulate
		point.dragging_modulate = dragging_modulate
		point.drop_areas = drop_areas
		point.drop_mode = drop_mode
		point.input_margin = input_margin
		

func _on_curve_changed() -> void:
	if curve:
		Sig.try_connect(curve.changed, update_point_list)
	if is_node_ready():
		update_point_list()
