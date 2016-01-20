// MainDlgScript.cs
// Created by huangzw Jan/13/2016
// 主界面

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainDlgScript : DlgScript
{
    private GameObject gameRoot = null;
	private Animator menuPanelAnimator = null;
	private Button menuBtn = null;

	// awake
	void Awake()
	{
        gameRoot = CtrlPair.GetCtrl("GameRoot");

		menuPanelAnimator = CtrlPair.GetCtrlComponent<Animator>("MenuPanel");

		// event
		menuBtn = CtrlPair.GetCtrlComponent<Button>("MenuBtn");
		menuBtn.onClick.AddListener(OnMeunBtn);

		Button btn = CtrlPair.GetCtrlComponent<Button>("BackBtn");
		btn.onClick.AddListener(OnBackBtn);

		// add type 1 game
		CreateGame();
	}

	// enable
	void OnEnable()
	{
	}

	private void OnMeunBtn()
	{
		menuPanelAnimator.Play("menu_panel_appear");
		menuBtn.gameObject.SetActive(false);
	}

	private void OnBackBtn()
	{
		menuPanelAnimator.Play("menu_panel_disappear");
		menuBtn.gameObject.SetActive(true);
	}

    private void CreateGame()
    {
        // type1
        Object ob = Resources.Load("UIs/Game1");
        GameObject game1 = GameObject.Instantiate(ob) as GameObject;
        game1.name = "Game1";
        game1.transform.SetParent(gameRoot.transform, false);
    }
}
