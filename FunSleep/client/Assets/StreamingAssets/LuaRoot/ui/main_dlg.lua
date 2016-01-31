-- main_dlg.lua
-- Ö÷½çÃæÂß¼­

require("unity")

main_dlg = {}

print(GameObject)

local gameRoot = nil
local menuPanelAnimator = nil
local menuBtn = nil
local pair = nil

print(Resource)

local function on_menu()
    menuPanelAnimator:Play("menu_panel_appear")
    menuBtn.gameObject:SetActive(false)
end

local function on_back()
    menuPanelAnimator:Play("menu_panel_disappear")
    menuBtn.gameObject:SetActive(true)
end

local function create_game()
    local ob = Resources.Load("UIs/Game1")
    local game1 = GameObject.Instantiate(ob)
    game1.name = "Game"
    game1.transform:SetParent(gameRoot.transform, false)
end

function main_dlg.awake(gameObject)
    pair = gameObject:GetComponent(typeof(UICtrlPair))
    gameRoot = pair:GetCtrl("GameRoot")
    
    menuPanelAnimator = pair:GetCtrlComponent("MenuPanel", typeof(Animator))
    
    menuBtn = pair:GetCtrlComponent("MenuBtn", typeof(Button))
    menuBtn.onClick:AddListener(on_menu)
    
    local btn = pair:GetCtrlComponent("BackBtn", typeof(Button))
    btn.onClick:AddListener(on_back)
    
    create_game()
end

