class_name History extends RefCounted

## This class handles undo and redo.

class Item extends RefCounted:
	var caller: Object
	var name: String
	var do_methods: Array[Callable]
	var undo_methods: Array[Callable]
	
	var time := Time.get_ticks_msec()
	
	var mergable: bool = false
	
	## If provided, will be called to check if it's possible to merge two [Item]s.
	## [br][code]func(a: Item, b: Item) -> bool[/code]
	var can_merge_method: Callable
	
	## If provided, will be run when possible to merge two [Item]s.
	## [br][code]func(a: Item, b: Item) -> Item[/code]
	var custom_merge_method: Callable
	
	var renderable: bool = true
	
	
	func _init(
		caller_value: Object,
		name_value: String,
		do_methods_value: Array[Callable] = [],
		undo_methods_value: Array[Callable] = [],
		mergable_value := false,
		can_merge_method_value := Callable(),
		custom_merge_method_value := Callable(),
		renderable_value := true,
	) -> void:
		caller = caller_value
		name = name_value
		do_methods = do_methods_value
		undo_methods = undo_methods_value
		mergable = mergable_value
		can_merge_method = can_merge_method_value
		custom_merge_method = custom_merge_method_value
		renderable = renderable_value
	
	
	func do() -> void:
		for method in do_methods:
			if method.is_valid(): method.call()
	
	
	func undo() -> void:
		for i in range(undo_methods.size(), 0, -1):
			var method := undo_methods[i - 1]
			if method.is_valid(): method.call()
	
	
	func can_merge(with: Item) -> bool:
		if mergable and name == with.name and abs(with.time - time) < 10000:
			if can_merge_method.is_valid(): return can_merge_method.call(self, with)
			else: return true
		return false
	
	
	## Returns a list containing the merged item (if merging is possible) or a list of both items if merging is impossible.
	func merge(with: Item) -> Array[Item]:
		if can_merge(with):
			if custom_merge_method.is_valid():
				return [custom_merge_method.call(self, with)]
			else:
				do_methods.append_array(with.do_methods)
				undo_methods.append_array(with.undo_methods)
				return [self]
		return [self, with]


signal before_do(item: Item)
signal after_do(item: Item)
signal before_undo(item: Item)
signal after_undo(item: Item)
signal before_redo(item: Item)
signal after_redo(item: Item)
signal changed


## If provided, undo/redo notifications will be rendered to this node.
var ui_node: Node
var render_do: bool = false
var render_undo: bool = true
var render_redo: bool = true

var stack: Array[Item] = []
var undone_stack: Array[Item] = []


func add(caller: Object, name: String, do_method := Callable(), undo_method := Callable(), renderable := true) -> Item:
	return add_multi(caller, name, [do_method], [undo_method], renderable)


func add_multi(
	caller: Object,
	name: String,
	do_methods: Array[Callable] = [],
	undo_methods: Array[Callable] = [],
	renderable := true,
) -> Item:
	var item := Item.new(caller, name, do_methods, undo_methods)
	item.renderable = renderable
	return add_item(item)


func merge_add(
	caller: Object,
	name: String,
	do_method := Callable(),
	undo_method := Callable(),
	can_merge_method := Callable(),
	custom_merge_method := Callable(),
	renderable := true,
) -> Item:
	return add_item(Item.new(
		caller,
		name, 
		[do_method],
		[undo_method], 
		true,
		can_merge_method,
		custom_merge_method,
		renderable
	))


func add_item(item: Item) -> Item:
	# Add item to stack
	if not stack.is_empty():
		var prev_item: Item = stack.pop_back()
		stack.append_array(prev_item.merge(item))
	else: stack.append(item)
	
	# Clear undo
	undone_stack.clear()
	
	# Perform item
	before_do.emit(item)
	item.do()
	after_do.emit(item)
	changed.emit()
	
	if render_do and item.renderable:
		_render_notification(item.name)
	
	return item


func undo() -> void:
	if not stack.is_empty():
		var item: Item = stack.pop_back()
		before_undo.emit(item)
		item.undo()
		undone_stack.append(item)
		after_undo.emit(item)
		changed.emit()
		
		if render_undo and item.renderable:
			_render_notification("Undo " + item.name)


func redo() -> void:
	if not undone_stack.is_empty():
		var item: Item = undone_stack.pop_back()
		before_redo.emit(item)
		item.do()
		stack.append(item)
		after_redo.emit(item)
		changed.emit()
		if render_redo and item.renderable:
			_render_notification("Redo " + item.name)


func clear() -> void:
	stack.clear()
	undone_stack.clear()
	changed.emit()


func on_after_do(item_name: StringName, callable: Callable) -> void:
	after_do.connect(
		func(item: Item) -> void:
			if item.name == item_name and callable.is_valid(): callable.call()
	)


func on_after_undo(item_name: StringName, callable: Callable) -> void:
	after_undo.connect(
		func(item: Item) -> void:
			if item.name == item_name and callable.is_valid(): callable.call()
	)


func on_after_redo(item_name: StringName, callable: Callable) -> void:
	after_redo.connect(
		func(item: Item) -> void:
			if item.name == item_name and callable.is_valid(): callable.call()
	)


func _render_notification(text: String) -> void:
	if ui_node:
		var label := Label.new()
		label.theme_type_variation = &"HistoryNotification"
		label.text = text
		label.modulate.a = 0
		ui_node.add_child(label)
		ui_node.move_child(label, 0)
		var tween := label.create_tween()
		tween.tween_property(label, "modulate:a", 1.0, 0.5)
		tween.tween_property(label, "modulate:a", 0.0, 0.5).set_delay(2.0)
		tween.tween_callback(func(): if Nodes.can_queue_free(label): label.queue_free())
