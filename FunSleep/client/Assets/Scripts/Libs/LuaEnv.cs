using UnityEngine;
using System.Collections;
using LuaInterface;
using System.Collections.Generic;

public static class LuaEnv
{
    public static LuaState State;

    private static List<string> globalSearchPaths = new List<string>();

    public static void AddSearchPath(params string[] searchPaths)
    {
        for (int i = 0; i < searchPaths.Length; ++i)
        {
            LuaFileUtils.Instance.AddSearchPath(searchPaths[i]);
            globalSearchPaths.Add(searchPaths[i]);
        }
    }

    public static void Init(string mainScript)
    {
        if (string.IsNullOrEmpty(mainScript))
            throw new System.NullReferenceException("Can't found the main script.");

        State = new LuaState();

        // 增加搜索路径
        for (int i = 0; i < globalSearchPaths.Count; ++i)
            State.AddSearchPath(globalSearchPaths[i]);

        State.OpenLibs(LuaDLL.luaopen_pb);
        LuaBinder.Bind(State);
        State.Start();
        State.DoFile(mainScript);
    }
}
