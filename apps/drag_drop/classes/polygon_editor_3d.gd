class_name PolygonEditor3D extends Node3D

const PointScene := preload("../scenes/polygon_point_3d.tscn")
const AddPointScene := preload("../scenes/add_point_button.tscn")
const RemovePointScene := preload("../scenes/remove_point_button.tscn")

@export var curve: Curve2D:
	set(value):
		curve = value
		_on_curve_changed()

@export_group("Behavior")

@export var enable_add_points: bool = false:
	set(value):
		enable_add_points = value
		update_add_button_list()

@export var enable_remove_points: bool = false:
	set(value):
		enable_remove_points = value
		update_remove_button_list()

@export var input_margin: float = 0.05:
	set(value):
		input_margin = value
		points.map(update_point)

@export var drop_mode := DraggableComponent3D.DropMode.ANYWHERE:
	set(value):
		drop_mode = value
		points.map(update_point)

@export var drop_areas: Array[DroppableArea3D] = []:
	set(value):
		drop_areas = value
		points.map(update_point)

@export_group("Appearance")

@export var size: float = 0.5:
	set(value):
		size = value
		points.map(update_point)

@export var modulate: Color = Color.WHITE:
	set(value):
		modulate = value
		points.map(update_point)

@export var dragging_modulate: Color = Color.WHITE:
	set(value):
		dragging_modulate = value
		points.map(update_point)


var history: History
var points: Array[Node3D] = []
var add_buttons: Array[Button] = []
var remove_buttons: Array[Button] = []


func _ready() -> void:
	_on_curve_changed()
	update_point_list()


func update_node_list(list: Array, max_size: int, update_node: Callable, make_node: Callable) -> void:	
	if list.size() > max_size:
		for i in range(max_size, list.size()):
			list[i].queue_free()
	list.resize(max_size)
	for i in range(max_size):
		if list[i] != null: update_node.call(list[i], i)
		else:
			list[i] = make_node.call(i)
			add_child(list[i])
			update_node.call(list[i], i)


func update_point_list() -> void:
	if not curve: return
	update_node_list(
		points,
		curve.point_count,
		func(node: Node3D, i: int) -> void: 
			node.curve_index = i
			update_point(node),
		func(_i: int) -> Node3D: return PointScene.instantiate()
	)
	update_add_button_list()
	update_remove_button_list()


func get_midpoint(a_index: int, b_index: int) -> Vector2:
	var a := curve.get_point_position(a_index)
	var b := curve.get_point_position(b_index)
	return a.lerp(b, 0.5)


func update_add_button_list() -> void:
	if not curve or curve.point_count == 0: return
	update_node_list(
		add_buttons,
		curve.point_count if enable_add_points else 0,
		func(node: Button, i: int) -> void:
			var midpoint := get_midpoint(i, (i - 1) if i > 0 else curve.point_count - 1)
			node.local_position = Vec3.from(midpoint, size * 3)
			node.set_meta(&"curve_index", i),
		func(_i: int) -> Button:
			var node: Button = AddPointScene.instantiate()
			node.pressed.connect(self._on_add_button_pressed.bind(node))
			return node
	)


func update_remove_button_list() -> void:
	if not curve: return
	update_node_list(
		remove_buttons,
		curve.point_count if enable_remove_points else 0,
		func(node: Button, i: int) -> void:
			node.local_position = Vec3.from(curve.get_point_position(i), size * 3)
			node.set_meta(&"curve_index", i),
		func(_i: int) -> Button:
			var node: Button = RemovePointScene.instantiate()
			node.pressed.connect(self._on_remove_button_pressed.bind(node))
			return node
	)


func update_point(point: Node) -> void:
	point.history = history
	point.curve = curve
	point.size = size
	point.modulate = modulate
	point.dragging_modulate = dragging_modulate
	point.drop_areas = drop_areas
	point.drop_mode = drop_mode
	point.input_margin = input_margin


func add_point(index: int, pos := Vector2.INF) -> void:
	if curve and index >= 0 and index <= curve.point_count:
		var midpoint := get_midpoint(index, (index - 1) if index > 0 else curve.point_count - 1)
		var point_pos := pos if pos.is_finite() else midpoint
		curve.add_point(point_pos, Vector2.ZERO, Vector2.ZERO, index)
		update_point_list()


func remove_point(index: int) -> void:
	if curve and index >= 0 and index < curve.point_count:
		curve.remove_point(index)
		update_point_list()


func _on_curve_changed() -> void:
	if curve:
		Sig.try_connect(curve.changed, update_point_list)
	if is_node_ready():
		update_point_list()


func _on_add_button_pressed(button: Node) -> void:
	var index: int = button.get_meta(&"curve_index", -1)
	if index != -1 and index < curve.point_count:
		if history: history.add(self,
			"Add Point",
			add_point.bind(index),
			remove_point.bind(index)
		)
		else: add_point(index)


func _on_remove_button_pressed(button: Node) -> void:
	var index: int = button.get_meta(&"curve_index", -1)
	if index != -1 and index < curve.point_count:
		if history: history.add(self,
			"Remove Point",
			remove_point.bind(index),
			add_point.bind(index, curve.get_point_position(index))
		)
		else: remove_point(index)
	
