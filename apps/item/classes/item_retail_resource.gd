class_name ItemRetailResource extends ModdableResource

@export var buy_price: int
@export var base_sell_price: int
@export_range(0, 1) var depreciation_rate: float = 0.5
