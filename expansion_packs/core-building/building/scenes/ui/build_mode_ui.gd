extends Control


const EditingBuildingUI := preload("editing_building_ui.tscn")

@onready var ui_root := UIConfig.get_game_mode_ui_root()
@onready var world: Node3D = $World


func _enter_tree() -> void:
	for building in get_buildings():
		Sig.try_connect(building.edit_requested, _on_edit_building_requested.bind(building))
		Sig.try_connect(building.destroy_requested, _on_destroy_building_requested.bind(building))
	respawn()


func _exit_tree() -> void:
	for building in get_buildings():
		Sig.disconnect_all_for_object(self, building.edit_requested)
		Sig.disconnect_all_for_object(self, building.destroy_requested)


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
			BuildingSpawner.new(building).spawn(world)
			BuildingUISpawner.new(building).spawn(world)


func get_buildings() -> Array[BuildingResource]:
	var buildings: Array[BuildingResource] = []
	for spawner in LocationSystem.current_state.spawners:
		if spawner is BuildingSpawner:
			buildings.append(spawner.resource as BuildingResource)
	return buildings


func _on_create_building_button_pressed() -> void:
	var scene := EditingBuildingUI.instantiate()
	scene.building = BuildingResource.new()
	if ui_root: ui_root.push(scene)


func _on_edit_building_requested(building: BuildingResource) -> void:
	var scene := EditingBuildingUI.instantiate()
	scene.building = building
	if ui_root: ui_root.push(scene)


func _on_destroy_building_requested(building: BuildingResource) -> void:
	LocationSystem.current_state.remove_spawners_with_resource(building)
	respawn()
