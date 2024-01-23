# AttributeDB
extends Node


signal attribute_registered(attribute: Attribute)
signal attribute_unregistered(attribute: Attribute)


var registered_attributes: Array[Attribute] = []


func register(attribute: Attribute) -> void:
	registered_attributes.append(attribute)
	attribute_registered.emit(attribute)


func unregister(attribute: Attribute) -> void:
	registered_attributes.erase(attribute)
	attribute_unregistered.emit(attribute)


func find(identifier: String) -> Attribute:
	for attribute in registered_attributes:
		if attribute.identifier == identifier: return attribute
	return null


func lua_fields() -> Array:
	return [
		"registered_attributes", 
		"register", 
		"unregister", 
		"find",
	]
