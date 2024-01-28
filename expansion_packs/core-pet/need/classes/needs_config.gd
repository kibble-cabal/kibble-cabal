class_name NeedsConfig

static var Needs := PackedStringArray([
	"hunger",
	"thirst",
	"energy",
	"activity"
])

static var FulfillNeedAbilities := PackedStringArray([
	"eat",
	"sleep",
	"drink",
	"play"
])


static func lua_fields() -> Array:
	return ["Needs", "FulfillNeedAbilities"]
