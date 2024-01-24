extends Object


func before_exit() -> void:
	SaveSystem.commit_changes()
