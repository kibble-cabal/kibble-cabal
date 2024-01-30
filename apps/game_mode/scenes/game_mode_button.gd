extends KButton

const ButtonFocusMaterial = preload("res://apps/game_mode/resources/game_mode_button_focus_material.tres")

@export var game_mode: GameModeResource:
	set(value):
		if game_mode: Sig.try_disconnect(game_mode.changed, update)
		if value: Sig.try_connect(value.changed, update)
		game_mode = value
		update()

@onready var icon_texture := $TextureRect as TextureRect
@onready var shadow_rect := $ShadowRect as ColorRect

var focus_material := ButtonFocusMaterial.duplicate()


func _ready() -> void:
	super()
	update()


func update() -> void:
	if not game_mode: return
	text = game_mode.name.to_lower()
	add_theme_color_override(&"font_color", game_mode.ui_color)
	add_theme_color_override(&"font_disabled_color", Color(game_mode.ui_color, 0.5))
	add_theme_color_override(&"font_hover_color", game_mode.ui_color)
	add_theme_color_override(&"font_pressed_color", game_mode.ui_color)
	add_theme_color_override(&"font_hover_pressed_color", game_mode.ui_color)
	add_theme_color_override(&"font_focus_color", game_mode.ui_color)
	
	if icon_texture:
		icon_texture.texture = game_mode.ui_icon
		icon_texture.modulate = game_mode.ui_color

	if shadow_rect:
		shadow_rect.custom_minimum_size = size
		shadow_rect.reset_size()
		shadow_rect.position = position + Vector2(-30, 10)


func _tween_focus() -> void:
	var tween: Tween = create_tween().set_parallel()
	tween.tween_property(material, "shader_parameter/use_color_1", Color("#0d0d0d"), 0.075)
	tween.tween_property(material, "shader_parameter/use_color_2", game_mode.ui_color, 0.075)
	tween.tween_property(self, "scale", Vector2(1.1, 1.1), 0.15)
	await tween.finished


func _tween_unfocus() -> void:
	var tween: Tween = create_tween().set_parallel().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_SINE)
	tween.tween_property(material, "shader_parameter/use_color_2", Color("#0d0d0d"), 0.075)
	tween.tween_property(material, "shader_parameter/use_color_1", game_mode.ui_color, 0.075)
	tween.tween_property(self, "scale", Vector2(1, 1), 0.15)
	await tween.finished


func _on_focus_entered() -> void:
	if not game_mode: return
	material = focus_material
	focus_material.set_shader_parameter("replace_color_1", game_mode.ui_color)
	focus_material.set_shader_parameter("replace_color_2", Color("#0d0d0d"))
	focus_material.set_shader_parameter("use_color_1", game_mode.ui_color)
	focus_material.set_shader_parameter("use_color_2", Color("#0d0d0d"))
	await _tween_focus()
	


func _on_focus_exited() -> void:
	await _tween_unfocus()
	material = null


func _on_pressed() -> void:
	if game_mode:
		GameModeSystem.to(game_mode)
