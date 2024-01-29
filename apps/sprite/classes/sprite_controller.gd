class_name SpriteController extends Node3D

var current_animation: String
var current_time: float


func _process(delta: float) -> void:
	current_time += delta


func start(animation: String) -> void:
	if current_animation:
		@warning_ignore("redundant_await")
		await transition(current_animation, current_time, animation)
		current_animation = ""
		current_time = 0
	play(animation)


## Override this function to play an animation.
## [br]Make sure to call [code]super()[/code]!
func play(animation: String) -> void:
	current_animation = animation


## Override this function to play a transition between two animations.
## [br]Make sure to call [code]super()[/code]!
func transition(prev_animation: String, _interrupt_time: float, next_animation: String) -> void:
	current_animation = "{0} to {1}".format([prev_animation, next_animation])


## Override this function to stop current animation.
## [br]Make sure to call [code]super()[/code]!
func stop() -> void:
	current_animation = ""
	current_time = 0
