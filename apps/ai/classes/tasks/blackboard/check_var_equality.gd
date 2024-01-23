@tool
extends BTAction

enum Op {
	EQUAL_TO,
	NOT_EQUAL_TO
}

@export var variable: BBVariant:
	set(value):
		variable = value
		emit_changed()

@export var value: BBVariant:
	set(value):
		value = value
		emit_changed()

@export var operator := Op.EQUAL_TO:
	set(value):
		operator = value
		emit_changed()



func _generate_name() -> String:
	return "Check {variable} is {operator} {value}".format({
		variable = variable,
		operator = _operator_string(),
		value = value
	})


func _tick(_delta: float) -> Status:
	if _is_success(): return SUCCESS
	return FAILURE


func _is_success() -> bool:
	var lhs = variable.get_value(agent, blackboard)
	var rhs
	if value == null: rhs = null
	elif value.value_source == BBVariant.ValueSource.SAVED_VALUE: rhs = value.saved_value
	else: rhs = value.get_value(agent, blackboard)
	match operator:
		Op.EQUAL_TO: return lhs == rhs
		Op.NOT_EQUAL_TO: return lhs != rhs
	return false


func _operator_string() -> String:
	return Op.find_key(operator).to_lower().replace("_", " ")
