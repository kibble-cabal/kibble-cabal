# PetSystem
extends Node


func add_pet(pet: PetResource) -> void:
	get_current_pets().append(pet)


func remove_pet(pet: PetResource) -> void:
	get_current_pets().erase(pet)


func get_current_pets() -> Array[PetResource]:
	if SaveSystem and SaveSystem.current_save:
		return SaveSystem.current_save.pets
	return []


func lua_fields() -> Array[String]:
	return ["add_pet", "remove_pet", "get_current_pets"]
