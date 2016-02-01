// StartUp.cs
// Created by huangzw Jan/13/2016
// 游戏启动

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.IO;

public class StartUp : MonoBehaviour
{
    // private
    private GameObject mainCanvas = null;

    // awake
    void Awake()
    {
        InitLuaState();
    }

    // 初始化Lua环境
    private void InitLuaState()
    {
        LuaEnv.AddSearchPath(Path.Combine(Application.streamingAssetsPath, "LuaRoot"));
        LuaEnv.Init("main");
        LuaCoroutine.Register(LuaEnv.State, this);
    }
}
