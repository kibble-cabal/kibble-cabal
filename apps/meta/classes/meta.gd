# Meta
extends Node

signal databases_ready
signal systems_ready


var are_databases_ready := false
var are_systems_ready := false


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
		PlayerSystem,
		SaveSystem,
		SoundManager
	]


func lua_fields() -> Array:
	return [
		"are_databases_ready",
		"are_systems_ready",
		"get_databases",
		"get_systems"
	]
