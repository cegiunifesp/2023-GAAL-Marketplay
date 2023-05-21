extends Area2D

var speed = 100
var velocity = Vector2()
var grav = 10

@onready
var esteira_pos: Vector2 = Vector2(0, 500)

enum STATES {
	FALLING,
	ESTEIRA,
	FALLING_OFF
}

var state: STATES = STATES.FALLING

@onready
var anim: AnimationPlayer = $AnimationPlayer

@export
var product_name: String = "leite" : set = set_product_name, get = get_product_name

func _input_event(_viewport, _event, _shape_idx):
	if Input.is_action_just_pressed("mouse_left") and state == STATES.ESTEIRA:
		var result: bool = get_parent()._on_Scanner(product_name)
		if result:
			state = STATES.FALLING_OFF
		else:
			anim.play("Treme")


func set_product_name(value: String):
	product_name = value
	$product.product_name = value

func get_product_name():
	return product_name

func _process(delta):
	match state:
		STATES.FALLING:
			$product.rotate(grav * delta / 10) 
			velocity.y += grav
			position += velocity * delta
			if position.y > esteira_pos.y:
				anim.play("Treme")
				state = STATES.ESTEIRA
		STATES.ESTEIRA:
			velocity.x = speed
			velocity.y = 0
			position += velocity * delta
			if position.x > 1000:
				state = STATES.FALLING_OFF
		STATES.FALLING_OFF:
			velocity.y += grav
			position += velocity * delta
			if position.y > get_viewport_rect().size.y:
				queue_free()
		

