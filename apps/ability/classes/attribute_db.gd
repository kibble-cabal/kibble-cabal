# AttributeDB
extends Node


signal attribute_registered(attribute: AAttribute)
signal attribute_unregistered(attribute: AAttribute)

signal attribute_table_template_registered(attribute_table_template: AAttributeTableTemplate)
signal attribute_table_template_unregistered(attribute_table_template: AAttributeTableTemplate)


var registered_attributes: Array[AAttribute] = []
var registered_attribute_table_templates: Array[AAttributeTableTemplate] = []


func register_attribute(attribute: AAttribute) -> void:
	registered_attributes.append(attribute)
	attribute_registered.emit(attribute)


func unregister_attribute(attribute: AAttribute) -> void:
	registered_attributes.erase(attribute)
	attribute_unregistered.emit(attribute)


func find_attribute(attribute_name: String) -> AAttribute:
	for attribute in registered_attributes:
		if attribute.name == attribute_name: return attribute
	return null


func register_attribute_table_template(attribute_table_template: AAttributeTableTemplate) -> void:
	registered_attribute_table_templates.append(attribute_table_template)
	attribute_table_template_registered.emit(attribute_table_template)


func unregister_attribute_table_template(attribute_table_template: AAttributeTableTemplate) -> void:
	registered_attribute_table_templates.erase(attribute_table_template)
	attribute_table_template_unregistered.emit(attribute_table_template)


func find_attribute_table_template(attribute_table_template_name: String) -> AAttributeTableTemplate:
	for attribute_table_template in registered_attribute_table_templates:
		if attribute_table_template.resource_name == attribute_table_template_name: return attribute_table_template
	return null


func lua_fields() -> Array:
	return [
		"registered_attributes", 
		"register_attribute", 
		"unregister_attribute", 
		"find_attribute", 
		"registered_attribute_table_templates", 
		"register_attribute_table_template", 
		"unregister_attribute_table_template", 
		"find_attribute_table_template"
	]
