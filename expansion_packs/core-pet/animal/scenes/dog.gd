extends SpriteController

## This is just a test class to try out the [SpriteController] framework.
## It definitely will not make it into the final game.

@onready var player = get_parent()
@onready var sprite := $AnimatedSprite3D as AnimatedSprite3D


func play(animation: String) -> void:
	@warning_ignore("redundant_await")
	await super(animation)
	sprite.play(StringName(animation), 1.0)


func transition(prev_animation: String, interrupt_time: float, next_animation: String) -> void:
	@warning_ignore("redundant_await")
	await super(prev_animation, interrupt_time, next_animation)
	await get_tree().create_tween().tween_property(sprite, "modulate", Color.YELLOW, 0.125).finished
	await get_tree().create_tween().tween_property(sprite, "modulate", Color.WHITE, 0.125).finished


func stop() -> void:
	sprite.stop()
