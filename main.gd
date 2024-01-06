extends Node2D


func _ready() -> void:
	var island: LocationResource = ResourceLoader.load("res://expansions/core/locations/island/island_resource.tres")
	LocationSystem.enter(island)
