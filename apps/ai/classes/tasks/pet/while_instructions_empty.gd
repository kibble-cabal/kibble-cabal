@tool
extends BTDecorator


## Executes the child task only if the owning pet has no instructions. If the pet does have instructions, the current task is aborted.

## The status to return if an instruction is found.
@export var on_instruction_found: Status = FAILURE


func _generate_name() -> String:
	return "While this pet has no instructions..."


func _tick(delta: float) -> Status:
	if get_child_count() == 0:
		return SUCCESS
	
	var pet := get_pet_resource()
	if not pet: return FAILURE
	
	if pet.instructions.is_empty():
		return get_child(0).execute(delta)
	else:
		get_child(0).abort()
		return on_instruction_found


func get_pet_resource() -> PetResource:
	if agent and "resource" in agent:
		return agent.resource as PetResource
	return null
