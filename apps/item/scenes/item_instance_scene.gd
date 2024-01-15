extends StaticBody2D

@onready var ability_system := $AbilitySystemComponent as AbilitySystemComponent

@export var item: ItemInstanceResource


func _ready() -> void:
	if item:
		ability_system.state = item.ability_state
		ability_system._update_state()
		
		var resource := item.get_item_resource()
		if resource and resource.physics_resource:
			var scene := resource.physics_resource.scene
			if scene: add_child(scene.instantiate())
