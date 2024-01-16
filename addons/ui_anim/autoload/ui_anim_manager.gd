# UIAnimManager
extends Node


signal mouse_exited_window

## Emitted when the mouse is released anywhere on screen.
## Helpful to reset state when user: clicks on a button → drags mouse elsewhere → releases
signal mouse_released


func _notification(what: int) -> void:
	match what:
		NOTIFICATION_WM_MOUSE_EXIT:
			mouse_exited_window.emit()


func _input(event: InputEvent) -> void:
	if event is InputEventMouseButton and event.is_released():
		mouse_released.emit()
