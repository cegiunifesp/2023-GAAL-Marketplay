extends Sprite2D

var timer = randf_range(0.5, 2)

var falling_object = preload("res://prototypes/objects/falling_object.tscn")

@onready
var area: Area2D = get_parent().get_node("CheckArea")

@onready
var item_monitor: Dictionary = get_parent().item_monitor

func _ready():
	pass

func _process(delta):
	timer -= delta
	if timer <= 0 and not area.has_overlapping_areas():
		timer = randf_range(1, 5)
		var obj = falling_object.instantiate()
		obj.position = _get_random_position()
		obj.product_name = item_monitor.keys()[randi() % item_monitor.size()]
		print_debug(obj.product_name)
		get_parent().add_child(obj)
		
		
func _get_random_position():
	var x = randf_range(48, 320)
	var y = randf_range(144, 470)
	return Vector2(x, y)
