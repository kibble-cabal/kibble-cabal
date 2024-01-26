# PetSystem
extends Node


func get_pet_nodes() -> Array[PetScene]:
	var nodes: Array[PetScene] = []
	if LocationSystem.current_state: 
		for spawner in LocationSystem.current_state.spawners:
			if spawner is PetSpawner and spawner.has_spawned:
				nodes.append(spawner.pet_node)
	return nodes


func lua_fields() -> Array:
	return [
		"get_pet_nodes",
	]
