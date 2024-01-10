class_name Log


static func log_from(caller: Object, string: String) -> void:
	print_rich(
		Bb.grey("[" + caller.to_string() + "]"),
		" ",
		Bb.white(string)
	)


static func log(string: String) -> void:
	print_rich(Bb.white(string))


static func log_bullet(string: String, indent: int = 1) -> void:
	print_rich(Bb.grey("".lpad(indent * 2, " ") + "• " + string))


static func start_section(caller: Object) -> void:
	print_rich(
		"\n-----",
		"\n" + Bb.bold(Bb.white("[" + caller.to_string() + "]")),
	)


static func end_section() -> void:
	print("-----\n")
