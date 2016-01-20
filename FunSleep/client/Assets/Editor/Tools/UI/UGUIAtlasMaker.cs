// UGUIAtlasMaker.cs
// Created by huangzw Oct/14/2014
// UGUI的图集制作

using UnityEngine;
using System.Collections;
using UnityEditor;

public class UGUIAtlasMaker
{
    [MenuItem("GameEditor/UGUI/Open/AltasMaker")]
    static void OpenUGUIAtlasMaker()
    {
        UGUIAtlasWnd wnd = EditorWindow.GetWindow<UGUIAtlasWnd>();
        if (null == wnd)
        {
            wnd.Show(true);
            wnd.Init();
        }
    }
}
