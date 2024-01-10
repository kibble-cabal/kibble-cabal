extends Object


func is_available(_save) -> bool:
	return true


func is_complete(_save) -> bool:
	return false


func complete(_save) -> int:
	print("Quest completed!")
	return OK
