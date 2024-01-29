extends MeshInstance3D


@export var room: RoomResource:
	set(value):
		room = value
		update()


func update() -> void:
	if room: mesh = room.get_mesh()
	else: mesh = null
