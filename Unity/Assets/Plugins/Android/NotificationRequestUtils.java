package com.unity3d.player;
import static android.content.Context.NOTIFICATION_SERVICE;

import android.app.AlertDialog;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.provider.Settings;
import android.support.v4.app.NotificationManagerCompat;
import android.util.Log;

public class NotificationRequestUtils {

    /**
     * 判断通知是否开启
     * @return true 开启
     * API19及以上可用
     */
    public static boolean checkNotificationsEnabled(){
        NotificationManagerCompat notification = NotificationManagerCompat.from(UnityPlayer.currentActivity);
        return notification.areNotificationsEnabled();
    }

    /**
     * 判断通知渠道是否开启(单个消息渠道)
     * @param channelID 渠道 id
     * @return true 开启
     * API26及以上可用
     */
    public static boolean checkNotificationsChannelEnabled(String channelID) {
        Log.i("Unity",Build.BOARD);
        NotificationManager manager = (NotificationManager) UnityPlayer.currentActivity.getSystemService(NOTIFICATION_SERVICE);
        if (manager == null) {
            return false;
        }
        NotificationChannel channel = null;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            channel = manager.getNotificationChannel(channelID);
        }
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            return !(channel.getImportance() == NotificationManager.IMPORTANCE_NONE);
        }
        return true;
    }

    // 弹窗引导用户打开通知权限 低版本的Android不支持运行时请求通知权限
    public static void openNotificationSetting()
    {

        // Android11之后启动时会自动请求权限
         if(Build.VERSION.SDK_INT > Build.VERSION_CODES.R)
            return;
        AlertDialog alertDialog = new AlertDialog.Builder(UnityPlayer.currentActivity)
                .setTitle("提示")
                .setMessage("检测到未打开通知\n否则会漏掉重要消息哦")
                .setNegativeButton("取消", (dialog, which) -> dialog.cancel())
                .setPositiveButton("去开启", (dialog, which) -> {
                    dialog.cancel();
                    Intent intent = new Intent(Settings.ACTION_APP_NOTIFICATION_SETTINGS);
                    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {// >= 8.0
                        intent.putExtra(Settings.EXTRA_APP_PACKAGE, UnityPlayer.currentActivity.getPackageName());
                    } else { // < 8.0 and >= 5.0
                        intent.putExtra("app_package", UnityPlayer.currentActivity.getPackageName());
                        intent.putExtra("app_uid", UnityPlayer.currentActivity.getApplicationInfo().uid);
                    }
                    UnityPlayer.currentActivity.startActivity(intent);
                }).create();
        alertDialog.show();
    }

    public static void test()
    {
        UnityPlayer.currentActivity.startActivity(new Intent(Settings.ACTION_APP_NOTIFICATION_BUBBLE_SETTINGS).
                putExtra(Settings.EXTRA_APP_PACKAGE, UnityPlayer.currentActivity.getPackageName()));
    }
}