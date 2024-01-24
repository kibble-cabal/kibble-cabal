extends PlayerBody2D

@export var resource: PetResource

@onready var start_position := global_position
@onready var sprite_controller := $SpriteController as SpriteController
@onready var ability_system := $AbilitySystem as AbilitySystem


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
		
		sprite_controller.modulate = resource.modulate
		global_position = resource.current_position
	
	# Update the cached ability system state whenever the game is saved
	SaveSystem.before_saved.connect(
		func() -> void:
			if resource and ability_system:
				resource.ability_state = AbilitySystemState.new_from_node(ability_system)
	)
	
	super._ready()


func get_random_target() -> Vector2:
	return Vector2(randf_range(300, 800), randf_range(300, 800))


func _instantiate_sprite_controller() -> void:
	move_started.connect(sprite_controller.start.bind("walk"))
	move_finished.connect(sprite_controller.start.bind("default"))


func _on_move_finished() -> void:
	if resource: resource.current_position = global_position
