class_name Log

const LINE_CHAR: String = "⎯"
const LINE_LENGTH: int = 80
const LINE_START: String = "⎯⎯⎯ "


static func from(caller: Object, string: String) -> void:
	if not OS.is_debug_build(): return
	print_rich(
		Bb.grey("[" + caller.to_string() + "]"),
		" ",
		Bb.white(string)
	)


static func log(string: String) -> void:
	if not OS.is_debug_build(): return
	print_rich(Bb.white(string))


static func bullet(string: String, indent: int = 1) -> void:
	if not OS.is_debug_build(): return
	print_rich(Bb.grey("".lpad(indent * 2, " ") + "• " + string))


static func start_section(caller: Object, header: String = "") -> void:
	if not OS.is_debug_build(): return
	var caller_string := "[" + caller.to_string() + "]"
	print_rich(
		"\n",
		Bb.bold(Bb.white((LINE_START + caller_string + " " + header + " ").rpad(LINE_LENGTH, LINE_CHAR)))
	)


static func end_section(caller: Object, string: String = "") -> void:
	if not OS.is_debug_build(): return
	line("[" + caller.to_string() + "] " + ((string + " ") if len(string) else ""))
	print()


static func line(string: String = "") -> void:
	if not OS.is_debug_build(): return
	if len(string): print_rich((LINE_START + string).rpad(LINE_LENGTH, LINE_CHAR))
	else: print_rich("".rpad(LINE_LENGTH, LINE_CHAR))
