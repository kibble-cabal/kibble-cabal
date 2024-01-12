# PetSystem
extends Node

const PetScene := preload("res://apps/pet/scenes/pet_scene.tscn")

var pet_nodes: Array[Node] = []


func _ready() -> void:
	LocationSystem.location_exited.connect(despawn_pets)
	LocationSystem.location_entered.connect(spawn_pets)


func spawn_pets(location: LocationResource) -> void:
	var main_scene := get_tree().current_scene
	for pet in get_pets_at_location(location):
		var node := PetScene.instantiate()
		node.resource = pet
		main_scene.add_child(node)
		pet_nodes.append(node)


func despawn_pets(_location: LocationResource) -> void:
	for node in pet_nodes:
		node.queue_free()
	pet_nodes.clear()


func add_pet_to_current_location(pet: PetResource) -> void:
	get_pets_at_current_location().append(pet)


func remove_pet_from_current_location(pet: PetResource) -> void:
	get_pets_at_current_location().erase(pet)


func get_pets_at_current_location() -> Array[PetResource]:
	return get_pets_at_location(LocationSystem.current_location)


func get_pets_at_location(location: LocationResource) -> Array[PetResource]:
	if SaveSystem and SaveSystem.current_save and location:
		var state := SaveSystem.current_save.get_or_create_location_state(location.name)
		if state: return state.pets
	return []


func lua_fields() -> Array:
	return [
		"add_pet_to_current_location", 
		"remove_pet_from_current_location",
		"get_pets_at_current_location"
	]
