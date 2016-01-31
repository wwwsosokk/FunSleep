using UnityEngine;
using System.Collections;
using LuaInterface;
using System;
using System.Collections.Generic;

public class LuaBindBhr : MonoBehaviour
{
    #region 全局LuaState
    protected static LuaState state
    {
        get
        {
            return LuaEnv.State;
        }
    }
    #endregion

    // 函数偏移
    public enum FuncOffset { awake = 0, start, update, lateUpdate, fixedUpdate, onEnable, onDisable, onDestroy, MaxSize }

    public string luaScript;
    public FuncOffset[] luaEvents;

    protected Dictionary<FuncOffset, LuaFunction> luaFunctions;

    protected virtual void Awake()
    {
        LoadLuaFunction();
        RunLuaMethod(FuncOffset.awake);
    }

    // Use this for initialization
    protected virtual void Start()
    {
        RunLuaMethod(FuncOffset.start);
	}
	
	// Update is called once per frame
	protected virtual void Update()
    {
        RunLuaMethod(FuncOffset.update);

        // state.Collect();

#if UNITY_EDITOR
        state.CheckTop();
#endif
    }

    protected virtual void FixedUpdate()
    {
        RunLuaMethod(FuncOffset.fixedUpdate);
    }

    protected virtual void LateUpdate()
    {
        RunLuaMethod(FuncOffset.lateUpdate);
    }

    protected virtual void OnEnable()
    {
        RunLuaMethod(FuncOffset.onEnable);
    }

    protected virtual void OnDisable()
    {
        RunLuaMethod(FuncOffset.onDisable);
    }

    protected virtual void OnDestroy()
    {
        RunLuaMethod(FuncOffset.onDestroy);

        UnloadLuaFunction();
    }

    private void LoadLuaFunction()
    {
        if (string.IsNullOrEmpty(luaScript)
            || null == luaEvents
            || luaEvents.Length <= 0)
            return;

        state.Require(luaScript);

        string sptName;
        int index = luaScript.LastIndexOf("/");
        if (-1 != index && index < luaScript.Length - 1)
            sptName = luaScript.Substring(index + 1);
        else
            sptName = luaScript;

        luaFunctions = new Dictionary<FuncOffset, LuaFunction>();
        for (int i = 0; i < luaEvents.Length; ++i)
        {
            if (luaFunctions.ContainsKey(luaEvents[i]))
            {
                Debug.LogWarning(string.Format("{0} has exist!", luaEvents[i].ToString()));
                continue;
            }

            luaFunctions.Add(luaEvents[i], state.GetFunction(string.Format("{0}.{1}", sptName, luaEvents[i].ToString())));
        }
    }

    private void UnloadLuaFunction()
    {
        if (null == luaFunctions || luaFunctions.Count <= 0)
            return;

        LuaFunction func = null;
        foreach(KeyValuePair<FuncOffset, LuaFunction> kvp in luaFunctions)
        {
            func = kvp.Value;
            if (null == func)
                continue;

            func.Dispose();
        }

        luaFunctions = null;
    }

    protected void RunLuaMethod(FuncOffset offset, params object[] args)
    {
        if (null == luaFunctions)
            return;

        LuaFunction func;
        luaFunctions.TryGetValue(offset, out func);
        if (null != func)
        {
            func.BeginPCall();
            func.Push(gameObject);

            if (null != args && args.Length > 0)
                func.PushArgs(args);

            func.PCall();
            func.EndPCall();
        }
    }
}
