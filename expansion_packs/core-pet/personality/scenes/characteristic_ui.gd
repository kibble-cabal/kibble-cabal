extends HBoxContainer

@export var characteristic: Attribute
@export var ability_system: AbilitySystem

@onready var label := $Label as Label
@onready var pill_range := $PillRange as PillRange


func _ready() -> void:
	if characteristic:
		characteristic.changed.connect(update)
	if ability_system:
		ability_system.attribute_value_changed.connect(
			func(attribute: Attribute, _value: float) -> void:
				if attribute == characteristic: update()
		)


func update() -> void:
	if not characteristic or not ability_system: return
	label.text = characteristic.identifier
	pill_range.min_value = characteristic.min_value
	pill_range.max_value = characteristic.max_value
	pill_range.step = (characteristic.max_value - characteristic.min_value) / 5
	pill_range.value = ability_system.get_attribute_value(characteristic)

