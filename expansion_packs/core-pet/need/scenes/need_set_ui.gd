extends VBoxContainer

const NeedScene := preload("need_ui.tscn")


@export var ability_system: AbilitySystem:
	set(value):
		ability_system = value
		_connect_ability_system()

var nodes := {}


func _ready() -> void:
	_connect_ability_system()
	update()


func update() -> void:
	if not ability_system or not is_inside_tree(): return
	
	var attributes := get_all_need_attributes()
	for attribute in attributes:
		# Skip attributes that haven't been added to the provided ability system
		if not ability_system.has_attribute(attribute): continue
		
		# Add attributes that haven't already been added
		if attribute not in nodes:
			var scene := NeedScene.instantiate()
			scene.attribute = attribute
			scene.ability_system = ability_system
			nodes[attribute] = scene
			add_child(scene)
	
	# Remove old attributes
	for attribute in nodes.keys():
		if attribute not in attributes or not ability_system.has_attribute(attribute):
			(nodes[attribute] as Node).queue_free()


func get_all_need_attributes() -> Array[Attribute]:
	var attributes: Array[Attribute] = []
	for need_identifier in NeedsConfig.Needs:
		attributes.append(AttributeDB.find(need_identifier))
	return attributes


func _connect_ability_system() -> void:
	if ability_system: Sig.try_connect(ability_system.attributes_changed, update)
	if is_inside_tree(): update()
