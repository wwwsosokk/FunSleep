// DlgScript.cs
// Created by huangzw Jan/13/2016
// ui基类

using UnityEngine;
using System.Collections;

public class DlgScript : MonoBehaviour
{
	private UICtrlPair ctrlPair = null;

	protected UICtrlPair CtrlPair
	{
		get
		{
			if (null != ctrlPair)
				return ctrlPair;
			
			ctrlPair = transform.GetComponent<UICtrlPair>();
			return ctrlPair;
		}
	}
}
