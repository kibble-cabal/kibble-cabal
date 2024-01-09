# ExpansionPackSystem
extends Node


## List of expansion pack IDs that have been initialized
var initialized_expansion_packs: Array[String]


func _ready() -> void:
	ExpansionPackDB.registered_packs.map(initialize)
	ExpansionPackDB.pack_registered.connect(initialize)


func initialize(pack: ExpansionPackResource) -> void:
	if not pack.id in initialized_expansion_packs:
		print("Initializing expansion pack: ", pack.display_name)
		initialized_expansion_packs.append(pack.id)
		if pack.entry_script:
			pack.entry_script.new()
