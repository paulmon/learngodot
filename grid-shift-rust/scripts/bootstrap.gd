extends Node

func _ready() -> void:
	if ClassDB.class_exists("RustProbe"):
		var rust_node: Object = ClassDB.instantiate("RustProbe")
		if rust_node != null:
			add_child(rust_node as Node)
			print("RustProbe instantiated")
		else:
			push_error("RustProbe class exists but could not instantiate")
	else:
		push_error("RustProbe class missing; gdextension did not load")

	get_tree().quit()
