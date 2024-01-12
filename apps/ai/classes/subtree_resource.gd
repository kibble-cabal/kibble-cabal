class_name SubtreeResource extends ModdableResource

@export var key: StringName
@export var subtree: BehaviorTree

## Defines the order in which subtrees will be added to the behavior tree.
## Will be run in order of highest priority to lowest priority.
@export var priority: int = 1


func lua_fields() -> Array:
	return ["key", "subtree", "priority"]
