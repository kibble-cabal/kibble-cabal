extends Control

const EventNames = {
	BuildingPicked = &"building_picked",
	RoomPicked = &"room_picked",
	RoomEdited = &"room_edited",
	BuildingCreated = &"building_created",
	RoomCreated = &"room_created",
	RoomShapePicked = &"room_shape_picked",
	RoomLocationPicked = &"room_location_picked",
	BuildingDestroyed = &"building_destroyed",
	RoomDestroyed = &"room_destroyed"
}

@onready var chart := $StateChart as StateChart

var selected_building: BuildingResource
var selected_room: RoomResource

var state: BuildModeState:
	get: return BuildingConfig.get_state()


func create_building() -> void:
	selected_building = BuildingResource.new()
	chart.send_event(EventNames.BuildingCreated)


func select_building(building: BuildingResource) -> void:
	selected_building = building
	chart.send_event(EventNames.BuildingPicked)


func create_room() -> void:
	if selected_building:
		selected_room = RoomResource.new()
		selected_building.rooms.append(selected_room)
		chart.send_event(EventNames.RoomCreated)


func select_room(room: RoomResource) -> void:
	selected_room = room
	chart.send_event(EventNames.RoomPicked)


func pick_room_shape(polygon: Curve2D) -> void:
	if selected_room:
		selected_room.polygon = polygon
		chart.send_event(EventNames.RoomShapePicked)


func pick_room_location(location: Vector2) -> void:
	if selected_room:
		selected_room.origin = location
		chart.send_event(EventNames.RoomLocationPicked)


func finish_editing_room() -> void:
	if selected_room:
		selected_room = null
		chart.send_event(EventNames.RoomEdited)


func destroy_building(building: BuildingResource) -> void:
	selected_building = null
	chart.send_event(EventNames.BuildingDestroyed)


func destroy_room(room: RoomResource) -> void:
	selected_room = null
	chart.send_event(EventNames.RoomDestroyed)


func any_states_in_group_active(group_name: StringName) -> bool:
	return Nodes.get_children_in_group(chart, group_name).any(func(state: State) -> bool: return state.active)


func _can_create_room() -> bool:
	return any_states_in_group_active(&"can_create_room_during_state")


func _can_create_building() -> bool:
	return any_states_in_group_active(&"can_create_building_during_state")
