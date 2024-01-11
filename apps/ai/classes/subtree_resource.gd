class_name SubtreeResource extends ModdableResource

@export var key: StringName
@export var subtree: BehaviorTree


func lua_fields() -> Array[String]:
	return ["key", "subtree"]
