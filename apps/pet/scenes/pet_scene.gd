extends PlayerBody2D

@export var resource: PetResource

@onready var start_position := global_position
@onready var ability_system := $AbilitySystem as AbilitySystem
@onready var interact_menu := %InteractMenu as ActionMenu

var sprite_controller: SpriteController


func _ready() -> void:
	move_finished.connect(_on_move_finished)
	if resource:
		_instantiate_sprite_controller()
		
		# Add all attributes, if not preset
		# FIXME this needs to be removed later
		for need in NeedsConfig.Needs:
			ability_system.grant_attribute(AttributeDB.find(need))
		
		for identifier in NeedsConfig.FulfillNeedAbilities:
			ability_system.grant_ability(AbilityDB.find(identifier))
			ability_system.grant_ability(AbilityDB.find(identifier + "/cooldown"))
		
		# Update ability system from cached state
		if resource.ability_state:
			resource.ability_state.merge_into_node(ability_system)
		
		global_position = resource.current_position
		_update_collision()
	
	# Update the cached ability system state whenever the game is saved
	SaveSystem.before_saved.connect(
		func() -> void:
			if resource and ability_system:
				resource.ability_state = AbilitySystemState.new_from_node(ability_system)
	)
	
	super._ready()
	
	await get_tree().create_timer(2.0).timeout
	spawn_thought_bubble("Some text", 3.0, 200)


func _unhandled_input(event: InputEvent) -> void:
	# Close interact menu when clicking outside
	if event is InputEventScreenTouch and event.is_pressed() and interact_menu.visible:
		var menu_radius := maxf(interact_menu.size.x, interact_menu.size.y) / 2
		if interact_menu.get_local_mouse_position().distance_to(interact_menu.size / 2) > menu_radius:
			interact_menu.close()


func get_random_target() -> Vector2:
	return Vector2(randf_range(0, 250), randf_range(0, 250))


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
		sprite_controller.modulate = resource.modulate
		add_child(sprite_controller)
		move_child(sprite_controller, 0)
		move_started.connect(sprite_controller.start.bind("walk"))
		move_finished.connect(sprite_controller.start.bind("default"))


func _update_collision() -> void:
	if not resource: return
	var animal := resource.get_animal_resource()
	if animal:
		(($CollisionShape2D as CollisionShape2D).shape as CircleShape2D).radius = animal.collision_radius
		(($Interactable2D/CollisionShape2D as CollisionShape2D).shape as CircleShape2D).radius = animal.collision_radius
		($FacingRay as RayCast2D).target_position = Vector2(0, animal.collision_radius * 1.5)


func _on_move_finished() -> void:
	if resource: resource.current_position = global_position


func _on_interact_menu_opening() -> void:
	pass


func _on_interactable_input_event(_viewport: Node, event: InputEvent, _shape_idx: int) -> void:
	if event is InputEventScreenTouch and event.is_pressed():
		interact_menu.open(PetActionMenuItem.Ctx.new(self, resource))
