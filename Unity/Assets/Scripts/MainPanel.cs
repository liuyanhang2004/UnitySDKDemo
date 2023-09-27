using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    public GameObject qqSDK, jfSDK, testUpdatePanel;
    SDKTest SDKTest = new SDKTest();
    public void Awake()
    {
        // qqsdk目前仅有Android平台
#if !UNITY_EDITOR && UNITY_ANDROID && SDK && QQSDK 
        jfSDK.SetActive(false);
#elif !UNITY_EDITOR && SDK && JFSDK
        qqSDK.SetActive(false);
#else
        qqSDK.SetActive(false);
        jfSDK.SetActive(false);
#endif
    }

    public Button clearOutButton, methodButton, methodAndReturnButton;
    public Button simTypeButton, networkStatusButton;
    public Button qqLoginButton, qqScanQRLoginButton, qqLogoutButton;
    public Button jfLoginButton, jfPayButton, jfLogoutButton;
    public Button scheduleNotification;
    public InputField notificationContent;
    public Button testUpdateButton;


    public void Start()
    {
        // BUTTON INIT
        clearOutButton.onClick.AddListener(UILog.clear);
        methodButton.onClick.AddListener(InteractionTest.inst.CallCommonMethod);
        methodAndReturnButton.onClick.AddListener(InteractionTest.inst.CallMethodAndReturn);
        simTypeButton.onClick.AddListener(() => UILog.i(SDKMgr.inst.GetSIMStatus()));
        networkStatusButton.onClick.AddListener(() => UILog.i(SDKMgr.inst.GetNetworkTypeFromStatus()));
        scheduleNotification.onClick.AddListener(() =>
        {
            LocalNotificationTest.inst.pushLocalNotification(Application.productName, notificationContent.text, 1);
        });
        testUpdateButton.onClick.AddListener(() =>
        {
            UILog.i("update");
            testUpdatePanel.SetActive(true);
            var slider = testUpdatePanel.GetComponentInChildren<Slider>();
            var downloadInfo = testUpdatePanel.GetComponentInChildren<Text>();
            downloadInfo.text = null;
            slider.value = 0;
            DownloadWrap.downloadFile(null, null, false, this.GetCancellationTokenOnDestroy(),
                (progress, totalSize) =>
                {
                    downloadInfo.text = $"TestDownload.zip {Mathf.Floor(progress * totalSize)}M / {totalSize}M";
                    slider.value = progress;
                },
                () =>
                {
                    UILog.i("update completed");
                    testUpdatePanel.SetActive(false);
                }).Forget();
        });
#if !UNITY_EDITOR && SDK && QQSDK && UNITY_ANDROID
        qqSDKUI();
#elif !UNITY_EDITOR && SDK && JFSDK
        jfSDKUI();
#endif
    }

    void qqSDKUI()
    {
        qqLoginButton.onClick.AddListener(SDKTest.login);
        qqLogoutButton.onClick.AddListener(SDKTest.logout);
    }

    void jfSDKUI()
    {
        jfLoginButton.onClick.AddListener(SDKTest.login);
        jfLogoutButton.onClick.AddListener(SDKTest.logout);
        jfPayButton.onClick.AddListener(SDKTest.pay);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

public class SDKTest
{
    bool loginState;

    public void login()
    {
        if (loginState)
        {
            UILog.w("已登录");
            return;
        }
        SDKMgr.inst.Login((r, uid, token) =>
        {
            UILog.i("login end");
            UILog.i(r);
            UILog.i(uid);
            loginState = r == SDKMgr.LoginResult.LOGIN_SUCCESS;
        });
    }

    public void logout()
    {
        if (!loginState)
        {
            UILog.w("未登录");
            return;
        }
        SDKMgr.inst.Logout((r) =>
        {
            UILog.i("logout end");
            UILog.i(r);
            loginState = !(r == SDKMgr.LogoutResult.LOGOUT_SUCCESS);
        });
    }

    public void pay()
    {
        if (!loginState)
        {
            UILog.w("未登录");
            return;
        }
        PayTestParam p = new PayTestParam();
        p.roleId = "16000001000383";
        p.roleName = "沧桑湘君";
        p.roleLevel = "1";
        p.roleCTimeroleCTime = "1580105921";
        p.serverId = "160";
        p.serverName = "众神远征";
        // 应该从服务器获得(保证订单号唯一)
        p.orderNo = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds
            .ToString().Replace(".", "");
        p.money = 1;
        p.productId = "348";
        p.productName = "10000钻石";
        p.productDesc = "10000钻石";
        p.productUnit = "个";
        p.extendInfo = "标示";
        SDKMgr.inst.Pay((r, r2, r3) =>
        {
            UILog.i("pay end");
            UILog.i(r);
        }, p);
    }

    public struct PayTestParam
    {
        /// <summary>
        /// 角色游戏内唯一标示
        /// </summary>
        public string roleId;
        /// <summary>
        /// 角色名称
        /// </summary>
        public string roleName;
        /// <summary>
        /// 角色等级
        /// </summary>
        public string roleLevel;
        /// <summary>
        /// 角色创建时间,同一角色创建时间不可变(单 位：秒 )
        /// </summary>
        public string roleCTimeroleCTime;
        /// <summary>
        /// 区服ID  不可为null 不可为空串
        /// </summary>
        public string serverId;
        /// <summary>
        /// 区服名称
        /// </summary>
        public string serverName;
        /// <summary>
        /// 应用订单号，应用内必须唯一
        /// </summary>
        public string orderNo;
        /// <summary>
        /// 价格(分)
        /// </summary>
        public long money;
        /// <summary>
        /// 商品Id
        /// </summary>
        public string productId;
        /// <summary>
        /// 商品名称
        /// </summary>
        public string productName;
        /// <summary>
        /// 商品描述
        /// </summary>
        public string productDesc;
        /// <summary>
        /// 数量
        /// </summary>
        public int count;
        /// <summary>
        /// 商品单位
        /// </summary>
        public string productUnit;
        /// <summary>
        /// 额外信息 通常为其它信息个json数据方便服务器解析
        /// </summary>
        public string extendInfo;
    }
}

