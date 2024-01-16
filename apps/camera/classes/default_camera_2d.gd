extends "res://addons/gesture_controlled_camera_2d/GCC2D.gd"

const MoveSpeed := 10.0


func _unhandled_input(event: InputEvent) -> void:
	super._unhandled_input(event)
	
	var vector := Input.get_vector("left", "right", "up", "down").ceil() * -1
	if not vector.round().is_zero_approx():
		trigger_move(vector * MoveSpeed)
		for _i in range(12): # kick off multiple moves to interpolate camera position
			await get_tree().physics_frame
			# Stop if the user has stopped
			if Input.get_vector("left", "right", "up", "down").round().is_zero_approx(): break
			trigger_move(vector * MoveSpeed)


func trigger_move(relative: Vector2) -> void:
	var new_event := InputEventSingleScreenDrag.new()
	new_event.relative = relative
	_move(new_event)
