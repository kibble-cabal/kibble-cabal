extends Node3D

@export var curve: Curve2D

@onready var line := $Line2D as Line2D


func _ready() -> void:
	$PolygonEditor3D.curve = curve
	curve.changed.connect(_on_curve_changed)


func _on_curve_changed() -> void:
	if curve and line:
		line.clear_points()
		var points := curve.tessellate(3)
		for point in points:
			line.add_point(point * 100)
