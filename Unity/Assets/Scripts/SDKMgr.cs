using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using LitJson;

public partial class SDKMgr : MonoBehaviour
{
    AndroidJavaClass jc_;
    AndroidJavaObject jo_;
    static SDKMgr inst_;
    public static SDKMgr inst => inst_;
    public static SDKMgr CreateInstance()
    {
        if (inst_ != null)
            return inst_;
        var go = new GameObject("SDKMgr");
        Debug.LogWarning("SDKMGR CreateInstance");
        var inst = go.AddComponent<SDKMgr>();
        DontDestroyOnLoad(go);
        inst_ = inst;
        // test
        inst.gameObject.AddComponent<InteractionTest>();
        inst.InitSDK((InitSDKResult r) =>
        {
            Debug.Log("InitSDKResult " + r.ToString());
        });
        return inst;
    }

    public void OnDestroy()
    {
        jo_ = null;
        jc_ = null;
    }

    /// <summary>
    /// 打开URL
    /// </summary>
    /// <param name="url"></param>
    public virtual void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }

    /// <summary>
    ///  当前平台
    /// </summary>
    public static string platform
    {
        get
        {
#if UNITY_ANDROID
            return "android";
#elif UNITY_IOS
            return "apple";
#else
            return "others";
#endif
        }
    }

#if UNITY_IOS && SDK
    // 释放c指针内存
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void FreePtr(IntPtr i);
    /// <summary>
    /// PtrToString
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public string PtrToString(IntPtr i)
    {
        string s = System.Runtime.InteropServices.Marshal.PtrToStringAuto(i);
        FreePtr(i);
        return s;
    }
#endif    
    public string PtrToString(string v) => v;


    public virtual void Log(string s)
    {
        if (s == null)
            Debug.Log("SDK LOG Info:(null)");
        else
            Debug.Log("SDK LOG Info:" + s);
    }

    public void LogWarning(string s)
    {
        if (s == null)
            Debug.LogWarning("SDK LOG Info:(null)");
        else
            Debug.LogWarning("SDK LOG Info:" + s);
    }

    public void LogError(string s)
    {
        if (s == null)
            Debug.LogError("SDK LOG Info:(null)");
        else
            Debug.LogError("SDK LOG Info:" + s);
    }

    /// <summary>
    /// 自动登录
    /// </summary>
    // public void AutoLogin(Action<LoginResult, string, string> loginCb)
    // {
    //    loginCb_ = loginCb;
    //    loginCb_(LoginResult.LOGIN_FAIL, "", "");
    // }

    public Action<LoginResult, string, string> loginCb_ = delegate { };
    // private string loginLoinResult_, loginUid_, loginToken_;
    /// <summary>
    /// 登陆
    /// </summary>
    /// <param name="cb"></param>
    public void Login(Action<LoginResult, string, string> cb)
    {
        loginCb_ = cb;
        // loginLoinResult_ = null;
        // loginUid_ = null;
        // loginToken_ = null;
        login();
    }

#if UNITY_ANDROID && SDK
    private void login() => callAndroid("login");
#elif UNITY_IOS && SDK
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void login();
#else
    private void login() => _login($"{{\"code\" : {(int)LoginResult.LOGIN_SUCCESS}}}");
#endif

    void _login(string result)
    {
        Log($"Login Result:{result}");
        JsonData jsonData = JsonMapper.ToObject(result);
        //loginCb_((LoginResult)int.Parse(jsonData["code"].ToString()), "openid", "token");
        loginCb_((LoginResult)int.Parse(jsonData["code"].ToString()), jsonData["userName"].ToString(), "token");
    }

    public Action<PayResult, string, string> payCb_ = delegate { };
    /// <summary>
    /// 支付
    /// </summary>
    /// <param name="cb"></param>
    public void Pay(Action<PayResult, string, string> cb, SDKTest.PayTestParam payTestParam)
    {
        payCb_ = cb;
        pay(payTestParam.roleId,
            payTestParam.roleName,
            payTestParam.roleLevel,
            payTestParam.roleCTimeroleCTime,
            payTestParam.serverId,
            payTestParam.serverName,
            payTestParam.orderNo,
            payTestParam.money,
            payTestParam.productId,
            payTestParam.productName,
            payTestParam.productDesc,
            payTestParam.count,
            payTestParam.productUnit,
            payTestParam.extendInfo);
    }

#if UNITY_ANDROID && SDK
    private void pay(string roleId, string roleName, string roleLevel, string roleCTimeroleCTime, string serverId, string serverName, string orderNo, long money, string productId, string productName, string productDesc, int count, string productUnit, string extendInfo)
    => callAndroid("pay", new object[] { roleId, roleName, roleLevel, roleCTimeroleCTime, serverId, serverName, orderNo, money, productId, productName, productDesc, count, productUnit, extendInfo });
#elif UNITY_IOS && SDK
   [System.Runtime.InteropServices.DllImport("__Internal")]
   private static extern void pay(string roleId, string roleName, string roleLevel, string roleCTimeroleCTime, string serverId, string serverName, string orderNo, long money, string productId, string productName, string productDesc, int count, string productUnit, string extendInfo);
#else
    private void pay(string roleId, string roleName, string roleLevel, string roleCTimeroleCTime, string serverId, string serverName, string orderNo, long money, string productId, string productName, string productDesc, int count, string productUnit, string extendInfo) { _pay($"{{\"code\":{(int)PayResult.PAY_SUCCESS}}}"); }
