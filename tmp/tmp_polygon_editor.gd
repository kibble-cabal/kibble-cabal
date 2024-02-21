extends Node3D

var curve: Curve2D

@onready var line := $Line2D as Line2D
@onready var editor := $PolygonEditor3D as CurveEditor3D


func _ready() -> void:
	curve = editor.Curve
	curve.changed.connect(_on_curve_changed)
	_on_curve_changed()


func _on_curve_changed() -> void:
	if curve and line:
		line.clear_points()
		var points := curve.tessellate(3)
		for point in points:
			line.add_point(point * 100)
