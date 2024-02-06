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


func _enter_tree() -> void:
	for room in building.rooms:
		Sig.try_connect(room.edit_requested, _on_edit_room_requested.bind(room))
		Sig.try_connect(room.destroy_requested, _on_destroy_room_requested.bind(room))
	
	respawn()


func _exit_tree() -> void:
	for room in building.rooms:
		Sig.disconnect_all_for_object(self, room.edit_requested)
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
		BuildingSpawner.new(building).spawn(world)
		for room in building.rooms:
			RoomUISpawner.new(room).spawn(world)


func _on_create_square_room_button_pressed() -> void:
	var scene := SelectRoomOriginUI.instantiate()
	var room := RoomResource.new()
	for point in SquareRoomPoints:
		room.polygon.add_point(point)
	scene.building = building
	scene.room = room
	if ui_root: ui_root.push(scene)


func _on_done_button_pressed() -> void:
	var spawner := BuildingSpawner.new(building)
	if LocationSystem.current_state and not LocationSystem.current_state.has_spawners_with_resource(building):
		LocationSystem.current_state.add_spawner(spawner)
	SaveSystem.commit_changes()
	if ui_root: ui_root.pop()


func _on_cancel_button_pressed() -> void:
	if ui_root: ui_root.pop()


func _on_edit_room_requested(room: RoomResource) -> void:
	var scene := EditingRoomUI.instantiate()
	scene.building = building
	scene.room = room
	if ui_root: ui_root.push(scene)


func _on_destroy_room_requested(room: RoomResource) -> void:
	building.rooms.erase(room)
	respawn()
