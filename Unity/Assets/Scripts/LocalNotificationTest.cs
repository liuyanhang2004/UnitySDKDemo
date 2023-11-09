using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
#if UNITY_IPHONE
using Unity.Notifications.iOS;
#elif UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class LocalNotificationTest : MonoBehaviour
{
    public static LocalNotificationTest inst;
#if UNITY_EDITOR
#elif UNITY_ANDROID
    AndroidJavaClass A_NotificationUtils = new AndroidJavaClass($"{PlayerSettings.applicationIdentifier}.game.NotificationRequestUtils");
#endif
    void Awake()
    {
        inst = this;
#if UNITY_EDITOR
#elif UNITY_ANDROID
        registerNotificationChannel();
        // 低版本Android需引导打开通知设置
        if (PlayerPrefs.GetString("NotificationsState") == string.Empty)
        {
            PlayerPrefs.SetString("NotificationsState", "true");
            if (!requestAuthorization())
            {
                A_NotificationUtils.CallStatic("openNotificationSetting");
            }
        }
#endif
        cleanNotification();
    }

#if !UNITY_EDITOR
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            return;
        cleanNotification();
    }
#endif

    public static void cleanNotification()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.ApplicationBadge = 0;
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
#endif
    }

    public void pushLocalNotification(string title, string body, int delaySecond)
    {
#if UNITY_ANDROID
        androidScheduleNotification(title, body, delaySecond);
#elif UNITY_IOS
        iosScheduleNotification(title, body, delaySecond);
#else
        Debug.Log("no match");
#endif
    }

#if UNITY_ANDROID
    void registerNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "test_notification",
            Name = "TestNotification",
            Importance = Importance.High,
            Description = "Generic notification",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public bool requestAuthorization()
    {
#if UNITY_EDITOR
        return true;
#else
        return A_NotificationUtils.CallStatic<bool>("checkNotificationsEnabled");
#endif

    }

    public void androidScheduleNotification(string title, string body, int delaySecond)
    {
        if (!requestAuthorization())
        {
            UILog.w("没有通知权限无法推送消息");
            return;
        }
        var notification = new AndroidNotification()
        {
            Title = title,
            Text = System.Text.RegularExpressions.Regex.Unescape(body),
            FireTime = DateTime.Now.AddSeconds(delaySecond),
            Number = 1,
            SmallIcon = "icon_small",
            LargeIcon = "icon_large",
        };
        int notificationId = AndroidNotificationCenter.SendNotification(notification, "test_notification");
        UILog.i(string.Format("ScheduleNotification\nTitle: {0}\nBody:\n{1}\nFireTime: {2}",
              notification.Title, notification.Text, dateTimeToStr(notification.FireTime)));
    }
#endif

#if UNITY_IPHONE

    public IEnumerator requestAuthorization()
    {
#if UNITY_EDITOR
        yield return true;
#endif
        var authorizationOption = AuthorizationOption.Alert;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            }
            string res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            Debug.Log(res);
            yield return req.Granted;
        }
    }

    public IEnumerator iosScheduleNotification(string title, string body, int delaySecond)
    {
        var _requestAuthorization = requestAuthorization();
        yield return _requestAuthorization;
        if (!(bool)_requestAuthorization.Current)
        {
            UILog.w("没有通知权限无法推送消息");
            yield break;
        }
        // 时间触发 在指定时间过后触发通知
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(0, 0, delaySecond),
            Repeats = false
        };
        // 日历触发 如果未指定year将在下一个指定的时间触发通知
        //var calendarTrigger = new iOSNotificationCalendarTrigger()
        //{
        //    // Year = 2020,
        //    // Month = 6,
        //    //Day = 1,
        //    Hour = 12,
        //    Minute = 0,
        //    // Second = 0
        //    Repeats = false
        //};
        // todo 位置触发 进入或离开某地理位置触发通知

        var notification = new iOSNotification()
        {
            // unique identifier
            Identifier = $"Tag{(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds}",
            Title = title,
            Body = $"{System.Text.RegularExpressions.Regex.Unescape(body)}",
            // 设备在前台时显示通知同时需要指定ForegroundPresentationOption
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
            // 显示在icon右上角的角标以收到最新消息的Badge为准
            // 应该根据历史消息计算为动态的，游戏一般要求没有这么高不管多少消息都显示为1
            Badge = 1,
        };
        UILog.i(string.Format("ScheduleNotification\nTitle: {0}\nBody:\n{1}",
              notification.Title, notification.Body));
        iOSNotificationCenter.ScheduleNotification(notification);
    }
#endif

    void removeScheduledNotification(string notificationId)
    {
#if UNITY_IPHONE
        iOSNotificationCenter.RemoveScheduledNotification(notificationId);
#elif UNITY_ANDROID
        AndroidNotificationCenter.CancelScheduledNotification(int.Parse(notificationId));
#endif
    }

    void removeAllScheduledNotification()
    {
#if UNITY_IPHONE
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#elif UNITY_ANDROID
        AndroidNotificationCenter.CancelAllScheduledNotifications();
#endif
    }

    string dateTimeToStr(DateTime dateTime)
    {
        return $"{ dateTime.ToShortDateString() } { dateTime.ToLongTimeString() }";
    }
}
