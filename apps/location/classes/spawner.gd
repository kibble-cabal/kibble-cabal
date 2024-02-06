class_name Spawner extends ModdableResource

const GroupName := &"spawned"
const MetaName := &"spawner"
const TopLevelGroupName := &"spawned_top_level"

@export var resource: Resource:
	set(value):
		Sig.switch_connection(resource, value, "changed", update)
		resource = value

var spawned_nodes: Array[Node] = []
var has_spawned: bool = false


func _init(resource_value: Resource = null) -> void:
	resource = resource_value


## Spawns nodes in provided world. Also adds metadata to spawned nodes.
## [br][b]Note:[/b] should not be overridden. Override [method _spawn] instead.
func spawn(world: Node3D) -> void:
	spawned_nodes = _spawn(world)
	for node in spawned_nodes:
		node.add_to_group(GroupName)
		if not _is_subspawner():
			node.add_to_group(TopLevelGroupName)
		node.set_meta(MetaName, self)
	has_spawned = true
	update()


## Called right after spawn, or when [member resource] changes.
## [br][b]Note:[/b] should not be overridden. Override [method _update] instead.
func update() -> void:
	_update(spawned_nodes)


## Despawns nodes from world.
## [br][b]Note:[/b] should not be overridden. Override [method _despawn] instead.
func despawn() -> void:
	_despawn(spawned_nodes)
	has_spawned = false


## Virtual function. Override to add custom spawn logic.
func _spawn(_world: Node3D) -> Array[Node]: return []


## Virtual function. Override to add custom update logic for when [member resource] changes.
func _update(_nodes: Array[Node]) -> void: pass


## Virtual function. Override to add custom despawn logic.
func _despawn(nodes: Array[Node]) -> void:
	for node in nodes.filter(Nodes.can_queue_free): node.queue_free()
	nodes.clear()


func _is_subspawner() -> bool:
	return false
