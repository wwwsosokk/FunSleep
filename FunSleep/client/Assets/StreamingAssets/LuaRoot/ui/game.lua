-- game.lua
-- 游戏逻辑

require("unity")

game = {}

local isInGame = false
local selectIndex = 0

local state1Toggle = nil
local state2Toggle = nil
local state3Toggle = nil

local buttonText = nil
local tipText = nil
local resultAnimator = nil
local spariteAtlas = nil
local resultImg = nil

local random_range = 20
local isWin = false

local pair = nil

local function LockScene()
end

local function onState1Toggle(isOn)
    if isInGame then
        return
    end
    
    if isOn then
        selectIndex = 1
    end
end

local function onState2Toggle(isOn)
    if isInGame then
        return
    end
    
    if isOn then
        selectIndex = 2
    end
end

local function onState3Toggle(isOn)
    if isInGame then
        return
    end
    
    if isOn then
        selectIndex = 3
    end
end

local function calculateResult()
    isInGame = true
	state1Toggle.enabled = false
	state2Toggle.enabled = false
	state3Toggle.enabled = false

	resultAnimator.enabled = true
	resultAnimator:Play("result")
	tipText.text = "。。。"

	WaitForSeconds(2)

	buttonText.text = "再来一次"
	resultAnimator:StopPlayback()
	resultAnimator.enabled = false

	local random = Random.Range(0, random_range)
	local index = 1

	if random < 3 then
		-- win
		tipText.text = "哇，今天运气不赖嘛，安心入睡吧。"
		buttonText.text = "点击锁屏"
		isWin = true

		if (1 == selectIndex) then
			index = 2
		elseif (2 == selectIndex) then
			index = 3
		elseif (3 == selectIndex) then
			index = 1
		end
			
		random_range = random_range + 5
		if (random_range > 20) then
			random_range = 20
		end
	elseif random >= 3 and random < 5 then
		-- same
		tipText.text = "平局啊！"
		index = selectIndex
	else
		-- lose
		tipText.text = "手气真挫！"

		if (1 == selectIndex) then
			index = 3
		elseif (2 == selectIndex) then
			index = 1
		elseif (3 == selectIndex) then
			index = 2
	    end

		random_range = random_range - 5
		if (random_range < 10) then
			random_range = 10
		end
	end

	resultImg.sprite = spriteAtlas:GetSprite(string.format("game_t1_%d", index))

	isInGame = false
	state1Toggle.enabled = true
	state2Toggle.enabled = true
	state3Toggle.enabled = true
end

local function onStartBtn()
    if isInGame then
        return
    end
    
    if isWin then
        LockScene()
        return
    end
    
    if 0 == selectIndex then
        tipText.text = "请选择石头、剪刀或布"
        return
    end
    
    local co = coroutine.create(calculateResult)
    coroutine.resume(co)
end

function game.awake(gameObject)
    pair = gameObject:GetComponent(typeof(UICtrlPair))
    
    state1Toggle = pair:GetCtrlComponent("State1", typeof(Toggle))
    state1Toggle.onValueChanged:AddListener(onState1Toggle)
    
    state2Toggle = pair:GetCtrlComponent("State2", typeof(Toggle))
    state2Toggle.onValueChanged:AddListener(onState2Toggle)
    
    state3Toggle = pair:GetCtrlComponent("State3", typeof(Toggle))
    state3Toggle.onValueChanged:AddListener(onState3Toggle)
    
    local btn = pair:GetCtrlComponent("AgainBtn", typeof(Button))
    btn.onClick:AddListener(onStartBtn)
    
    buttonText = pair:GetCtrlComponent("AgainBtnText", typeof(Text))
    tipText = pair:GetCtrlComponent("Tip", typeof(Text))
    
    resultAnimator =pair:GetCtrlComponent("Result", typeof(Animator))
	resultImg =pair:GetCtrlComponent("Result", typeof(Animator))
	spriteAtlas = gameObject:GetComponent(typeof(SpriteAtlas))

	tipText.text = "每晚睡觉前来一发。。。"
end