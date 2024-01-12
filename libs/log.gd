class_name Log

const LINE_CHAR: String = "⎯"
const LINE_LENGTH: int = 80
const LINE_START: String = "⎯⎯⎯ "


static func _bracket(inner: String) -> String:
	return "[" + inner + "]"


static func _caller_string(caller) -> String:
	if caller is String or caller is StringName: return _bracket(caller)
	if caller is Object: return _bracket(caller.to_string())
	return _bracket(str(caller))


static func from(caller, string: String) -> void:
	if not OS.is_debug_build(): return
	print_rich(
		Bb.grey(_caller_string(caller)),
		" ",
		Bb.white(string)
	)


static func log(string: String) -> void:
	if not OS.is_debug_build(): return
	print_rich(Bb.white(string))


static func bullet(string: String, indent: int = 1) -> void:
	if not OS.is_debug_build(): return
	print_rich(Bb.grey("".lpad(indent * 2, " ") + "• " + string))


static func start_section(caller, header: String = "") -> void:
	if not OS.is_debug_build(): return
	var header_string := _caller_string(caller) + " " + header + " "
	print()
	print_rich(Bb.bold(Bb.white((LINE_START + header_string).rpad(LINE_LENGTH, LINE_CHAR))))


static func end_section(caller, string: String = "") -> void:
	if not OS.is_debug_build(): return
	line(_caller_string(caller) + " " + ((string + " ") if len(string) else ""))
	print()


static func line(string: String = "") -> void:
	if not OS.is_debug_build(): return
	if len(string): print_rich((LINE_START + string).rpad(LINE_LENGTH, LINE_CHAR))
	else: print_rich("".rpad(LINE_LENGTH, LINE_CHAR))


static func warning_from(caller, string: String, push: bool = true) -> void:
	from(caller, Bb.yellow(string))
	if push: push_warning(_caller_string(caller) + " " + string)


static func warning(string: String, push: bool = true) -> void:
	print_rich(Bb.yellow(string))
	if push: push_warning(string)


static func error_from(caller, string: String, push: bool = true) -> void:
	from(caller, Bb.red(string))
	if push: push_error(_caller_string(caller) + " " + string)


static func error(string: String, push: bool = true) -> void:
	print_rich(Bb.red(string))
	if push: push_error(string)


func lua_fields() -> Array:
	return ["start_section", "end_section", "line", "bullet", "log", "from", "warning_from", "warning", "error_from", "error"]
