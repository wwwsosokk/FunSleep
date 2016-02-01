// UICtrlPair.cs
// Created by huangzw Sep/29/2014
// 设置控件名与预置中的对象关联的脚本，与UIRootLeaf挂在一起

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class UICtrlPair : MonoBehaviour
{
    [System.Serializable]
    public class CtrlPair
    {
        public string name;
        public GameObject ob;

        public CtrlPair(string name, GameObject ob)
        {
            this.name = name;
            this.ob = ob;
        }
    }

    // 控件
    [SerializeField] private CtrlPair[] ctrls;

    private Dictionary<string, GameObject> ctrlDict;

    public void AddPair(string key, GameObject go)
    {
        if (null == ctrls)
            ctrls = new CtrlPair[0];

        // 要使用add,当有重复的key时需要给出提示
        if (null != FindCtrl(key))
        {
            Debug.LogWarning("have exist key : " + key);
            return;
        }

        CtrlPair[] newPair = new CtrlPair[ctrls.Length + 1];
        ctrls.CopyTo(newPair, 0);
        CtrlPair pair = new CtrlPair(key, go);
        newPair[ctrls.Length] = pair;
        ctrls = newPair;

        // 清除缓存数据
        ctrlDict = null;
    }

    // 替换名字
    public void ReplacePair(string oldName, string newName, GameObject ob)
    {
        CtrlPair ctrl = FindCtrl(oldName);
        if (null == ctrl)
            return;

        if (oldName == newName)
        {
            ctrl.ob = ob;
        } else
        {
            if (null != FindCtrl(newName))
            {
                Debug.LogWarning("have exist key : " + newName);
                return;
            }

            ctrl.name = newName;
            ctrl.ob = ob;
        }

        // 清除缓存数据
        ctrlDict = null;
    }

    public void Remove(string key)
    {
        if (null == ctrls || 0 == ctrls.Length)
            return;

        if (null == FindCtrl(key))
            return;

        CtrlPair[] newPair = new CtrlPair[ctrls.Length - 1];
        int n = 0;

        for (int i = 0; i < ctrls.Length; i++)
        {
            if (ctrls[i].name == key)
                continue;

            newPair[n++] = ctrls[i];
        }

        ctrls = newPair;

        // 清除缓存数据
        ctrlDict = null;
    }

    public void Clear()
    {
        ctrls = null;
        ctrlDict = null;
    }

    private CtrlPair FindCtrl(string key)
    {
        if (null == ctrls)
            return null;

        for (int i = 0; i < ctrls.Length; i++)
            if (ctrls[i].name == key)
                return ctrls[i];

        return null;
    }

    private bool DoInit()
    {
        if (null == ctrls)
            // 没有数据
            return false;

        do
        {
            if (null != ctrlDict)
                break;

            ctrlDict = new Dictionary<string, GameObject>();
            for (int i = 0; i < ctrls.Length; ++i)
            {
                ctrlDict[ctrls[i].name] = ctrls[i].ob;
            }
        } while (false);

        return true;
    }

    public GameObject GetCtrl(string key)
    {
        if (! DoInit())
            return null;

        GameObject go;
        ctrlDict.TryGetValue(key, out go);
        return go;
    }

    public UnityEngine.Object GetCtrlComponent(string key, System.Type type)
    {
        GameObject go = GetCtrl(key);
        if (null == go)
            return null;

        return go.GetComponent(type);
    }

    public UnityEngine.Object[] GetCtrlComponents(string key, System.Type type)
    {
        GameObject go = GetCtrl(key);
        if (null == go)
            return null;

        return go.GetComponents(type);
    }

    public T GetCtrlComponent<T>(string key) where T : Component
    {
        GameObject go = GetCtrl(key);
        if (null == go)
            return default(T);

        return go.GetComponent<T>();
    }

    public T[] GetCtrlComponents<T>(string key) where T : Component
    {
        GameObject go = GetCtrl(key);
        if (null == go)
            return null;

        return go.GetComponents<T>();
    }

    public CtrlPair[] Pairs()
    {
        return ctrls;
    }
}
