class_name BuildingConfig

static var WallHeight: float = 2
static var WallThickness: float = 0.1
static var FloorThickness: float = 0.1


static func lua_fields() -> Array:
	return [
		"WallHeight",
		"WallThickness",
		"FloorThickness"
	]
