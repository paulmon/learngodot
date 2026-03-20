extends Node

var _controller: Object = null

func _ready() -> void:
	_ensure_action_key("move_up", KEY_W)
	_ensure_action_key("move_down", KEY_S)
	_ensure_action_key("move_left", KEY_A)
	_ensure_action_key("move_right", KEY_D)
	_ensure_action_key("undo", KEY_Z)
	_ensure_action_key("restart", KEY_R)

	if ClassDB.class_exists("RustGameController"):
		_controller = ClassDB.instantiate("RustGameController")
		if _controller != null:
			add_child(_controller as Node)
			print("RustGameController instantiated")
		else:
			push_error("RustGameController class exists but could not instantiate")
	else:
		push_error("RustGameController class missing; gdextension did not load")

func _unhandled_input(event: InputEvent) -> void:
	if _controller == null:
		return

	if event.is_action_pressed("move_up"):
		_controller.call("handle_action", "move_up")
	elif event.is_action_pressed("move_down"):
		_controller.call("handle_action", "move_down")
	elif event.is_action_pressed("move_left"):
		_controller.call("handle_action", "move_left")
	elif event.is_action_pressed("move_right"):
		_controller.call("handle_action", "move_right")
	elif event.is_action_pressed("undo"):
		_controller.call("handle_action", "undo")
	elif event.is_action_pressed("restart"):
		_controller.call("handle_action", "restart")

func _ensure_action_key(action_name: StringName, keycode: Key) -> void:
	if not InputMap.has_action(action_name):
		InputMap.add_action(action_name)

	for input_event in InputMap.action_get_events(action_name):
		if input_event is InputEventKey and input_event.physical_keycode == keycode:
			return

	var key_event := InputEventKey.new()
	key_event.physical_keycode = keycode
	InputMap.action_add_event(action_name, key_event)
