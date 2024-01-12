# QuestDB
extends Node

signal quest_registered(quest: QuestResource)
signal quest_unregistered(quest: QuestResource)


var registered_quests: Array[QuestResource] = []


func register(quest: QuestResource) -> void:
	registered_quests.append(quest)
	quest_registered.emit(quest)


func unregister(quest: QuestResource) -> void:
	registered_quests.erase(quest)
	quest_unregistered.emit(quest)


func find_by_id(quest_id: String) -> QuestResource:
	for quest in registered_quests:
		if quest.id == quest_id: return quest
	return null


func find_by_name(quest_name: String) -> QuestResource:
	for quest in registered_quests:
		if quest.name == quest_name: return quest
	return null


func lua_fields() -> Array:
	return ["register", "find_by_id", "find_by_name", "registered_quests"]
