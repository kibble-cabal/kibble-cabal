# Meta
extends Node

signal databases_ready
signal systems_ready

signal singleton_added(node: Node)

var are_databases_ready := false
var are_systems_ready := false

## [Dictionary][[Script], [Node]]
var singletons := {}


func _process(_delta: float) -> void:
	if not are_databases_ready:
		are_databases_ready = get_databases().all(
			func(db: Node) -> bool: return db and db.is_inside_tree()
		)
		if are_databases_ready: databases_ready.emit()
	
	if not are_systems_ready:
		are_systems_ready = get_systems().all(
			func(system: Node) -> bool: return system and system.is_inside_tree()
		)
		if are_systems_ready: systems_ready.emit()
	
	# Stop processing after all events have fired
	if are_databases_ready and are_systems_ready:
		set_process(false)


func get_databases() -> Array[Node]:
	return [
		AbilityDB,
		AttributeDB,
		TagDB,
		EffectDB,
		ActionDB,
		SubtreeDB,
		AnimalDB,
		ExpansionPackDB,
		GameModeDB,
		ItemDB,
		LocationDB,
		ModDB,
		MusicDB,
		QuestDB,
		SettingDefinitionDB,
		SoundEffectDB,
	]


func get_systems() -> Array[Node]:
	return [
		DatetimeSystem,
		ExpansionPackSystem,
		GameModeSystem,
		LocationSystem,
		ModSystem,
		LuaSystem,
		MusicSystem,
		PetSystem,
		SaveSystem,
		SoundManager
	]


func add_or_get_singleton(script: Script) -> Node:
	if script in singletons:
		return singletons[script]
	var node: Node = script.new()
	singletons[script] = node
	add_child(node)
	singleton_added.emit(node)
	return node


func get_singleton(script: Script) -> Node:
	return singletons.get(script)


func lua_fields() -> Array:
	return [
		"are_databases_ready",
		"are_systems_ready",
		"get_databases",
		"get_systems",
		"get_singleton"
	]
