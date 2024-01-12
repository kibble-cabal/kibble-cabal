# LocationDB
extends Node

signal location_registered(location: LocationResource)
signal location_unregistered(location: LocationResource)


var registered_locations: Array[LocationResource] = []


func register(location: LocationResource) -> void:
	registered_locations.append(location)
	location_registered.emit(location)


func unregister(location: LocationResource) -> void:
	registered_locations.erase(location)
	location_unregistered.emit(location)


func find(location_name: String) -> LocationResource:
	for location in registered_locations:
		if location.name == location_name: return location
	return null


func lua_fields() -> Array:
	return ["register", "unregister", "find", "registered_locations"]
