class_name Spawner extends ModdableResource

const GroupName := &"spawned"
const MetaName := &"spawner"

@export var resource: Resource
@export var position: Vector2

var spawned_nodes: Array[Node] = []
var has_spawned: bool = false


func _init(resource_value: Resource = null, position_value: Vector2 = Vector2.ZERO) -> void:
	resource = resource_value
	position = position_value


## Spawns nodes in provided world. Also adds metadata to spawned nodes.
## [br][b]Note:[/b] should not be overridden. Override [method _spawn] instead.
func spawn(world: Node2D) -> void:
	spawned_nodes = _spawn(world)
	for node in spawned_nodes:
		node.add_to_group(GroupName)
		node.set_meta(MetaName, self)
	has_spawned = true


## Despawns nodes from world.
## [br][b]Note:[/b] should not be overridden. Override [method _despawn] instead.
func despawn(world: Node2D) -> void:
	_despawn(world)
	has_spawned = false


## Virtual function. Override to add custom spawn logic.
func _spawn(_world: Node2D) -> Array[Node]: return []


## Virtual function. Override to add custom despawn logic.
func _despawn(_world: Node2D) -> void:
	for node in spawned_nodes:
		if Nodes.can_queue_free(node): node.queue_free()
	spawned_nodes.clear()
