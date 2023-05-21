extends Node2D

var scanner_cursor = preload("res://prototypes/levels/level1/scanner.png")

var actual_object: String = "coxinha" : set = set_actual_object, get = get_actual_object

func set_actual_object(value):
	actual_object = value
	%ShowProduct.product_name = actual_object
	if OS.is_debug_build():
		dbg_actual_object.set_text("Actual Object: " + actual_object)

func get_actual_object():
	return actual_object

var progress: int = 0 : set = set_progress, get = get_progress

func set_progress(value):
	progress = value
	if OS.is_debug_build():
		%UI/Progress.value = progress
	
	if progress >= 7:
		get_tree().change_scene_to_file("res://prototypes/levels/next_level/next_level.tscn")

func get_progress():
	return progress



var score: int = 0 : set = set_score, get = get_score

func set_score(value):
	score = value
	if OS.is_debug_build():
		dbg_score.set_text("Score: " + str(score))

func get_score():
	return score

# Debug vars
var dbg_level_name: Label = Label.new()
var dbg_score: Label = Label.new()
var dbg_actual_object: Label = Label.new()
var dbg_objetos_na_esteira: Label = Label.new()
var dbg_add_progress: Button = Button.new()
var dbg_set_actual_object: LineEdit = LineEdit.new()

func _ready():
	Input.set_custom_mouse_cursor(scanner_cursor, Input.CURSOR_ARROW, Vector2(12,21))
	actual_object = "coxinha"

	if OS.is_debug_build():
		%DebugUI/Infos.add_child(dbg_level_name)
		%DebugUI/Infos.add_child(dbg_score)
		%DebugUI/Infos.add_child(dbg_actual_object)
		dbg_level_name.set_text("Level 1")
		dbg_score.set_text("Score: 0")
		dbg_actual_object.set_text("Actual Object: " + actual_object)
		%DebugUI/Infos.add_child(dbg_add_progress)
		dbg_add_progress.set_text("Add Progress")
		dbg_add_progress.button_down.connect(_increase_progress.bind())
		%DebugUI/Infos.add_child(dbg_set_actual_object)
		dbg_set_actual_object.placeholder_text = "Set Actual Object"
		dbg_set_actual_object.text_changed.connect(_set_actual_object.bind())
		%DebugUI/Infos.add_child(dbg_objetos_na_esteira)
		dbg_objetos_na_esteira.set_text("Objetos na esteira: " + str(get_node("CheckArea").has_overlapping_areas()))


func _increase_progress():
	progress += 1

func _set_actual_object(text):
	actual_object = text

func _on_Scanner(product_name) -> bool:
	print_debug("Scanner: " + product_name)
	if product_name == actual_object:
		score += 100
		progress += 1
		%Monitor.text = item_monitor[actual_object] + " Escaneado!"
		actual_object = item_monitor.keys()[randi() % item_monitor.size()]
		return true
	else:
		%Monitor.text = "Item Errado!"
		score -= 50
		return false

var item_monitor: Dictionary = {
	"coxinha": "Coxinha",
	"leite": "Caixa de Leite",
}

func _process(_delta):
	if OS.is_debug_build():
		dbg_objetos_na_esteira.set_text("Objetos na esteira: " + str(get_node("CheckArea").has_overlapping_areas()))