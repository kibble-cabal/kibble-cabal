extends PetActionMenuItem

const RandomNames = [
	"Fluffy",
	"Buzz",
	"Fido",
	"Princess"
]


func _get_display_text(ctx: PetActionMenuItem.Ctx = null) -> String:
	if ctx and ctx.resource and ctx.resource.name.length():
		return "Rename {0}...".format([ctx.resource.name])
	return "Rename..."


func _get_menu_identifiers(_ctx: PetActionMenuItem.Ctx = null) -> Array[StringName]:
	return [&"pet/interact"]


func _on_press(ctx: PetActionMenuItem.Ctx = null) -> void:
	if ctx and ctx.resource:
		ctx.resource.name = RandomNames.pick_random()
		prints("Renamed to:", ctx.resource.name)


func _is_visible(_ctx: PetActionMenuItem.Ctx = null) -> bool:
	return true
