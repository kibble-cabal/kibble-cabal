class_name AbilitySystemState extends Resource

@export var attributes: Dictionary
@export var tags: Array[Tag]
@export var events: Array[AbilityEvent]
@export var abilities: Array[Ability]


## Modifies this resource by replacing tags, attributes, events, and abilities from the provided node.
func populate_from_node(node: AbilitySystem) -> void:
	attributes = node.attributes
	tags = node.tags
	events = node.events
	abilities = node.abilities


## Modifies the provided node by setting its tags, attributes, events, and abilities from this state.
func populate_node(node: AbilitySystem) -> void:
	node.attributes = attributes
	node.tags = tags
	node.events = events
	node.abilities = abilities


## Modifies the provided node by merging its tags, attributes, events, and abilities from this state.
func merge_into_node(node: AbilitySystem) -> void:
	node.attributes.merge(attributes)
	for tag in tags:
		if not tag in node.tags: node.tags.append(tag)
	for event in events:
		if not event in node.events: node.events.append(event)
	for ability in abilities:
		if not ability in node.abilities: node.abilities.append(ability)


## Modifies this resource by merging tags, attributes, events, and abilities from the provided state into this resource.
func merge(other: AbilitySystemState) -> void:
	attributes.merge(other.attributes)
	for other_tag in other.tags:
		if not other_tag in tags: tags.append(other_tag)
	for other_event in other.events:
		if not other_event in events: events.append(other_event)
	for other_ability in other.abilities:
		if not other_ability in abilities: abilities.append(other_ability)


## Modifies this resource by merging tags, attributes, events, and abilities from the provided node into this resource.
func merge_from_node(node: AbilitySystem) -> void:
	merge(AbilitySystemState.new_from_node(node))


## Creates a new resource representing the current state of the provided node.
static func new_from_node(node: AbilitySystem) -> AbilitySystemState:
	var state := AbilitySystemState.new()
	state.populate_from_node(node)
	return state
