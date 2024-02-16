extends Control


const EditingBuildingUI := preload("editing_building_ui.tscn")


@onready var ui_root := UIConfig.get_game_mode_ui_root()
@onready var world: Node3D = $World


func _enter_tree() -> void:
	if LocationSystem.current_state:
		Sig.try_connect(LocationSystem.current_state.spawners_changed, respawn)
	for building in get_buildings():
		Sig.try_connect(building.changed, respawn)
		Sig.try_connect(building.EditRequested, _on_edit_building_requested.bind(building))
		Sig.try_connect(building.DestroyRequested, _on_destroy_building_requested.bind(building))
	respawn()


func _exit_tree() -> void:
	if LocationSystem.current_state:
		Sig.try_disconnect(LocationSystem.current_state.spawners_changed, respawn)
	for building in get_buildings():
		Sig.try_disconnect(building.changed, respawn)
		Sig.disconnect_all_for_object(self, building.EditRequested)
		Sig.disconnect_all_for_object(self, building.DestroyRequested)


func respawn() -> void:
	if not is_node_ready(): await ready
	
	if is_inside_tree():
		if not world.is_node_ready():
			await world.ready
		
		# Remove outdated nodes
		for child in world.get_children():
			child.queue_free()

		# Add buildings and their UI
		for building in get_buildings():
			BuildingUISpawner.new(building).spawn(world)


func get_buildings() -> Array[Building]:
	var buildings: Array[Building] = []
	for spawner in LocationSystem.current_state.spawners:
		if spawner is BuildingSpawner:
			buildings.append(spawner.resource as Building)
	return buildings


func _on_create_building_button_pressed() -> void:
	var building := Building.new()
	var scene := EditingBuildingUI.instantiate()
	scene.building = building
	if ui_root: ui_root.push(scene)


func _on_edit_building_requested(building: Building) -> void:
	var scene := EditingBuildingUI.instantiate()
	scene.building = building
	if ui_root: ui_root.push(scene)


func _on_destroy_building_requested(building: Building) -> void:
	BuildModeState.get_history().add(
		self,
		&"Destroy Building",
		func destroy_building() -> void:
			LocationSystem.current_state.remove_spawners_with_resource(building)
			respawn(),
		func undo_destroy_building() -> void:
			LocationSystem.current_state.add_spawner(BuildingSpawner.new(building))
			respawn()
	)
