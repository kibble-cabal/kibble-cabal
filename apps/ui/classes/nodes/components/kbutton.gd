class_name KButton extends Button

## This is a wrapper around the default [Button] class which provides some virtual methods and default animations.

var tweens := [
	await UIScaleOnHover.new(self),
	await UIScaleOnPress.new(self)
]


func _ready() -> void:
	Sig.try_connect(toggled, _on_toggled)
	Sig.try_connect(pressed, _on_pressed)
	Sig.try_connect(focus_entered, _on_focus_entered)
	Sig.try_connect(focus_exited, _on_focus_exited)
	
	# Private methods.
	Sig.try_connect(tree_exiting, __exiting_tree)


func __exiting_tree() -> void:
	if has_focus(): release_focus()


## Virtual method.
func _on_toggled(_value: bool) -> void: pass
## Virtual method.
func _on_pressed() -> void: pass
## Virtual method.
func _on_focus_entered() -> void: pass
## Virtual method.
func _on_focus_exited() -> void: pass
