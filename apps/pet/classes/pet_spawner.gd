class_name PetSpawner extends Spawner

const Pet := preload("res://apps/pet/scenes/pet_scene.tscn")

var pet_node: PetScene


func _spawn(world: Node3D) -> Array[Node]:
	if resource and resource is PetResource:
		pet_node = Pet.instantiate()
		pet_node.resource = resource
		world.add_child(pet_node)
	return [pet_node]
