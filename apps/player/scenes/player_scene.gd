extends PlayerBody2D

@onready var collision_shape := $CollisionShape as CollisionShape2D
@onready var footstep_player := $FootstepPlayer as SoundEffectPlayer2D

var sprite_controller: SpriteController

var resource: PlayerResource:
	get: return SaveSystem.current_save.player if SaveSystem and SaveSystem.current_save else null


func _ready() -> void:
	if resource:
		_instantiate_sprite_controller()
		sprite_controller.modulate = resource.modulate
	super._ready()
	move_finished.connect(_on_move_finished)


func reset() -> void:
	speed = 144
	size = Vector2(32, 64)
	motion_mode = CharacterBody2D.MOTION_MODE_FLOATING


func _unhandled_input(event: InputEvent) -> void:
	if (
		SaveSystem.get_setting("tap_to_move", true)
		and event.is_action_pressed("click") 
		and navigation_agent
	): 
		var mouse_position := get_global_mouse_position()
		navigation_agent.set_target_position(mouse_position)
		target_received.emit(mouse_position)
		move_started.emit()


func _physics_process(_delta: float) -> void:
	if navigation_agent and not navigation_agent.is_navigation_finished(): return
	current_direction = Input.get_vector("left", "right", "up", "down").round()
	velocity = current_direction * speed
	move_and_slide()


func _instantiate_sprite_controller() -> void:
	if sprite_controller:
		sprite_controller.queue_free()
		sprite_controller = null
	if resource.sprite_scene:
		sprite_controller = resource.sprite_scene.instantiate()
		move_started.connect(sprite_controller.start.bind("walk"))
		move_finished.connect(sprite_controller.start.bind("default"))
		add_child(sprite_controller)


func _on_move_finished() -> void:
	if resource:
		resource.current_position = position
		SaveSystem.commit_changes()
