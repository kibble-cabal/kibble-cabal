class_name UIHoverBase extends UIAnimBase


func _get_signals_to_connect() -> Array[Signal]:
	return [
		node.mouse_entered,
		node.mouse_exited,
		UIAnimManager.mouse_exited_window
	]


## Virtual method.
func _on_mouse_entered() -> void: pass


## Virtual method. Called when mouse exits [member node].
func _on_mouse_exited() -> void: pass


## Virtual method. Called when mouse exits the window.
func _on_mouse_exited_window() -> void: pass
