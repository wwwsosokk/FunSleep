// StartUp.cs
// Created by huangzw Jan/13/2016
// 游戏启动

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class StartUp : MonoBehaviour
{
    // private
    private GameObject mainCanvas = null;

	// awake
	void Awake()
	{
	}

	// enable
	void OnEnable()
	{
		CreateMainCanvas();
        CreateMainDlg();
	}

	// 创建主canvas
	private void CreateMainCanvas()
	{
        Object canvas = Resources.Load("UIs/Canvas");
        if (null != canvas)
        {
            mainCanvas = GameObject.Instantiate(canvas) as GameObject;
            mainCanvas.name = "MainCanvas";
        }

		Object eventSystem = Resources.Load("UIs/EventSystem");
		if (null != eventSystem)
		{
			GameObject et = GameObject.Instantiate(eventSystem) as GameObject;
			et.name = "EventSystem";
		}
	}

	// 创建主界面
	private void CreateMainDlg()
	{
		Object ob = Resources.Load("UIs/MainDlg");
		if (null != ob)
        {
            GameObject dlg = GameObject.Instantiate(ob) as GameObject;
            dlg.name = "MainDlg";
            Transform trans = dlg.transform;
            trans.SetParent(mainCanvas.transform, false);
        }
	}
}
