class_name ItemRetailResource extends ModdableResource

@export var buy_price: int
@export var base_sell_price: int
@export_range(0, 1) var depreciation_rate: float = 0.5


func lua_fields() -> Array:
	return super() + ["buy_price", "base_sell_price", "depreciation_rate"]
