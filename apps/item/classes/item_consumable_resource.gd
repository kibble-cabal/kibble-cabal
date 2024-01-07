class_name ItemConsumableResource extends ModdableResource

@export var total_uses: int = 1


func lua_fields() -> Array[String]:
	return super() + ["total_uses"]
