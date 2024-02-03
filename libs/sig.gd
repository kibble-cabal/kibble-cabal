class_name Sig


static func try_connect(sig: Signal, callable: Callable) -> void:
	if not sig.is_connected(callable):
		sig.connect(callable)


static func try_disconnect(sig: Signal, callable: Callable) -> void:
	if sig.is_connected(callable):
		sig.disconnect(callable)


static func disconnect_all(sig: Signal) -> void:
	for connection in sig.get_connections():
		sig.disconnect(connection.callable)


## Disconnects all callables in which the calling object is the same as [code]object[/code].
static func disconnect_all_for_object(object: Object, sig: Signal) -> void:
	for connection in sig.get_connections():
		if (connection.callable as Callable).get_object() == object:
			sig.disconnect(connection.callable)


## Disconnects [code]from_object[sig_name][/code] from the provided callable. 
## Connects [code]to_object[sig_name][/code] to the provided callable.
static func switch_connection(
	from_object: Object, 
	to_object: Object, 
	sig_name: StringName, 
	callable: Callable
) -> void:
	if from_object and from_object.has_signal(sig_name):
		Sig.try_disconnect(from_object[sig_name], callable)
	if to_object and to_object.has_signal(sig_name):
		Sig.try_connect(to_object[sig_name], callable)
