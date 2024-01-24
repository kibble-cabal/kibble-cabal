class_name PetActionMenuItem extends ActionMenuItem

class Ctx:
	var node: CharacterBody2D
	var resource: PetResource
	
	func _init(node: CharacterBody2D, resource: PetResource) -> void:
		self.node = node
		self.resource = resource
	
	func lua_fields() -> Array:
		return ["node", "resource"]


func _get_display_text(ctx: Ctx = null) -> String: 
	return super(ctx)


func _get_menu_identifiers(ctx: Ctx = null) -> Array[StringName]: 
	return super(ctx)


func _on_press(ctx: Ctx = null) -> void:
	super(ctx)


func _is_visible(ctx: Ctx = null) -> bool:
	return super(ctx)


func _update(node: Button, ctx: Ctx = null) -> void:
	super(node, ctx)


func update(node: Button, ctx: Ctx = null) -> void:
	super(node, ctx)


func render(ctx: Ctx = null) -> Button:
	return super(ctx)
