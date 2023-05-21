@tool
extends Node2D

@export
var product_name: String = "coxinha" : set = set_product_name, get = get_product_name

func set_product_name(value: String) -> void:
	product_name = value
	$Sprite2D.texture = load("prototypes/objects/" + product_name + ".png")

func get_product_name() -> String:
	return product_name


func _ready():
	$Sprite2D.texture = load("prototypes/objects/" + product_name + ".png")