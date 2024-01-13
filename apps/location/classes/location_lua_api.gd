class_name LocationLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetLocationDB", func(): return LocationDB)
	lua.push_variant("GetLocationSystem", func(): return LocationSystem)


func expose_hooks(lua: LuaAPI) -> void:	
	LocationDB.location_registered.connect(
		func(location: LocationResource) -> void:
			lua.call_function("OnLocationRegistered", [location])
	)
	LocationDB.location_unregistered.connect(
		func(location: LocationResource) -> void:
			lua.call_function("OnLocationUnregistered", [location])
	)
	LocationSystem.location_entered.connect(
		func(location: LocationResource) -> void:
			lua.call_function("OnLocationEntered", [location])
	)
	LocationSystem.location_exited.connect(
		func(location: LocationResource) -> void:
			lua.call_function("OnLocationExited", [location])
	)
	LocationSystem.location_changed.connect(lua.call_function.bind("OnLocationChanged", []))
