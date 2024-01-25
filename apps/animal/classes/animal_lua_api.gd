class_name AnimalLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetAnimalDB", func(): return AnimalDB)
	
	lua.push_variant(
		"LoadAnimal",
		func(path: String) -> AnimalResource:
			return load_json(lua, path, AnimalResource.make_loader()) as AnimalResource
	)
	lua.push_variant(
		"CreateAnimal",
		func(data: Dictionary) -> AnimalResource:
			return AnimalResource.make_loader().load_from_string(JSON.stringify(data)) as AnimalResource
	)


func expose_hooks(lua: LuaAPI) -> void:
	AnimalDB.animal_registered.connect(
		func(animal: AnimalResource) -> void:
			lua.call_function("OnAnimalRegistered", [animal])
	)
	AnimalDB.animal_unregistered.connect(
		func(animal: AnimalResource) -> void:
			lua.call_function("OnAnimalUnregistered", [animal])
	)
