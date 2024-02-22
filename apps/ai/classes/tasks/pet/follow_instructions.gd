@tool
extends BTAction


func _generate_name() -> String:
	return "Follow Instructions"


func _enter() -> void:
	var pet := get_pet_resource()
	if pet:
		blackboard.set_var(&"context/is_instruction", true)
		for tree in pet.Instructions:
			add_child(tree.instantiate(agent, blackboard))


func _exit() -> void:
	blackboard.set_var(&"context/is_instruction", false)


func _tick(delta: float) -> Status:
	var pet := get_pet_resource()
	if not pet: return FAILURE
	
	if pet.Instructions.is_empty():
		return SUCCESS
	
	var index: int = pet.Instructions.size() - 1
	if get_child(index).execute(delta) in [SUCCESS, FAILURE]:
		pet.Instructions.pop_back()
	
	return RUNNING


func get_pet_resource() -> RPet:
	if agent and "resource" in agent and agent.resource is RPet:
		return agent.resource as RPet
	return null
