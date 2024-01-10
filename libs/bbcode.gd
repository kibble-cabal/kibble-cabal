class_name Bb

## Helper methods to quickly format BBCode strings

static func code(text: String) -> String:
	return "[code]{0}[/code]".format([text])

static func italic(text: String) -> String:
	return "[i]{0}[/i]".format([text])


static func bold(text: String) -> String:
	return "[b]{0}[/b]".format([text])


static func pink(text: String) -> String:
	return "[color=pink]{0}[/color]".format([text])


static func grey(text: String) -> String:
	return "[color=gray]{0}[/color]".format([text])


static func white(text: String) -> String:
	return "[color=white]{0}[/color]".format([text])
