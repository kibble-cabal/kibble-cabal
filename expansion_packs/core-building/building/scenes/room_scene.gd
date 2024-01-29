extends CanvasGroup

@export var room: RoomResource:
	set(value):
		room = value
		update_materials()

@onready var tile_map := $TileMap as TileMap


func _ready() -> void:
	if room:
		tile_map.clear()
		tile_map.set_cells_terrain_connect(0, room.tiles, 0, 0)
		print(room.get_size())
		update_materials()


func update_materials() -> void:
	var mat := material as ShaderMaterial
	if room.get_floor_resource():
		mat.set_shader_parameter(&"floor_texture", room.get_floor_resource().physics_resource.static_image)
	if room.get_interior_resource():
		mat.set_shader_parameter(&"interior_texture", room.get_interior_resource().physics_resource.static_image)
	if room.get_exterior_resource():
		mat.set_shader_parameter(&"exterior_texture", room.get_exterior_resource().physics_resource.static_image)
	mat.set_shader_parameter(&"size", room.get_size())
	
