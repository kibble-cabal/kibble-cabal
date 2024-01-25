class_name InstructionActionMenuItem extends PetActionMenuItem

@export var instruction_tree: BehaviorTree
@export var display_text: String


func _get_display_text(_ctx: Ctx = null) -> String:
	return display_text


func _get_menu_identifiers(_ctx: Ctx = null) -> Array[StringName]:
	return [&"pet/interact"]


func _on_press(ctx: Ctx = null) -> void:
	if not ctx or not ctx.resource or not instruction_tree: return
	Log.log("Giving instruction \"{0}\" to pet.".format([display_text]))
	ctx.resource.instructions.append(instruction_tree)


func lua_fields() -> Array:
	return super() + ["instruction_tree", "display_text"]
