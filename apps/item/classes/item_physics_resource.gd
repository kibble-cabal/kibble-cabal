class_name ItemPhysicsResource extends ModdableResource

enum Flag {
	CAN_WALK_THROUGH = 1,
	CAN_PLACE_ON_FLOOR = 2,
	CAN_PLACE_ON_SURFACE = 4,
	CAN_PLACE_ON_WALL = 8,
	CAN_PLACE_ON_CEILING = 16
}

@export var scene: PackedScene

@export_flags(
	"CanWalkThrough:1", 
	"CanPlaceOnFloor:2", 
	"CanPlaceOnSurface:4", 
	"CanPlaceOnWall:8", 
	"CanPlaceOnCeiling:16"
) var flags = 0
