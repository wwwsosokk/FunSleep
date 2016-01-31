--main.lua
-- 游戏主函数

require "unity"

mainCanvas = nil

local function createMainCanvas()
    local canvas = Resources.Load("UIs/Canvas")
    if nil ~= canvas then
        mainCanvas = GameObject.Instantiate(canvas)
        mainCanvas.name = "MainCanvas"
    end
    
    local eventSystem = Resources.Load("UIs/EventSystem")
    if nil ~= eventSystem then
        local et = GameObject.Instantiate(eventSystem)
        et.name = "EventSystem"
    end
end

local function createMainDlg()
    local ob = Resources.Load("UIs/MainDlg")
    if nil ~= ob then
        local dlg = GameObject.Instantiate(ob)
        dlg.name = "MainDlg"
        local trans = dlg.transform
        trans:SetParent(mainCanvas.transform, false)
    end 
end

createMainCanvas()
createMainDlg()
