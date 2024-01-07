# AnimalDB
extends Node

signal animal_registered(animal: AnimalResource)
signal animal_unregistered(animal: AnimalResource)


var registered_animals: Array[AnimalResource] = []


func register(animal: AnimalResource) -> void:
	registered_animals.append(animal)
	animal_registered.emit(animal)


func unregister(animal: AnimalResource) -> void:
	registered_animals.erase(animal)
	animal_unregistered.emit(animal)


func find(animal_name: String) -> AnimalResource:
	for animal in registered_animals:
		if animal.name == animal_name: return animal
	return null
