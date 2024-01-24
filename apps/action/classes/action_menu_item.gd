@tool
class_name ActionMenuItem extends Resource

## Base class for items in [ActionMenu]


func _init() -> void:
	resource_local_to_scene = true


## Virtual function. Override to change display text.
func _get_display_text(_ctx = null) -> String: return String()


## Virtual function. Override to change menu identifiers.
func _get_menu_identifiers(_ctx = null) -> Array[StringName]: return []


## Virtual function. Override to change what happens when pressed.
func _on_press(_ctx = null) -> void: pass


## Virtual function. Override to add logic to dynamically hide/show item.
func _is_visible(_ctx = null) -> bool: return true


## Virtual function. Override to change rendering.
func _update(button: Button, ctx = null) -> void:
	button.text = _get_display_text(ctx)


func get_display_text(ctx = null) -> String:
	return _get_display_text(ctx)


func get_menu_identifiers(ctx = null) -> Array[StringName]:
	return _get_menu_identifiers(ctx)


func update(button: Button, ctx = null) -> void:
	button.visible = _is_visible(ctx)
	Sig.disconnect_all(button.pressed)
	button.pressed.connect(_on_press.bind(ctx))
	_update(button, ctx)


func render(ctx = null) -> Button:
	var button := Button.new()
	button.pressed.connect(_on_press.bind(ctx))
	return button


func lua_fields() -> Array:
	return [
		"get_display_text",
		"get_menu_identifiers",
		"update",
		"render"
	]
