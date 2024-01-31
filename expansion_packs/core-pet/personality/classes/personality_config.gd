class_name PersonalityConfig


static var Characteristics := PackedStringArray([
	"conscientiousness",
	"extraversion",
	"neuroticism",
	"agreeableness",
	"openness",
])


static func randomize_personality(ability_system: AbilitySystem, overwrite: bool = false) -> void:
	if not ability_system: return
	for id in Characteristics:
		var attribute := AttributeDB.find(id)
		if not attribute: continue
		var value := randf_range(attribute.min_value, attribute.max_value)
		var has_attribute := ability_system.has_attribute(attribute)
		if not has_attribute:
			ability_system.grant_attribute(attribute)
			ability_system.set_attribute_value(attribute, value)
		elif overwrite:
			ability_system.set_attribute_value(attribute, value)
