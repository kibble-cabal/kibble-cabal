@tool
class_name DroppableArea3D extends Area3D


signal dropped(draggable: DraggableComponent3D, drop_position: Vector3)


## If [code]true[/code], the dropped node will be reparented to this node.
@export var reparent_on_drop: bool = false

## If [code]true[/code], the dropped node will be disabled.
@export var disable_on_drop: bool = false

## If a node is intersecting multiple drop areas, this property decides which area
## will handle the dropped node. Higher priority means this area will be chosen.
@export var drop_priority: int = 0


func _init() -> void:
	collision_layer = Bit.Physics.UI_DROP
	collision_mask = Bit.Physics.UI_DRAG
	monitoring = false
	input_ray_pickable = false


func drop(draggable: DraggableComponent3D, drop_position: Vector3) -> void:
	if reparent_on_drop:
		draggable.node.reparent(self)
	if disable_on_drop:
		draggable.process_mode = PROCESS_MODE_DISABLED
	dropped.emit(draggable, drop_position)
