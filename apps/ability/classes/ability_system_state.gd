class_name AbilitySystemState extends Resource

## This class uses the AbilityDB, TagDB, and AttributeDB to populate the state of an [AbilitySystem] node.
##
## This class stores the state of an [AbilitySystem] node by storing just the IDENTIFIERS of the node's [Attribute]s, [Ability]s, and [Tag]s.
## It's done this way to ensure that there are no outdated instances of abilities, tags, etc. serialized anywhere.

## [Dictionary][[StringName], [float]] 
@export var attributes: Dictionary
@export var tags: Array[StringName]
@export var abilities: Array[StringName]


## Modifies this resource by replacing tags, attributes, and abilities from the provided node.
func populate_from_node(node: AbilitySystem) -> void:
	attributes = {}
	for attribute in node.attributes.keys():
		if attribute: attributes[attribute.identifier] = node.get_attribute_value(attribute)
	tags = []
	for tag in node.tags:
		if tag: tags.append(tag.identifier)
	abilities = []
	for ability in node.abilities:
		if ability: abilities.append(ability.identifier)


## Modifies the provided node by setting its tags, attributes, and abilities from this state.
func populate_node(node: AbilitySystem) -> void:
	node.attributes = {}
	for attribute_identifier in attributes.keys():
		var attribute := AttributeDB.find(attribute_identifier)
		if attribute: node.attributes[attribute] = attributes[attribute_identifier]
	node.tags = []
	for tag_identifier in tags:
		var tag := TagDB.find(tag_identifier)
		if tag: node.tags.append(tag)
	node.abilities = []
	for ability_identifier in abilities:
		var ability := AbilityDB.find(ability_identifier)
		if ability: node.abilities.append(ability)


## Modifies the provided node by merging its tags, attributes, and abilities from this state.
func merge_into_node(node: AbilitySystem) -> void:
	for attribute_identifier in attributes.keys():
		var attribute := AttributeDB.find(attribute_identifier)
		if attribute:
			if node.has_attribute(attribute):
				node.set_attribute_value(attribute, attributes[attribute_identifier])
			else:
				node.attributes[attribute] = attributes[attribute_identifier]
	for tag_identifier in tags:
		var tag := TagDB.find(tag_identifier)
		if tag and not tag in node.tags: node.tags.append(tag)
	for ability_identifier in abilities:
		var ability := AbilityDB.find(ability_identifier)
		if ability and not ability in node.abilities: node.abilities.append(ability)


## Modifies this resource by merging tags, attributes, and abilities from the provided state into this resource.
func merge(other: AbilitySystemState) -> void:
	attributes.merge(other.attributes)
	for other_tag in other.tags:
		if not other_tag in tags: tags.append(other_tag)
	for other_ability in other.abilities:
		if not other_ability in abilities: abilities.append(other_ability)


## Modifies this resource by merging tags, attributes, and abilities from the provided node into this resource.
func merge_from_node(node: AbilitySystem) -> void:
	merge(AbilitySystemState.new_from_node(node))


## Creates a new resource representing the current state of the provided node.
static func new_from_node(node: AbilitySystem) -> AbilitySystemState:
	var state := AbilitySystemState.new()
	state.populate_from_node(node)
	return state
