class_name PetScene extends PetBody3D

@export var resource: PetResource

@onready var start_position := global_position
@onready var ability_system := $AbilitySystem as AbilitySystem
@onready var interact_menu := %InteractMenu as ActionMenu

@onready var viewport := get_viewport()
@onready var camera := viewport.get_camera_3d()
@onready var behavior_tree := $BTPlayer as BTPlayer

var sprite_controller: SpriteController


func _ready() -> void:
	move_finished.connect(_on_move_finished)
	if resource:
		_instantiate_sprite_controller()
		
		# Update ability system from cached state
		if resource.ability_state:
			resource.ability_state.merge_into_node(ability_system)
		
		# Add all attributes, if not preset
		# FIXME this needs to be removed later
		for need in NeedsConfig.Needs:
			ability_system.grant_attribute(AttributeDB.find(need))
		
		for identifier in NeedsConfig.FulfillNeedAbilities:
			var ability := AbilityDB.find(identifier)
			var cooldown_ability := AbilityDB.find(identifier + "/cooldown")
			if ability: ability_system.grant_ability(ability)
			if cooldown_ability: ability_system.grant_ability(cooldown_ability)
		
		# Add personality, if not preset
		# FIXME this needs to be removed later
		PersonalityConfig.randomize_personality(ability_system)
		
		global_position = resource.current_position
		_update_collision()
		_update_speed()
	
	# Update the cached ability system state whenever the game is saved
	SaveSystem.before_saved.connect(
		func() -> void:
			if resource and ability_system:
				resource.ability_state = AbilitySystemState.new_from_node(ability_system)
	)
	
	super._ready()


func _unhandled_input(event: InputEvent) -> void:
	# Close interact menu when clicking outside
	if event is InputEventScreenTouch and event.is_pressed() and interact_menu.visible:
		var menu_radius := maxf(interact_menu.size.x, interact_menu.size.y) / 2
		if interact_menu.get_local_mouse_position().distance_to(interact_menu.size / 2) > menu_radius:
			interact_menu.close()
			viewport.set_input_as_handled()


func get_random_target() -> Vector3:
	return Vector3(randf_range(-2, 2), 0, randf_range(-2, 2))


func destroy_thought_bubble(bubble: ThoughtBubble) -> void:
	if bubble == null or not bubble.is_inside_tree() or bubble.is_queued_for_deletion(): return
	await (bubble
		.create_tween()
		.set_ease(Tween.EASE_IN)
		.set_trans(Tween.TRANS_BACK)
		.tween_property(bubble, "scale", Vector2.ZERO, 0.25)).finished
	bubble.queue_free()


func destroy_thought_bubbles() -> void:
	Nodes.get_children_in_group(self, &"thought_bubble").map(destroy_thought_bubble)
	await get_tree().create_timer(0.25).timeout


func spawn_thought_bubble(text: String, duration: float = 3, max_width: float = -1) -> void:
	await destroy_thought_bubbles()
	
	# Create thought bubble
	var bubble := ThoughtBubble.new()
	bubble.add_to_group(&"ui")
	bubble.add_to_group(&"thought_bubble")
	bubble.text = text
	bubble.scale *= 0
	bubble.max_width = max_width
	bubble.reset_size()
	bubble.position = Vector2(0, -bubble.size.y / 2)
	
	# Move above pet
	if resource:
		var animal := resource.get_animal_resource()
		if animal: bubble.position.y -= animal.collision_radius
	
	add_child(bubble)
	await (bubble
		.create_tween()
		.set_ease(Tween.EASE_OUT)
		.set_trans(Tween.TRANS_BACK)
		.tween_property(bubble, "scale", Vector2.ONE, 0.25)).finished
	
	# Destroy thought bubble after duration passed
	await get_tree().create_timer(duration).timeout
	await destroy_thought_bubble(bubble)


func _instantiate_sprite_controller() -> void:
	if not resource: return
	var animal := resource.get_animal_resource()
	if animal and animal.sprite_scene:
		sprite_controller = animal.sprite_scene.instantiate()
		# FIXME
		#sprite_controller.modulate = resource.modulate
		add_child(sprite_controller)
		move_child(sprite_controller, 0)
		move_started.connect(sprite_controller.start.bind("walk"))
		move_finished.connect(sprite_controller.start.bind("default"))


func _update_collision() -> void:
	if not resource: return
	var animal := resource.get_animal_resource()
	if animal:
		($NavigationAgent as NavigationAgent).radius = animal.collision_radius
		(($CollisionShape as CollisionShape3D).shape as SphereShape3D).radius = animal.collision_radius
		(($Area/CollisionShape as CollisionShape3D).shape as SphereShape3D).radius = animal.collision_radius * 1.5
		($FacingRay as RayCast3D).target_position = Vector3(animal.collision_radius * 1.5, 0, 0)


func _update_speed() -> void:
	if not resource: return
	var animal := resource.get_animal_resource()
	if animal:
		($NavigationAgent as NavigationAgent).max_speed = animal.speed * 2


func _on_move_finished() -> void:
	if resource: resource.current_position = global_position


func _on_interact_menu_opening() -> void:
	pass


func _on_area_input_event(_camera: Node, event: InputEvent, _position: Vector3, _normal: Vector3, _shape_idx: int) -> void:
	if event is InputEventScreenTouch and event.is_pressed():
		interact_menu.open(PetActionMenuItem.Ctx.new(self, resource))
