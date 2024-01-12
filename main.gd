extends Node2D

@onready var island := LocationDB.find("Island")


func _ready() -> void:
	SaveSystem.save_opened.connect(_on_save_opened)
	if SaveSystem.current_save:
		_on_save_opened(SaveSystem.current_save)
	
	await get_tree().create_timer(0.5).timeout
	var test_mod: ModResource = ResourceLoader.load("res://mods/test_mod/test_mod.mod.tres")
	test_mod.run_entry_script()


func _on_save_opened(_save: SaveResource) -> void:
	await get_tree().process_frame
	LocationSystem.enter(island)
	
