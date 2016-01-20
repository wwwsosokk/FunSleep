// UICtrlPairInspector.cs
// Created by huangzw Sep/29/2014
// UICtrlPair编辑界面

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(UICtrlPair))]
public class UICtrlPairInspector : Editor
{
    public override void OnInspectorGUI ()
    {
        UICtrlPair pair = target as UICtrlPair;

        UICtrlPair.CtrlPair[] ctrls = pair.Pairs();

        if (null != ctrls)
        {
            for (int i = 0; i < ctrls.Length; i++)
            {
                UICtrlPair.CtrlPair para = ctrls[i];

                EditorGUILayout.BeginHorizontal();

                // key
                EditorGUILayout.LabelField("Ctrl", GUILayout.Width(40f));
                string name = EditorGUILayout.TextField(para.name, GUILayout.Width(120f));
                if (name != para.name)
                {
                    pair.ReplacePair(para.name, name, para.ob);
                    EditorGUILayout.EndHorizontal();
                    break;
                }

                // object
                EditorGUILayout.LabelField("Ob", GUILayout.Width(40f));
                GameObject ob = (GameObject) EditorGUILayout.ObjectField(para.ob, typeof(GameObject), true, GUILayout.Width(120f));
                if (ob != para.ob)
                {
                    name = para.name.Equals("input ctrl name") ? ob.name : para.name;
                    pair.ReplacePair(para.name, name, ob);
                    EditorGUILayout.EndHorizontal();
                    break;
                }

                // delete
                if (GUILayout.Button("Delete", GUILayout.Width(60f)))
                {
                    pair.Remove(para.name);
                    EditorGUILayout.EndHorizontal();
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.BeginHorizontal();

        // add
        if (GUILayout.Button("Add Pair", GUILayout.Width(100f)))
        {
            pair.AddPair("input ctrl name", null);
        }

        EditorGUILayout.EndHorizontal();
    }
}
