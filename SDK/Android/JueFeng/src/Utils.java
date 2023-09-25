package com.lyh.t1.game;

import android.util.Log;
import android.widget.Toast;

import com.unity3d.player.*;

public class Utils {
    static String unityReceiveMessageOGN = "SDKMgr";
    static String tag = "UnityFromAndroid";

    public static void showToast(String r) {
        Toast.makeText( UnityPlayer.currentActivity, r, Toast.LENGTH_SHORT).show();
    }

    public static void Log(String s) {
        Log.i(tag, s);
    }
    public static void LogError(String s) {
        Log.e(tag, s);
    }
    // 发送消息给Unity
    public static void SendMessageToUnity(String method) {
        SendMessageToUnity(method, "");
    }
    public static void SendMessageToUnity(String method, String parameters) {
        Log("SendMessage: method:" + method + " parameters:" + parameters);
        UnityPlayer.UnitySendMessage(unityReceiveMessageOGN, method, parameters);
    }
}
