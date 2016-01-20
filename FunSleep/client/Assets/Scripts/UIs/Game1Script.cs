// Game1Script.cs
// Created by huangzw Jan/13/2016
// 游戏1--石头剪刀布

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game1Script : DlgScript
{
    private bool isInGame = false;
    private int selectIndex = 0;

    private Toggle state1Toggle = null;
    private Toggle state2Toggle = null;
    private Toggle state3Toggle = null;

	private Text buttonText = null;
	private Text tipText = null;

	private Animator resultAnimator = null;
	private SpriteAtlas spriteAtlas = null;
	private Image resultImg = null;

	private int random_range = 20;
	private bool isWin = false;

    // awake
    void Awake()
    {
        state1Toggle = CtrlPair.GetCtrlComponent<Toggle>("State1");
        state1Toggle.onValueChanged.AddListener((isOn)=>OnState1Toggle(isOn));

        state2Toggle = CtrlPair.GetCtrlComponent<Toggle>("State2");
        state2Toggle.onValueChanged.AddListener((isOn)=>OnState2Toggle(isOn));

        state3Toggle = CtrlPair.GetCtrlComponent<Toggle>("State3");
        state3Toggle.onValueChanged.AddListener((isOn)=>OnState3Toggle(isOn));

        Button btn = CtrlPair.GetCtrlComponent<Button>("AgainBtn");
		btn.onClick.AddListener(OnStartBtn);

		buttonText = CtrlPair.GetCtrlComponent<Text>("AgainBtnText");
		tipText = CtrlPair.GetCtrlComponent<Text>("Tip");

		resultAnimator = CtrlPair.GetCtrlComponent<Animator>("Result");
		resultImg = CtrlPair.GetCtrlComponent<Image>("Result");
		spriteAtlas = GetComponent<SpriteAtlas>();

		tipText.text = "每晚睡觉前来一发。。。";
    }

    private void OnState1Toggle(bool isOn)
    {
        if (isInGame)
            return;

        if (isOn)
            selectIndex = 1;
    }

    private void OnState2Toggle(bool isOn)
    {
        if (isInGame)
            return;

        if (isOn)
            selectIndex = 2;
    }

    private void OnState3Toggle(bool isOn)
    {
        if (isInGame)
            return;

        if (isOn)
            selectIndex = 3;
    }

    private void OnStartBtn()
    {
        if (isInGame)
            return;

		if (isWin)
		{
			LockScreen();
			return;
		}

		if (0 == selectIndex)
		{
			tipText.text = "请选择石头、剪刀或布";
			return;
		}

		StartCoroutine(CalculateResult());
    }

	private IEnumerator CalculateResult()
	{
		isInGame = true;
		state1Toggle.enabled = false;
		state2Toggle.enabled = false;
		state3Toggle.enabled = false;

		resultAnimator.enabled = true;
		resultAnimator.Play("result");
		tipText.text = "。。。";

		yield return new WaitForSeconds(2f);

		buttonText.text = "再来一次";
		resultAnimator.StopPlayback();
		resultAnimator.enabled = false;

		int random = Random.Range(0, random_range);
		int index = 1;

		if (random < 3)
		{
			// win
			tipText.text = "哇，今天运气不赖嘛，安心入睡吧。";
			buttonText.text = "点击锁屏";
			isWin = true;

			if (1 == selectIndex)
				index = 2;
			else
			if (2 == selectIndex)
				index = 3;
			else
			if (3 == selectIndex)
				index = 1;

			random_range += 5;
			if (random_range > 20)
				random_range = 20;
		} else
		if (random >= 3 && random < 5)
		{
			// same
			tipText.text = "平局啊！";
			index = selectIndex;
		} else
		{
			// lose
			tipText.text = "手气真挫！";

			if (1 == selectIndex)
				index = 3;
			else
			if (2 == selectIndex)
				index = 1;
			else
			if (3 == selectIndex)
				index = 2;

			random_range -= 5;
			if (random_range < 10)
				random_range = 10;
		}

		resultImg.sprite = spriteAtlas.GetSprite("game_t1_" + index);

		isInGame = false;
		state1Toggle.enabled = true;
		state2Toggle.enabled = true;
		state3Toggle.enabled = true;
	}

	private void LockScreen()
	{
        Debug.Log("开始锁屏");
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidUtil.LockPhone();
#endif
	}
}


