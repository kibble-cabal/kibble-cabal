class_name FilterByTagGroupsQueryFilter extends QueryFilter

enum CheckFor {
	SOME_TAGS_IN_SOME_GROUPS,
	SOME_TAGS_IN_ALL_GROUPS,
	NO_TAGS_IN_GROUPS
}

@export var tag_groups_to_check: Array[ATagGroup] = []
@export var check_for: CheckFor = CheckFor.SOME_TAGS_IN_SOME_GROUPS


func filter(results: Array) -> Array:
	return results.filter(_filter_item)


func _filter_item(item) -> bool:
	if not item is PhysicsQueryResult: return false
	if not item.collider: return false
	var component := _get_child_component(item.collider)
	if component:
		match check_for:
			CheckFor.SOME_TAGS_IN_SOME_GROUPS: return component.has_some_tags_in_some_groups(tag_groups_to_check)
			CheckFor.SOME_TAGS_IN_ALL_GROUPS: return component.has_some_tags_in_all_groups(tag_groups_to_check)
			CheckFor.NO_TAGS_IN_GROUPS: return not component.has_some_tags_in_some_groups(tag_groups_to_check)
	return false


func _get_child_component(node: Node) -> AbilitySystemComponent:
	for child in node.get_children():
		if child is AbilitySystemComponent: return child
		var component := _get_child_component(child)
		if component: return component
	return null
