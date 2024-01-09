class_name FateResource extends ModdableResource

@export var amount: int = 0:
	set(value):
		amount = value
		emit_changed()


func earn(fate_amount: int) -> void:
	amount += fate_amount


func lose(fate_amount: int) -> void:
	amount -= fate_amount


func lua_fields() -> Array[String]:
	return super() + ["amount", "earn", "lose"]
