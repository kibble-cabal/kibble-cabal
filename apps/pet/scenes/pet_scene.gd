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
	interact_menu.open(PetActionMenuItem.Ctx.new(self, resource))


func get_random_target() -> Vector2:
	return Vector2(randf_range(300, 800), randf_range(300, 800))


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
