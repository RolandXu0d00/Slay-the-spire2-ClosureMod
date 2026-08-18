extends AnimatedSprite2D

# 休息点立绘自动播放：AnimatedSprite2D.playing 是只读属性，场景里设 playing=true 无效，
# 必须在 _ready 里调用 play() 启动 Sit 循环动画。
func _ready():
	play()
