extends MeshInstance3D


@export var room: RoomResource:
	set(value):
		Sig.switch_connection(room, value, &"changed", update)
		room = value
		update()


func update() -> void:
	if room: mesh = room.get_mesh()
	else: mesh = null
