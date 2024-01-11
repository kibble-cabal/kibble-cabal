# PetSystem
extends Node

const PetScene := preload("res://apps/pet/scenes/pet_scene.tscn")


func add_pet_to_current_location(pet: PetResource) -> void:
	get_pets_at_current_location().append(pet)


func remove_pet_from_current_location(pet: PetResource) -> void:
	get_pets_at_current_location().erase(pet)


func get_pets_at_current_location() -> Array[PetResource]:
	if SaveSystem and SaveSystem.current_save:
		var current_location := LocationSystem.current_location
		var state: LocationStateResource = SaveSystem.current_save.get_or_create_location_state(current_location.name)
		if state: return state.pets
	return []


func lua_fields() -> Array[String]:
	return [
		"add_pet_to_current_location", 
		"remove_pet_from_current_location",
		"get_pets_at_current_location"
	]
