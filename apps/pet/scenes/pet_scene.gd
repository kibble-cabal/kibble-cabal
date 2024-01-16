extends PlayerBody2D

@export var resource: PetResource

@onready var start_position := global_position
@onready var sprite_controller := $SpriteController as SpriteController
@onready var ability_system := $AbilitySystemComponent as AbilitySystemComponent


func _ready() -> void:
	move_finished.connect(_on_move_finished)
	if resource:
		_instantiate_sprite_controller()
		
		# Add all attributes, if not preset
		# FIXME this needs to be removed later
		if resource.ability_state.attributes.is_empty():
			var table := AAttributeTable.new()
			for need in NeedsConfig.Needs:
				table.add(AttributeDB.find_attribute(need))
			resource.ability_state.attributes.append(table)
		
		# Add all abilities, if not preset
		# FIXME this needs to be removed later
		for ability_name in NeedsConfig.FulfillNeedAbilities:
			var ability := AbilityDB.find_ability(ability_name)
			if not ability in resource.ability_state.abilities:
				resource.ability_state.abilities.append(ability)
		
		resource.ability_state.ability_tasks.clear()
		resource.ability_state.effects.clear()
		
		sprite_controller.modulate = resource.modulate
		global_position = resource.current_position
		ability_system.state = resource.ability_state
		ability_system._update_state()
	super._ready()


func get_random_target() -> Vector2:
	return Vector2(randf_range(300, 800), randf_range(300, 800))


func _instantiate_sprite_controller() -> void:
	move_started.connect(sprite_controller.start.bind("walk"))
	move_finished.connect(sprite_controller.start.bind("default"))


func _on_move_finished() -> void:
	if resource: resource.current_position = global_position
