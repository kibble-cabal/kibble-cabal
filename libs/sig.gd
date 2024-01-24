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
