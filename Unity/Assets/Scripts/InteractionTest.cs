using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class InteractionTest : MonoBehaviour
{
    public static InteractionTest inst { get; private set; }
    void Awake() => inst = this;

    public void CallCommonMethod()
    {
        commonMethod();
    }
#if !UNITY_EDITOR && UNITY_ANDROID && SDK && (JFSDK || QQSDK)
    void commonMethod() => SDKMgr.inst.callAndroid("commonMethod");
#elif !UNITY_EDITOR && UNITY_IOS && SDK && (JFSDK || QQSDK)
    [DllImport("__Internal")]
    static extern void commonMethod();
#else
    void commonMethod() => _commonMethod(default);
#endif

    void _commonMethod(string r)
    {
        UILog.i("commonMethod");
    }

    public void CallMethodAndReturn()
    {
        UILog.i($"Return：{SDKMgr.inst.PtrToString(methodAndReturn())}");
    }
#if !UNITY_EDITOR && UNITY_ANDROID && SDK && (JFSDK || QQSDK)
    string methodAndReturn() => SDKMgr.inst.callAndroid<string>("methodAndReturn");
#elif !UNITY_EDITOR && UNITY_IOS && SDK && (JFSDK || QQSDK)
    [DllImport("__Internal")]
    static extern IntPtr methodAndReturn();
#else
    string methodAndReturn() => "test methodAndReturn";
#endif
}