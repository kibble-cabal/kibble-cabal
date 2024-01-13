extends Node2D

@onready var island := LocationDB.find("Island")
@onready var live_mode := GameModeDB.find("Live")
@onready var live_paused_mode := GameModeDB.find("Live/Paused")


func _ready() -> void:
	SaveSystem.save_opened.connect(_on_save_opened)
	if SaveSystem.current_save:
		_on_save_opened(SaveSystem.current_save)


func _on_save_opened(_save: SaveResource) -> void:
	await get_tree().process_frame
	LocationSystem.enter(island)
	GameModeSystem.to(live_mode)
	
