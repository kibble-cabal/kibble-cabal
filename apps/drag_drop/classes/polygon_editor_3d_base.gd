class_name PolygonEditor3DBase extends Node3D

signal point_changed(index: int, pos: Vector2)

const AddPointScene := preload("../scenes/add_point_button.tscn")
const RemovePointScene := preload("../scenes/remove_point_button.tscn")

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
		_points.map(update_point)

@export var drop_mode := DraggableComponent3D.DropMode.ANYWHERE:
	set(value):
		drop_mode = value
		_points.map(update_point)

@export var drop_areas: Array[DroppableArea3D] = []:
	set(value):
		drop_areas = value
		_points.map(update_point)

@export_group("Appearance")

@export var size: float = 0.5:
	set(value):
		size = value
		_points.map(update_point)

@export var modulate: Color = Color.WHITE:
	set(value):
		modulate = value
		_points.map(update_point)

@export var dragging_modulate: Color = Color.WHITE:
	set(value):
		dragging_modulate = value
		_points.map(update_point)


var history: History
## If provided, should have signature: func (position: Vector3) -> Vector3
var custom_snap_method = null
var _points: Array[Node3D] = []
var _add_buttons: Array[Button] = []
var _remove_buttons: Array[Button] = []


func _ready() -> void:
	update_point_list()


func _get_point_scene() -> PackedScene: return null
func _get_point_count() -> int: return 0
func _get_point_position(_index: int) -> Vector2: return Vector2.ZERO
func _set_point_position(_index: int, _pos: Vector2) -> void: pass
func _add_point(_index: int, _pos: Vector2) -> void: pass
func _remove_point(_index: int) -> void: pass


func set_point_position(index: int, pos: Vector2) -> void:
	_set_point_position(index, pos)
	point_changed.emit(index, pos)


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
	update_node_list(
		_points,
		_get_point_count(),
		func(node: Node3D, i: int) -> void: 
			node.index = i
			update_point(node),
		func(_i: int) -> Node3D: return _get_point_scene().instantiate()
	)
	update_add_button_list()
	update_remove_button_list()


func get_midpoint(a_index: int, b_index: int) -> Vector2:
	var a := _get_point_position(a_index)
	var b := _get_point_position(b_index)
	return a.lerp(b, 0.5)


func update_add_button_list() -> void:
	update_node_list(
		_add_buttons,
		_get_point_count() if enable_add_points else 0,
		func(node: Button, i: int) -> void:
			var midpoint := get_midpoint(i, (i - 1) if i > 0 else _get_point_count() - 1)
			node.local_position = Vec3.from(midpoint, size * 3)
			node.set_meta(&"point_index", i),
		func(_i: int) -> Button:
			var node: Button = AddPointScene.instantiate()
			node.pressed.connect(self._on_add_button_pressed.bind(node))
			return node
	)


func update_remove_button_list() -> void:
	update_node_list(
		_remove_buttons,
		_get_point_count() if enable_remove_points else 0,
		func(node: Button, i: int) -> void:
			node.local_position = Vec3.from(_get_point_position(i), size * 3)
			node.set_meta(&"point_index", i),
		func(_i: int) -> Button:
			var node: Button = RemovePointScene.instantiate()
			node.pressed.connect(self._on_remove_button_pressed.bind(node))
			return node
	)


func update_point(point: Node) -> void:
	Sig.disconnect_all_for_object(self, point.position_changed)
	point.position = Vec3.from(_get_point_position(point.index))
	point.history = history
	point.size = size
	point.inactive_modulate = modulate
	point.dragging_modulate = dragging_modulate
	point.drop_areas = drop_areas
	point.drop_mode = drop_mode
	point.input_margin = input_margin
	point.custom_snap_method = custom_snap_method
	point.position_changed.connect(
		func(pos: Vector2) -> void: set_point_position(point.index, pos)
	)


func add_point(index: int, pos := Vector2.INF) -> void:
	if index >= 0 and index <= _get_point_count():
		var midpoint := get_midpoint(index, (index - 1) if index > 0 else _get_point_count() - 1)
		var point_pos := pos if pos.is_finite() else midpoint
		_add_point(index, point_pos)
		update_point_list()


func remove_point(index: int) -> void:
	if index >= 0 and index < _get_point_count():
		_remove_point(index)
		update_point_list()


func _on_add_button_pressed(button: Node) -> void:
	var index: int = button.get_meta(&"point_index", -1)
	if index != -1 and index < _get_point_count():
		if history: history.add(self,
			"Add Point",
			add_point.bind(index),
			remove_point.bind(index)
		)
		else: add_point(index)


func _on_remove_button_pressed(button: Node) -> void:
	var index: int = button.get_meta(&"point_index", -1)
	if index != -1 and index < _get_point_count():
		if history: history.add(self,
			"Remove Point",
			remove_point.bind(index),
			add_point.bind(index, _get_point_position(index))
		)
		else: remove_point(index)
	
