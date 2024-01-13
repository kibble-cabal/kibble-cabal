class_name QuestResource extends ModdableResource

@export var id: String
@export var display_name: String
@export var display_description: String

## This scene will be rendered within the quest UI. It probably shouldn't have much functionality, just visualize the player's progress, whether they're completed or not, etc.
## It should have this function: [code]func render(quest: QuestResource) -> void[/code]
@export var ui: PackedScene

@export_category("Scripts")

## This script handles checking whether or not a quest is available to be displayed to the player. 
## It should check that it's been unlocked and not yet completed.
@export var check_quest_available_script: GDScript
## The name of the method in the script [member check_quest_available_script] to check if the quest is available
## [br]Should have the signature: [code]func (save: SaveResource) -> bool[/code]
@export var check_quest_available_method: String = "main"

## This script handles checking whether or not the quest requirements are completed by the player
@export var check_quest_complete_script: GDScript
## The name of the method in the script [member check_quest_complete_script] to check if the quest is complete
## [br]Should have the signature: [code]func (save: SaveResource) -> bool[/code]
@export var check_quest_complete_method: String = "main"

## This script handles completion of the quest by the user, giving reward, etc
@export var complete_quest_script: GDScript
## The name of the method in the script [member complete_quest_script] to complete the quest.
## [br]Should have the signature: [code]func (save: SaveResource) -> Error[/code]
@export var complete_quest_method: String = "main"


func is_available(save: SaveResource) -> bool:
	return call_subscript(check_quest_available_script, check_quest_available_method, [save], false)


func is_complete(save: SaveResource) -> bool:
	return call_subscript(check_quest_complete_script, check_quest_complete_method, [save], false)


func complete(save: SaveResource) -> bool:
	return call_subscript(complete_quest_script, complete_quest_method, [save], FAILED)


func lua_fields() -> Array:
	return super() + [
		"id",
		"display_name",
		"display_description",
		"ui",
		"check_quest_available_script",
		"check_quest_available_method",
		"check_quest_complete_script",
		"check_quest_complete_method",
		"complete_quest_script",
		"complete_quest_method",
		"is_available",
		"is_complete",
		"complete"
	]
