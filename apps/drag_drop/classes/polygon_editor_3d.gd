class_name PolygonEditor3D extends PolygonEditor3DBase

const PointScene := preload("../scenes/polygon_point_3d.tscn")

@export var polygon: PackedVector2Array


func _get_point_scene() -> PackedScene:
	return PointScene


func _get_point_count() -> int:
	return polygon.size()


func _get_point_position(index: int) -> Vector2:
	if polygon.size() > index and index >= 0:
		return polygon[index]
	return Vector2.ZERO


func _set_point_position(index: int, pos: Vector2) -> void:
	if polygon.size() > index and index >= 0:
		polygon[index] = pos


func _add_point(index: int, pos: Vector2) -> void:
	if index >= 0:
		polygon.insert(index, pos)


func _remove_point(index: int) -> void:
	if polygon.size() > index and index >= 0:
		polygon.remove_at(index)
