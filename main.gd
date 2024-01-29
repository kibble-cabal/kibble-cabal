extends Node3D

@onready var island := LocationDB.find("Island")
@onready var live_mode := GameModeDB.find("Live")
@onready var live_paused_mode := GameModeDB.find("Live/Paused")

@onready var flower := ItemDB.find_by_id("core/flower")


func _ready() -> void:
	SaveSystem.save_opened.connect(_on_save_opened)
	if SaveSystem.current_save:
		_on_save_opened(SaveSystem.current_save)


func _on_save_opened(_save: SaveResource) -> void:
	await get_tree().process_frame
	LocationSystem.enter(island)
	GameModeSystem.to(live_mode)
	($NavigationRegion3D as NavigationRegion3D).bake_navigation_mesh()


func _debug_remake_location_inventory() -> void:
	var state := SaveSystem.current_save.get_or_create_location_state("Island")
	var item := flower.instantiate()
	item.location = Vector3(1, 0, 1)
	state.inventory = InventoryResource.new()
	state.inventory.item_instances.append(item)
	

func _debug_spawn_room() -> void:
	var room := RoomResource.new()
	room.interior_id = &"build/wall/striped_with_trim"
	room.exterior_id = &"build/wall/basic_siding"
	room.floor_id = &"build/floor/basic_wood"
	for point in [
		Vector2(2, 2),
		Vector2(2, 0),
		Vector2(0, 0),
		Vector2(0, 2),
	]: room.polygon.add_point(point + Vector2(0, 1))
	var spawner := RoomSpawner.new(room)
	spawner.spawn($World)
