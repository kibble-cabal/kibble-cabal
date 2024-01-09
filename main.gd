extends Node2D

@onready var island := LocationDB.find("Island")


func _ready() -> void:
	SaveSystem.save_opened.connect(_on_save_opened)


func _on_save_opened(_save: SaveResource) -> void:
	await get_tree().process_frame
	LocationSystem.enter(island)
	
