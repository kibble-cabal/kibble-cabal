extends Node2D

@onready var ability_system := $AbilitySystem as AbilitySystem

@export var item: ItemInstanceResource


func _ready() -> void:
	if item:
		var resource := item.get_item_resource()
		
		if resource and resource.ability_state:
			resource.ability_state.merge_into_node(ability_system)
		if item.ability_state:
			item.ability_state.merge_into_node(ability_system)
		
		if resource and resource.physics_resource:
			var scene := resource.physics_resource.scene
			if scene: add_child(scene.instantiate())
	
	SaveSystem.before_saved.connect(
		func() -> void:
			item.ability_state = AbilitySystemState.new_from_node(ability_system)
	)
