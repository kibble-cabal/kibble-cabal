class_name Bit

const L1 = 1 << 0
const L2 = 1 << 1
const L3 = 1 << 2
const L4 = 1 << 3
const L5 = 1 << 4
const L6 = 1 << 5
const L7 = 1 << 6
const L8 = 1 << 7
const L9 = 1 << 8
const L10 = 1 << 9
const L11 = 1 << 10
const L12 = 1 << 11


enum Physics {
	WORLD = L1,
	PLAYERS = L2,
	PETS = L3,
	ITEMS = L4,
	BUILDINGS = L5,
	
	# UI
	UI_DRAG = L10,
	UI_DROP = L11,
	UI_PHYSICS_RAY = L12
}
