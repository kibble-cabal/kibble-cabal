extends VBoxContainer

const AddWallUI := preload("add_wall_ui.tscn")
const MoveUI := preload("move_ui.tscn")

const SquareRoomPoints: Array[Vector2] = [
	Vector2(0, 0),
	Vector2(0, 2),
	Vector2(2, 2),
	Vector2(2, 0),
	Vector2(0, 0)
]

@export var building: Building

@onready var world: Node3D = $Spawner
@onready var ui_root := UIConfig.get_game_mode_ui_root()


func _ready() -> void:
	building.DestroyWallRequested.connect(_on_destroy_wall_requested)
	building.DestroyFloorRequested.connect(_on_destroy_floor_requested)
	building.MoveWallRequested.connect(_on_move_wall_requested)
	building.MoveFloorRequested.connect(_on_move_floor_requested)
	
	BuildModeState.get_history().on_after_undo(&"Add Building", _on_undo_add_building)


func _enter_tree() -> void:
	respawn()


func respawn() -> void:
	if not is_node_ready(): await ready
	
	if is_inside_tree():
		if not world.is_node_ready():
			await world.ready
		
		# Remove outdated nodes
		for child in world.get_children(): 
			child.queue_free()
		
		# Spawn new nodes
		BuildingSpawner.new(building).spawn(world)
		for wall_index in range(building.wall_count):
			var spawner := WallUISpawner.new(building)
			spawner.index = wall_index
			spawner.spawn(world)
		for floor_index in range(building.floor_count):
			var spawner := FloorUISpawner.new(building)
			spawner.index = floor_index
			spawner.spawn(world)


func initiate_move(walls: PackedInt32Array, floors: PackedInt32Array) -> void:
	if ui_root:
		var scene := MoveUI.instantiate()
		scene.start_position = Vec3.project_position_to_floor(get_viewport().get_camera_3d(), get_global_mouse_position())
		scene.building = building
		scene.walls = walls
		scene.floors = floors
		ui_root.push(scene)


func _on_create_square_room_button_pressed() -> void:
	var walls: PackedInt32Array = building.add_walls(SquareRoomPoints)
	var floor_index: int = building.add_floor(SquareRoomPoints)
	
	var move_scene := MoveUI.instantiate()
	move_scene.start_position = Vec3.project_position_to_floor(get_viewport().get_camera_3d(), get_global_mouse_position())
	move_scene.building = building
	move_scene.walls = walls
	move_scene.floors = PackedInt32Array([floor_index])
	
	if ui_root:
		ui_root.push(move_scene)


func _on_done_button_pressed() -> void:
	if ui_root: ui_root.pop()


func _on_destroy_wall_requested(wall_index: int) -> void:
	if building.has_wall(wall_index): BuildModeState.get_history().add(
		building,
		&"Destroy Wall",
		building.remove_wall.bind(wall_index),
		building.insert_wall.bind(wall_index, building.get_wall_data(wall_index))
	)


func _on_destroy_floor_requested(floor_index: int) -> void:
	if building.has_floor(floor_index): BuildModeState.get_history().add(
		building,
		&"Destroy Floor",
		building.remove_floor.bind(floor_index),
		building.insert_floor.bind(floor_index, building.get_floor_data(floor_index))
	)


func _on_move_wall_requested(wall_index: int) -> void:
	initiate_move(PackedInt32Array([wall_index]), PackedInt32Array())


func _on_move_floor_requested(floor_index: int) -> void:
	initiate_move(PackedInt32Array(), PackedInt32Array([floor_index]))


func _on_undo_add_building() -> void:
	if ui_root: ui_root.pop()


func _on_add_wall_button_pressed() -> void:
	if ui_root:
		var scene := AddWallUI.instantiate()
		scene.building = building
		ui_root.push(scene)


func _on_add_floor_button_pressed() -> void:
	pass # Replace with function body.
