class_name AnimalLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetAnimalDB", func(): return AnimalDB)


func expose_hooks(lua: LuaAPI) -> void:
	AnimalDB.animal_registered.connect(
		func(animal: AnimalResource) -> void:
			lua.call_function("OnAnimalRegistered", [animal])
	)
	AnimalDB.animal_unregistered.connect(
		func(animal: AnimalResource) -> void:
			lua.call_function("OnAnimalUnregistered", [animal])
	)
