class_name NeedAttribute extends Attribute

@export_range(0, 1, 0.01) var depletion_rate: float = 0.5


func lua_fields() -> Array:
	return [
		"depletion_rate",
		"identifier",
		"max_value",
		"min_value",
		"default_value",
		"ui_color"
	]
