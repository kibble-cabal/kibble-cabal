extends VBoxContainer

const Scene := preload("characteristic_ui.tscn")

@export var ability_system: AbilitySystem

# Dictionary[Attribute, Node]
var nodes := {}


func _ready() -> void:
	update()
	if ability_system:
		ability_system.attributes_changed.connect(update)


func update() -> void:
	# Add or update all characteristics
	for id: String in PersonalityConfig.Characteristics:
		var attribute := AttributeDB.find(id)
		if attribute and ability_system.has_attribute(attribute):
			if attribute in nodes: nodes[attribute].update()
			else:
				var scene := Scene.instantiate()
				scene.characteristic = attribute
				scene.ability_system = ability_system
				add_child(scene)
				nodes[attribute] = scene
	
	# Remove outdated characteristics
	for attribute: Attribute in nodes.keys():
		if not attribute.identifier in PersonalityConfig.Characteristics:
			nodes[attribute].queue_free()
			nodes.erase(attribute)
