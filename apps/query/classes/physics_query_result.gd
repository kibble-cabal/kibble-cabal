class_name PhysicsQueryResult extends Object

var collider: Object
var collision_point: Vector2
var distance: float


func _init(args := {}) -> void:
	for key in args: if key in self: self[key] = args[key]


func _to_string() -> String:
	return "PhysicsQueryResult<{0}>".format([
		JSON.stringify({
			collider = collider,
			collision_point = collision_point,
			distance = distance,
		}, "  ")
	])
