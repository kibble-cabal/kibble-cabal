class_name UIPressBase extends UIAnimBase


func _init(node: BaseButton) -> void:
	await super._init(node)


func _get_signals_to_connect() -> Array[Signal]:
	return [
		node.button_up,
		node.button_down,
		UIAnimManager.mouse_released
	]


## Virtual method. Called when mouse is pressed on [member node].
func _on_button_up() -> void: pass


## Virtual method. Called when mouse is released on [member node].
func _on_button_down() -> void: pass


## Virtual method. Called when mouse is released anywhere in the window.
func _on_mouse_released() -> void: pass
