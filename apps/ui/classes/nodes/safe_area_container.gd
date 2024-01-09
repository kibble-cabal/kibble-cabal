@tool
class_name SafeAreaContainer extends MarginContainer

@export_group("Extra margins")
@export var margin_top = 0:
	set(value):
		margin_top = value
		update()

@export var margin_left = 0:
	set(value):
		margin_left = value
		update()

@export var margin_right = 0:
	set(value):
		margin_right = value
		update()

@export var margin_bottom = 0:
	set(value):
		margin_bottom = value
		update()


func update() -> void:
	var safe_area := DisplayServer.get_display_safe_area()
	var global_rect = get_global_rect()
	var margin_top_left = Vector2(safe_area.position) - global_rect.position
	var margin_bottom_right = global_rect.end - Vector2(safe_area.end)
	
	add_theme_constant_override("margin_left", maxf(0, margin_top_left.x) + margin_left)
	add_theme_constant_override("margin_top", maxf(0, margin_top_left.y) + margin_top)
	add_theme_constant_override("margin_right", maxf(0, margin_bottom_right.x) + margin_right)
	add_theme_constant_override("margin_bottom", maxf(0, margin_bottom_right.y) + margin_bottom)


func _draw() -> void:
	update()
