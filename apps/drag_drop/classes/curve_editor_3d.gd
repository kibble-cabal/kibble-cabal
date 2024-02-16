class_name CurveEditor3D extends PolygonEditor3DBase

const PointScene := preload("../scenes/polygon_point_3d.tscn")


@export var curve: Curve2D:
	set(value):
		curve = value
		_on_curve_changed()


func _ready() -> void:
	super()
	_on_curve_changed()


func _get_point_scene() -> PackedScene:
	return PointScene


func _get_point_count() -> int:
	if curve: return curve.point_count
	return 0


func _get_point_position(index: int) -> Vector2:
	if curve and index >= 0 and index < curve.point_count:
		return curve.get_point_position(index)
	return Vector2.ZERO


func _set_point_position(index: int, pos: Vector2) -> void:
	if curve and index >= 0 and curve.point_count > index:
		curve.set_point_position(index, pos)


func _add_point(index: int, pos: Vector2) -> void:
	if curve and index >= 0:
		curve.add_point(pos, Vector2.ZERO, Vector2.ZERO, index)


func _remove_point(index: int) -> void:
	if curve and index >= 0 and curve.point_count > index:
		curve.remove_point(index)


func update_point(point: Node) -> void:
	super(point)
	point.curve = curve


func _on_curve_changed() -> void:
	if curve:
		Sig.try_connect(curve.changed, update_point_list)
	if is_node_ready():
		update_point_list()
