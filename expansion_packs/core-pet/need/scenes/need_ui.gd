extends HBoxContainer

@export var ability_system: AbilitySystem:
	set(value):
		ability_system = value
		_connect_ability_system()

@export var attribute: Attribute:
	set(value):
		attribute = value
		_connect_attribute()


@onready var label := %Label as Label
@onready var progress_bar := %ProgressBar as ProgressBar


func _ready() -> void:
	_connect_ability_system()
	_connect_attribute()
	update()


func update() -> void:
	if ability_system and attribute:
		label.text = attribute.identifier.replace("_", " ").capitalize()
		progress_bar.min_value = attribute.min_value
		progress_bar.max_value = attribute.max_value
		progress_bar.step = (attribute.max_value - attribute.min_value) / 100
		progress_bar.value = ability_system.get_attribute_value(attribute)


func _connect_ability_system() -> void:
	if ability_system: Sig.try_connect(
		ability_system.attribute_value_changed,
		_on_attribute_value_changed
	)


func _connect_attribute() -> void:
	#set_meta(&"need_attribute", attribute)
	if attribute: Sig.try_connect(attribute.changed, update)


func _on_attribute_value_changed(changed_attribute: Attribute, _value: float) -> void:
	if changed_attribute == attribute: update()
