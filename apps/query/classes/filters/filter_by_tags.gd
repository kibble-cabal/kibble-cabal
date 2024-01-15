class_name FilterByTagsQueryFilter extends QueryFilter

enum CheckFor {
	ALL,
	NONE,
	SOME
}

@export var tags_to_check: Array[ATag] = []
@export var check_for: CheckFor = CheckFor.SOME


func filter(results: Array) -> Array:
	return results.filter(_filter_item)


func _filter_item(item) -> bool:
	if not item is PhysicsQueryResult: return false
	if not item.collider: return false
	var component := _get_child_component(item.collider)
	if component:
		match check_for:
			CheckFor.ALL: return component.has_tags(tags_to_check)
			CheckFor.SOME: return component.has_some_tags(tags_to_check)
			CheckFor.NONE: return not component.has_some_tags(tags_to_check)
	return false


func _get_child_component(node: Node) -> AbilitySystemComponent:
	for child in node.get_children():
		if child is AbilitySystemComponent: return child
		var component := _get_child_component(child)
		if component: return component
	return null
