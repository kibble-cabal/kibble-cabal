class_name Toggle extends RefCounted

## This class is a wrapper around [bool] that emits signals when its value changes.

signal toggled
signal toggled_on
signal toggled_off


var value: bool:
	set(next_value):
		var prev_value := value
		value = next_value
		if next_value and not prev_value:
			toggled_on.emit()
		if not next_value and prev_value:
			toggled_off.emit()


func _init(initial_value: bool) -> void:
	value = initial_value
	toggled_on.connect(func(): toggled.emit())
	toggled_off.connect(func(): toggled.emit())


func to(new_value: bool) -> void:
	value = new_value


func is_true() -> bool:
	return value


func is_false() -> bool:
	return not value
