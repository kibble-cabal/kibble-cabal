extends Control3D

@export var building: Building
@export var index: int

var wall: WallRef


func _ready() -> void:
	if building and building.has_wall(index):
		wall = building.get_wall(index)
		building.changed.connect(update)
		update()


func update() -> void:
	if wall: local_position = Vec3.from(wall.get_midpoint(), wall.height / 2)


func _on_move_button_pressed() -> void:
	building.MoveWallRequested.emit(index)


func _on_destroy_button_pressed() -> void:
	building.DestroyWallRequested.emit(index)
