@tool
class_name ThoughtBubble extends Node2D

const BubbleShader := preload("res://content/shaders/canvas_item/thought_bubble_2.gdshader")
const WavyTextMaterial := preload("res://content/materials/wavy_text.tres")

@export_multiline var text: String:
	set(value):
		text = value
		queue_redraw_all()

@export var font: Font = null:
	set(value):
		font = value
		queue_redraw_all()

@export var font_size: int = -1:
	set(value):
		font_size = value
		queue_redraw_all()

@export var max_width: float = -1:
	set(value):
		max_width = value
		queue_redraw_all()

@export var color: Color = Color.WHITE:
	set(value):
		color = value
		queue_redraw_all()

@export var background_color: Color = Color.WHITE * 0.1:
	set(value):
		background_color = value
		queue_redraw_all()

@export var centered: bool = true

var size: Vector2:
	get: return get_background_size()

var mat := ShaderMaterial.new()
var background := ColorRect.new()
var foreground := Label.new()


func _enter_tree() -> void:
	mat.shader = BubbleShader
	mat.set_shader_parameter("seed", randi_range(0, 1000))
	mat.set_shader_parameter("num_bubbles", randf_range(7.5, 9.5))
	add_child(background)
	add_child(foreground)


func _notification(what: int) -> void:
	if what in [NOTIFICATION_READY, NOTIFICATION_ENTER_CANVAS, NOTIFICATION_DRAW, NOTIFICATION_VISIBILITY_CHANGED]:
		queue_redraw_all()


func update() -> void:
	# Update background
	background.color = Color.BLACK
	background.material = mat
	background.custom_minimum_size = get_background_size()
	if centered:
		background.position = -background.custom_minimum_size / 2
	else:
		background.position = Vector2.ZERO
	background.reset_size()
	mat.set_shader_parameter("color", background_color)
	
	foreground.text = text
	foreground.use_parent_material = true
	foreground.add_theme_font_override("font", get_font())
	foreground.add_theme_color_override("font_color", color)
	foreground.add_theme_font_size_override("font_size", get_font_size())
	foreground.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	foreground.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
	foreground.custom_minimum_size = get_foreground_size()
	foreground.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	foreground.reset_size()
	foreground.position = background.position + ((background.size - foreground.size) / 2)
	
	material = WavyTextMaterial


func get_font() -> Font:
	return font if font else ThemeDB.get_project_theme().default_font


func get_font_size() -> int:
	return font_size if font_size > 0 else ThemeDB.get_project_theme().default_font_size


func get_width() -> float:
	if max_width > 0: return max_width
	return 600


func get_foreground_size() -> Vector2:
	return get_font().get_multiline_string_size(text, HORIZONTAL_ALIGNMENT_CENTER, get_width() * 0.707, get_font_size())


func get_background_size() -> Vector2:
	var width := get_width()
	var min_height := maxf(width, get_foreground_size().y)
	return Vec2.xy(max(width, min_height)) * 1.3


func reset_size() -> void:
	background.custom_minimum_size = get_background_size()
	foreground.custom_minimum_size = get_foreground_size()
	background.reset_size()
	foreground.reset_size()


func queue_redraw_all() -> void:
	update()
	background.queue_redraw()
	foreground.queue_redraw()

func destroy() -> void:
	if not self.is_inside_tree() or self.is_queued_for_deletion(): return
	await (create_tween()
		.set_ease(Tween.EASE_IN)
		.set_trans(Tween.TRANS_BACK)
		.tween_property(self, "scale", Vector2.ZERO, 0.25)).finished
	queue_free()

func initialize(text: String, duration: float = 3, max_width: float = -1) -> void:
	add_to_group(&"ui")
	add_to_group(&"thought_bubble")
	self.text = text
	scale *= 0
	self.max_width = max_width
	reset_size()
	position = Vector2(0, -size.y / 2)
	if duration > 0: ready.connect(
		func() -> void:
			await (create_tween()
				.set_ease(Tween.EASE_OUT)
				.set_trans(Tween.TRANS_BACK)
				.tween_property(self, "scale", Vector2.ONE, 0.25)).finished
			
			# Destroy thought bubble after duration passed
			if Nodes.can_queue_free(self):
				await get_tree().create_timer(duration).timeout
				await destroy()
	)


static func spawn(text: String, duration: float = 3, max_width: float = -1) -> ThoughtBubble:	
	var bubble := ThoughtBubble.new()
	bubble.initialize(text, duration, max_width)
	return bubble
