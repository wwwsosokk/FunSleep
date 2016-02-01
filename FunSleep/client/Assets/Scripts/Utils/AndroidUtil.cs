﻿using UnityEngine;
using System.Collections;

public static class AndroidUtil
{
    private static AndroidJavaObject Android;

    static AndroidUtil()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        Android = jc.GetStatic<AndroidJavaObject>("currentActivity");
    }

    public static void LockPhone()
    {
        Android.Call("doLock");
    }

    public static void InitWXApi(string appId)
    {
        Android.Call("initWXApi", appId);
    }

    public static void SendTextToWX(string text)
    {
        Android.Call("sendTextToWX", text);
    }
}
