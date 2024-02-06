extends VBoxContainer

const SelectRoomOriginUI := preload("select_room_origin_ui.tscn")
const EditingRoomUI := preload("editing_room_ui.tscn")

const SquareRoomPoints: Array[Vector2] = [
	Vector2(0, 0),
	Vector2(0, 2),
	Vector2(2, 2),
	Vector2(2, 0)
]

@export var building: BuildingResource

@onready var world: Node3D = $Spawner
@onready var ui_root := UIConfig.get_game_mode_ui_root()


func _ready() -> void:
	$DesignBuildingUI.building = building
	$DesignBuildingUI.update()
	building.changed.connect(respawn)
	
	BuildModeState.get_history().on_after_undo(&"Add Building", _on_undo_add_building)


func _enter_tree() -> void:
	for room in building.rooms:
		Sig.try_connect(room.edit_requested, _on_edit_room_requested.bind(room))
		Sig.try_connect(room.move_requested, _on_move_room_requested.bind(room))
		Sig.try_connect(room.destroy_requested, _on_destroy_room_requested.bind(room))
	
	respawn()


func _exit_tree() -> void:
	for room in building.rooms:
		Sig.disconnect_all_for_object(self, room.edit_requested)
		Sig.disconnect_all_for_object(self, room.move_requested)
		Sig.disconnect_all_for_object(self, room.destroy_requested)


func respawn() -> void:
	if not is_node_ready(): await ready
	
	if is_inside_tree():
		if not world.is_node_ready():
			await world.ready
		
		# Remove outdated nodes
		for child in world.get_children(): 
			child.queue_free()
		
		# Spawn new nodes
		for room in building.rooms:
			RoomUISpawner.new(room).spawn(world)


func _on_create_square_room_button_pressed() -> void:
	var room := RoomResource.new()
	for point in SquareRoomPoints:
		room.polygon.add_point(point)
	var edit_scene := EditingRoomUI.instantiate()
	edit_scene.building = building
	edit_scene.room = room
	var move_scene := SelectRoomOriginUI.instantiate()
	move_scene.building = building
	move_scene.room = room
	if ui_root:
		ui_root.push(edit_scene)
		ui_root.push(move_scene)


func _on_done_button_pressed() -> void:
	if ui_root: ui_root.pop()


func _on_edit_room_requested(room: RoomResource) -> void:
	var scene := EditingRoomUI.instantiate()
	scene.building = building
	scene.room = room
	if ui_root: ui_root.push(scene)


func _on_move_room_requested(room: RoomResource) -> void:
	var scene := SelectRoomOriginUI.instantiate()
	scene.building = building
	scene.room = room
	if ui_root: ui_root.push(scene)


func _on_destroy_room_requested(room: RoomResource) -> void:
	if building.has_room(room): BuildModeState.get_history().add(
		building,
		"Destroy Room",
		building.remove_room.bind(room),
		building.add_room.bind(room)
	)


func _on_undo_add_building() -> void:
	if ui_root: ui_root.pop()