#endif
    void _pay(string result)
    {
        Log($"Pay Result:{result}");
        JsonData jsonData = JsonMapper.ToObject(result);
        payCb_((PayResult)int.Parse(jsonData["code"].ToString()), "", "");
    }

    private Action<InitSDKResult> initSDKCb_;
    public void InitSDK(Action<InitSDKResult> cb)
    {
        initSDKCb_ = cb;
        initSDKCb_(InitSDKResult.INIT_SUCCESS);
    }

    public Action<LogoutResult> logoutCb_;
    /// <summary>
    /// 退出登录
    /// </summary>
    /// <param name="cb"></param>
    public void Logout(Action<LogoutResult> cb)
    {
        logoutCb_ = cb;
        logout();
    }
#if UNITY_ANDROID && SDK
    private void logout() => callAndroid("logout");
#elif UNITY_IOS && SDK
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void logout();
#else
    private void logout() => _logout($"{{\"code\" : {(int)LogoutResult.LOGOUT_SUCCESS}}}");
#endif
    void _logout(string result)
    {
        Log($"Logout Result:{result}");
        JsonData jsonData = JsonMapper.ToObject(result);
        if (logoutCb_ == null)
        {
            // todo
        }
        else
        {
            logoutCb_((LogoutResult)int.Parse(jsonData["code"].ToString()));
        }
        logoutCb_ = null;
    }

#if UNITY_ANDROID && SDK && JFSDK
    void _gameExit(string r)
    {
        Log(r);
        Application.Quit();
    }
    void _cancleExit(string r)
    {
        Log(r);
    }
    void _createdOrder(string r)
    {
        Log(r);
    }
#endif

    /// <summary>
    /// 获得当前网络状态
    /// </summary>
    /// <returns>2G/3G/4G/5G/WiFi/Unknown</returns>
    public string GetNetworkTypeFromStatus()
    {
        return PtrToString(getNetworkTypeFromStatus());
    }

#if !UNITY_EDITOR && UNITY_ANDROID && SDK
    private string getNetworkTypeFromStatus() => callAndroid<string>("getNetworkTypeFromStatus");
#elif !UNITY_EDITOR && UNITY_IOS && SDK
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern IntPtr getNetworkTypeFromStatus();
#else
    private string getNetworkTypeFromStatus() => "Unknown";
#endif

    /// <summary>
    /// 获得当前SIM卡类型
    /// </summary>
    /// <returns>中国移动/中国联通/中国电信/Unknown</returns>
    public string GetSIMStatus()
    {
        return PtrToString(getSIMStatus());
    }
#if !UNITY_EDITOR && UNITY_ANDROID && SDK
    private string getSIMStatus() => callAndroid<string>("getSIMStatus");
#elif !UNITY_EDITOR && UNITY_IOS && SDK
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern IntPtr getSIMStatus();
#else
    private string getSIMStatus() => "Unknown";
#endif
}

public partial class SDKMgr
{

    public enum InitSDKResult
    {
        INIT_SUCCESS = 1,   // 初始化成功 
        INIT_FAIL = 2,      //初始化失败 
    }

    public enum LoginResult
    {
        LOGIN_SUCCESS = 1,  // 登陆成功
        LOGIN_CANCEL = 2,   // 取消登录
        LOGIN_FAIL = 3,     // 登陆失败
    }

    public enum ChangeAccount
    {
        LOGIN_SUCCESS = 1,  // 登录成功，不管之前是什么登录状态，游戏内部都要切换成 新的
        LOGIN_FAI = 2,      // 登录失败，游戏内部之前如果是已经登录的，要清楚自己记录的 登录状态，设置成未登录。如果之前未登录，不用处理。 
        LOGIN_CANCEL = 3,   // 操作前后的登录状态没变化 
    }

    public enum PayResult
    {
        PAY_SUCCESS = 1,      //支付成功
        PAY_CANCEL = 2,       //订单支付取消
        PAY_FAIL = 3,          //订单支付失败
        PAY_SUBMIT_ORDER = 4, //订单已经提交，支付结果未知（比如：已经请求了，但是查询超时）
        PAYMENT_FAIL_REPEAT = 5, //已经购买过，无需重复购买
        PAYMENT_WAITING = 6, // 支付进行中
        REPLACEMENT_SUCCESS = 7, // 补单成功
        PAYMENT_FAIL_GET_ORDERNO = 8, // 从onesdk平台获取订单号失败
    }

    public enum LogoutResult
    {
        LOGOUT_SUCCESS = 1,    // 注销成功
        LOGOUT_FAIL = 2,       // 注销失败
        LOGOUT_CANCEL = 3, // 取消注销
    }
}

public partial class SDKMgr
{
    /// <summary>
    /// 调用AndroidSDK接口
    /// </summary>
    /// <param name="method"></param>
    /// <param name="objs"></param>
    public void callAndroid(string method, params object[] objs)
    {
        string str = "";
        for (int i = 0; i < objs.Length; i++)
            str += objs[i] + " , ";
        Log("call android :" + method + "   params:" + str);
        if (jc_ == null)
            jc_ = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        if (jo_ == null)
            jo_ = jc_.GetStatic<AndroidJavaObject>("currentActivity");
        if (objs.Length == 0)
            jo_.Call(method);
        else
            jo_.Call(method, objs);
    }

    public T callAndroid<T>(string method, params object[] objs)
    {
        string str = "";
        for (int i = 0; i < objs.Length; i++)
            str += objs[i] + " , ";
        Log("call android :" + method + "   params:" + str);
        if (jc_ == null)
            jc_ = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        if (jo_ == null)
            jo_ = jc_.GetStatic<AndroidJavaObject>("currentActivity");
        if (objs.Length == 0)
            return jo_.Call<T>(method);
        else
            return jo_.Call<T>(method, objs);
    }
}