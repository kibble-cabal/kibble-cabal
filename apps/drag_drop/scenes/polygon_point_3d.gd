@tool
extends Sprite3D

signal position_changed(pos: Vector2)

@export var index: int

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

@export var inactive_modulate: Color = Color.WHITE:
	set(value):
		inactive_modulate = value
		update()

@export var dragging_modulate: Color = Color.WHITE:
	set(value):
		dragging_modulate = value
		update()

var history: History
## If provided, should have signature: func (position: Vector3) -> Vector3
var custom_snap_method = null

@onready var draggable := $Draggable as DraggableComponent3D
@onready var collider := $Draggable/CollisionShape as CollisionShape3D


func _ready() -> void:
	draggable.drag_started.connect(_on_drag_started)
	draggable.drag_finished.connect(_on_drag_finished)
	draggable.position_changed.connect(_on_position_changed)
	modulate = inactive_modulate
	update()


func _on_drag_started() -> void:
	create_tween().tween_property(self, "modulate", dragging_modulate, 0.125)


func _on_drag_finished() -> void:
	create_tween().tween_property(self, "modulate", inactive_modulate, 0.125)


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
	global_position = global_pos
	position_changed.emit(Vec2.from(global_pos))


func calc_pixel_size() -> float:
	if texture:
		var texture_size := texture.get_size()
		var avg := (texture_size.x + texture_size.y) / 2
		return size / avg
	return 0.01


func update() -> void:
	pixel_size = calc_pixel_size()
	if collider:
		(collider.shape as SphereShape3D).radius = size + input_margin
	if draggable:
		draggable.drop_mode = drop_mode
		draggable.drop_areas = drop_areas
		draggable.start_position = position
		draggable.custom_snap_method = custom_snap_method
	if Engine.is_editor_hint():
		modulate = inactive_modulate
