class_name FateResource extends ModdableResource

@export var amount: int = 0:
	set(value):
		amount = value
		emit_changed()
