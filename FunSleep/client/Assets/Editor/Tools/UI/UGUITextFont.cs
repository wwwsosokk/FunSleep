using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Text;

public class UGUITextFont
{
    private const string FONT_PATH = "Assets/3Others/Fonts/DroidSansFallback-Chinese.ttf";

    [MenuItem("GameEditor/UGUI/ChangeFont")]
    private static void ChangeFont()
    {
        Font f = AssetDatabase.LoadAssetAtPath(FONT_PATH, typeof(Font)) as Font;
        if (null == f)
            Debug.LogError(string.Format("Can't found font at({0})", FONT_PATH));

        GameObject[] sels = Selection.gameObjects;
        for (int i = 0; i < sels.Length; ++i)
        {
            GameObject g = sels[i];
            ChangeFont(g, f);
            EditorUtility.SetDirty(g);
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private static void ChangeFont(GameObject g, Font f)
    {
        Text[] allText = g.GetComponentsInChildren<Text>(true);
        for (int  i = 0; i < allText.Length; ++i)
        {
            if (allText[i].font != f)
            {
                allText[i].font = f;
                Debug.Log("ChangeFont:" + GetFullHierarchyName(allText[i].transform));
            }
        }
    }

    private static string GetFullHierarchyName(Transform t)
    {
        StringBuilder sb = new StringBuilder();
        if (null != t.parent)
            sb.Append(GetFullHierarchyName(t.parent));

        sb.Append("/");
        sb.Append(t.name);

        return sb.ToString();
    }
}
