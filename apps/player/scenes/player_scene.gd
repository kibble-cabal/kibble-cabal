class_name PlayerRoot extends PlayerBody2D

@export var footstep_sound_effect: AudioStream

@onready var collision_shape := $CollisionShape as CollisionShape2D
@onready var ability_system := $AbilitySystemComponent as AbilitySystemComponent

var sprite_controller: SpriteController

var resource: PlayerResource:
	get: return SaveSystem.current_save.player if SaveSystem and SaveSystem.current_save else null


func _ready() -> void:
	if resource:
		_instantiate_sprite_controller()
		sprite_controller.modulate = resource.modulate
		ability_system.state = resource.ability_state
	super._ready()
	move_started.connect(_on_move_started)
	move_finished.connect(_on_move_finished)


func _unhandled_input(event: InputEvent) -> void:
	if (
		SaveSystem.get_setting("tap_to_move", true)
		and event.is_action_pressed("click") 
		and navigation_agent
	): 
		var mouse_position := get_global_mouse_position()
		navigation_agent.set_target_position(mouse_position)
		move_started.emit()


func _physics_process(delta: float) -> void:
	var direction := Input.get_vector("left", "right", "up", "down").round()
	if direction.x != 0 or direction.y != 0:
		velocity = direction * navigation_agent.max_speed
	super._physics_process(delta)


func _instantiate_sprite_controller() -> void:
	if sprite_controller:
		sprite_controller.queue_free()
		sprite_controller = null
	if resource.sprite_scene:
		sprite_controller = resource.sprite_scene.instantiate()
		move_started.connect(sprite_controller.start.bind("walk"))
		move_finished.connect(sprite_controller.start.bind("default"))
		add_child(sprite_controller)


func _on_move_started() -> void:
	SoundManager.play_sound_with_pitch(footstep_sound_effect, SoundEffectPlayer2D.get_random_pitch(), -5.0)


func _on_move_finished() -> void:
	SoundManager.sound_effects.stop(footstep_sound_effect)
	if resource:
		resource.current_position = position
		SaveSystem.commit_changes()
