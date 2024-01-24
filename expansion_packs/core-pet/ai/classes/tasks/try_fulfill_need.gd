@tool
extends BTFulfillNeed


@export var need_attribute: Attribute:
	set(value):
		need_attribute = value
		emit_changed()

@export var need_provider_tag: Tag:
	set(value):
		need_provider_tag = value
		emit_changed()

## This query searches for items with [member need_provider_tag] in the area.
var query := PhysicsQuery.new()

## If [code]true[/code], [member query] has already been run.
var has_run_query := false


func _generate_name() -> String:
	if need_attribute: return "Try Fulfilling {0}".format([need_attribute])
	else: return "Try Fulfilling Need"


func _get_configuration_warning() -> PackedStringArray:
	var warning := super()
	if not need_attribute: warning.append("Missing need attribute!")
	if not need_provider_tag: warning.append("Missing need provider tag!")
	return warning


func _enter() -> void:
	super()
	_build_query()


func _tick(delta: float) -> Status:
	# Fail if missing ability system or missing attribute.
	var ability_system := get_ability_system()
	if not ability_system or not ability_system.has_attribute(need_attribute): 
		_warning("Missing ability system or {0}.".format([need_attribute]))
		return FAILURE
	
	# Get value of need attribute.
	var need_value := ability_system.get_attribute_value(need_attribute)
	var need_is_low := need_value > (need_attribute.max_value - need_attribute.min_value) / 2 + need_attribute.min_value
	
	# Fail if need value is above 50% (only when acting autonomously, not during instructions).
	if not is_instruction() and need_is_low:
		_warning("{0} is not low.".format([need_attribute]))
		return FAILURE
	
	# Run query.
	if not has_run_query:
		has_run_query = true
		var result = query.query(agent)
		# Fail if no result was found.
		if result and result is PhysicsQueryResult:
			if target_item_variable.length(): blackboard.set_var(target_item_variable, result.collider)
		else:
			_warning("No item to fulfill {0} found.".format([need_attribute]))
			return FAILURE
	
	return super(delta)


func _build_query() -> void:
	if (not "resource" in agent 
		or not "collision_mask" in agent
		or not need_provider_tag): return
	
	var pet := get_pet_resource()
	if not pet: return
	
	var animal := pet.get_animal_resource()
	if not animal: return
	
	# Update detection
	var region := CircleShape2D.new()
	region.radius = animal.detection_radius
	query.region = region
	query.detect_areas = true
	query.collision_mask = Bit.L4
	
	# Add filters
	var filter := FilterByTagsQueryFilter.new()
	filter.tags_to_check = [need_provider_tag]
	query.filters = [filter]
	
	# Add transformations
	var distance_transform := SortByDistanceQueryTransformation.new()
	var index_transform := GetIndexQueryTransformation.new()
	query.transformations = [distance_transform, index_transform]


func get_pet_resource() -> PetResource:
	if agent and "resource" in agent:
		return agent.resource as PetResource
	return null


func is_instruction() -> bool:
	return blackboard.get_var(&"context/is_instruction", false)
