// GENERATED BY UNITY. REMOVE THIS COMMENT TO PREVENT OVERWRITING WHEN EXPORTING AGAIN
package com.lyh.t1.game;

import android.Manifest;
import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.res.Configuration;
import android.graphics.PixelFormat;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.provider.Settings;
import android.telephony.TelephonyManager;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.os.Process;

import android.util.Log;
import android.content.Context;
import android.provider.MediaStore;

import java.io.File;
import java.sql.Time;

import android.net.Uri;
import android.widget.Toast;

import com.juefeng.sdk.juefengsdk.services.bean.JfOrderInfo;
import com.unity3d.player.*;
import com.juefeng.sdk.juefengsdk.JFSDK;
import com.juefeng.sdk.juefengsdk.interf.SDKEventListener;
import com.juefeng.sdk.juefengsdk.services.bean.CreatOrderInfo;
import com.juefeng.sdk.juefengsdk.services.bean.LoginErrorMsg;
import com.juefeng.sdk.juefengsdk.services.bean.LogincallBack;
import com.juefeng.sdk.juefengsdk.services.bean.PayFaildInfo;
import com.juefeng.sdk.juefengsdk.services.bean.PaySuccessInfo;

public class UnityPlayerActivity extends Activity {

    Context mContext = null;
    private static final int RC_CustomHeadIcon_TakePhoto = 30001;
    private static final int RC_CustomHeadIcon_CropPhoto = 30002;
    private static final int RC_CustomHeadIcon_PickPhoto = 30003;
    private final String PhotoName_Raw = "photo_raw.jpg";
    private final String PhotoName_Croped = "photo_croped.png";

    protected UnityPlayer mUnityPlayer; // don't change the name of this variable; referenced from native code

    // Override this in your custom UnityPlayerActivity to tweak the command line arguments passed to the Unity Android Player
    // The command line arguments are passed as a string, separated by spaces
    // UnityPlayerActivity calls this from 'onCreate'
    // Supported: -force-gles20, -force-gles30, -force-gles31, -force-gles31aep, -force-gles32, -force-gles, -force-vulkan
    // See https://docs.unity3d.com/Manual/CommandLineArguments.html
    // @param cmdLine the current command line arguments, may be null
    // @return the modified command line string or null
    protected String updateUnityCommandLineArguments(String cmdLine) {
        return cmdLine;
    }

    // Setup activity layout
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        super.onCreate(savedInstanceState);

        String cmdLine = updateUnityCommandLineArguments(getIntent().getStringExtra("unity"));
        getIntent().putExtra("unity", cmdLine);

        mUnityPlayer = new UnityPlayer(this);
        setContentView(mUnityPlayer);
        mUnityPlayer.requestFocus();

        SDKEventListener mSDKEvenetListener = new SDKEventListener() {
            @Override
            public void onLoginSuccess(LogincallBack logincallBack) {
                String msg = String.format("{" +
                                "\"code\":\"%d\"," +
                                "\"birthday\":\"%s\"," +
                                "\"userId\":\"%s\"," +
                                "\"userName\":\"%s\"," +
                                "\"token\":\"%s\"," +
                                "\"isAuthenticated\":%b," +
                                "\"isAdult\":%b" +
                                "}",
                        1,
                        logincallBack.birthday,
                        logincallBack.userId,
                        logincallBack.userName,
                        logincallBack.token,
                        logincallBack.isAuthenticated,
                        logincallBack.isAdult);
                JFSDK.getInstance().showFloatView(UnityPlayerActivity.this);
                Utils.Log("LoginSuccess msg: " + msg);
                Utils.SendMessageToUnity("_login", msg);
            }

            @Override
            public void onLoginFailed(LoginErrorMsg loginErrorMsg) {
                String msg = String.format("{" +
                                "\"code\":\"%s\"," +
                                "\"errorMsg\":\"%s\"" +
                                "}",
                        loginErrorMsg.getCode(),
                        loginErrorMsg.getErrorMsg());
                Utils.Log("LoginFailed msg: " + msg);
                Utils.SendMessageToUnity("_login", msg);
            }

            @Override
            public void onInitSuccess(String desc, boolean isAutoLogin) {
                String msg = String.format("{" +
                                "\"code\":\"%d\"," +
                                "\"errorMsg\":\"%s\"," +
                                "\"isAutoLogin\":%b" +
                                "}",
                        1,
                        desc,
                        isAutoLogin);
                Utils.Log("InitSuccess msg:" + msg);
                // Utils.SendMessageToUnity("_initSDK",msg);
            }

            @Override
            public void onInitFaild(String desc) {
                String msg = String.format("{" +
                                "\"code\":\"%d\"," +
                                "\"desc\":\"%s\"" +
                                "}",
                        2,
                        desc);
                Utils.Log("InitFaild mag:" + msg);
                // Utils.SendMessageToUnity("_initSDK",msg);
            }

            @Override
            public void onPaySuccessCallback(PaySuccessInfo paySuccessInfo) {
                String msg = String.format("{" +
                                "\"code\":\"%d\"," +
                                "\"orderId\":\"%s\"," +
                                "\"gameRole\":\"%s\"," +
                                "\"gameArea\":\"%s\"," +
                                "\"productName\":\"%s\"," +
                                "\"productDesc\":\"%s\"," +
                                "\"remark\":\"%s\"" +
                                "}",
                        1,
                        paySuccessInfo.orderId,
                        paySuccessInfo.gameRole,
                        paySuccessInfo.gameArea,
                        paySuccessInfo.productName,
                        paySuccessInfo.productDesc,
                        paySuccessInfo.remark);
                Utils.Log("PaySuccess mag: " + msg);
                Utils.SendMessageToUnity("_pay", msg);
            }

            @Override
            public void onPayFaildCallback(PayFaildInfo payFaildInfo) {
                String msg = String.format("{" +
                                "\"code\":\"%s\"," +
                                "\"msg\":\"%s\"" +
                                "}",
                        // payFaildInfo.getCode(),
                        2,
                        payFaildInfo.getMsg());
                Utils.Log("PayFaild mag: " + msg);
                Utils.SendMessageToUnity("_pay", msg);
            }

            @Override
            public void onExit(String s) {
                Utils.Log("onExit: " + s);
                Utils.SendMessageToUnity("_gameExit", s);
            }

            @Override
            public void onCancleExit(String s) {
                Utils.Log("onCancleExit: " + s);
                Utils.SendMessageToUnity("_cancleExit", s);
            }

            @Override
            public void onCreatedOrder(CreatOrderInfo creatOrderInfo) {
                String msg = String.format("{" +
                                "\"orderId\":\"%s\"," +
                                "\"gameRole\":\"%s\"," +
                                "\"gameArea\":\"%s\"," +
                                "\"productName\":\"%s\"," +
                                "\"productDesc\":\"%s\"," +
                                "\"remark\":\"%s\"" +
                                "}",
                        creatOrderInfo.orderId,
                        creatOrderInfo.gameRole,
                        creatOrderInfo.gameArea,
                        creatOrderInfo.productName,
                        creatOrderInfo.productDesc,
                        creatOrderInfo.remark);
                Utils.Log("CreatedOrder mag: " + msg);
                Utils.SendMessageToUnity("_createdOrder", msg);
            }

            @Override
            public void onLogoutLogin() {
                Utils.Log("onLogoutLogin :" + "{\"code\" : 1}");
                Utils.SendMessageToUnity("_logout", "{\"code\" : 1}");
            }

            @Override
            public void onSwitchAccountSuccess(LogincallBack logincallBack) {
                String msg = String.format("{" +
                                "\"birthday\":\"%s\"," +
                                "\"userId\":\"%s\"," +
                                "\"userName\":\"%s\"," +
                                "\"token\":\"%s\"," +
                                "\"isAuthenticated\":%b," +
                                "\"isAdult\":%b" +
                                "}",
                        logincallBack.birthday,
                        logincallBack.userId,
                        logincallBack.userName,
                        logincallBack.token,
                        logincallBack.isAuthenticated,
                        logincallBack.isAdult);
                Utils.Log("SwitchAccount msg:" + msg);
                Utils.SendMessageToUnity("_switchAccountSuccess", msg);
            }

            @Override
            public void onGameSwitchAccount() {
                Utils.Log("onGameSwitchAccount");
                Utils.SendMessageToUnity("gameSwitchAccount");
            }
        };
        JFSDK.getInstance().init(this, mSDKEvenetListener);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        // To support deep linking, we need to make sure that the client can get access to
        // the last sent intent. The clients access this through a JNI api that allows them
        // to get the intent set on launch. To update that after launch we have to manually
        // replace the intent with the one caught here.
        setIntent(intent);
    }

    // Quit Unity
    @Override
    protected void onDestroy() {
        mUnityPlayer.destroy();
        super.onDestroy();
    }

    // If the activity is in multi window mode or resizing the activity is allowed we will use
    // onStart/onStop (the visibility callbacks) to determine when to pause/resume.
    // Otherwise it will be done in onPause/onResume as Unity has done historically to preserve
    // existing behavior.
    @Override
    protected void onStop() {
        super.onStop();
        JFSDK.getInstance().onStop(UnityPlayerActivity.this);
        mUnityPlayer.pause();
    }

    @Override
    protected void onStart() {
        super.onStart();
        JFSDK.getInstance().onStart(UnityPlayerActivity.this);
        mUnityPlayer.resume();
    }

    // Pause Unity
    @Override
    protected void onPause() {
        super.onPause();
        JFSDK.getInstance().onPause(UnityPlayerActivity.this);
        mUnityPlayer.pause();
    }

    // Resume Unity
    @Override
    protected void onResume() {
        super.onResume();
        JFSDK.getInstance().onResume(UnityPlayerActivity.this);
        mUnityPlayer.resume();
    }

    // Low Memory Unity
    @Override
    public void onLowMemory() {
        super.onLowMemory();

        mUnityPlayer.lowMemory();
    }

    // Trim Memory Unity
    @Override
    public void onTrimMemory(int level) {
        super.onTrimMemory(level);
        if (level == TRIM_MEMORY_RUNNING_CRITICAL) {
            mUnityPlayer.lowMemory();
        }
    }

    // This ensures the layout will be correct.
    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        mUnityPlayer.configurationChanged(newConfig);
    }

    // Notify Unity of the focus change.
    @Override
    public void onWindowFocusChanged(boolean hasFocus) {
        super.onWindowFocusChanged(hasFocus);
        mUnityPlayer.windowFocusChanged(hasFocus);
    }

    // For some reason the multiple keyevent type is not supported by the ndk.
    // Force event injection by overriding dispatchKeyEvent().
    @Override
    public boolean dispatchKeyEvent(KeyEvent event) {
        if (event.getAction() == KeyEvent.ACTION_MULTIPLE)
            return mUnityPlayer.injectEvent(event);

        if (event.getKeyCode() == KeyEvent.KEYCODE_BACK) {
            if (event.getAction() == KeyEvent.ACTION_DOWN && event.getRepeatCount() == 0) {
                JFSDK.getInstance().exitLogin(UnityPlayerActivity.this);
                return true;
            }
        }
        return super.dispatchKeyEvent(event);
    }

    // Pass any events not handled by (unfocused) views straight to UnityPlayer
    @Override
    public boolean onKeyUp(int keyCode, KeyEvent event) {
        return mUnityPlayer.injectEvent(event);
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        return mUnityPlayer.injectEvent(event);
    }

    @Override
    public boolean onTouchEvent(MotionEvent event) {
        return mUnityPlayer.injectEvent(event);
    }

    /*API12*/
    public boolean onGenericMotionEvent(MotionEvent event) {
        return mUnityPlayer.injectEvent(event);
    }

    private static void log(String tag, String msg) {
        Log.i(tag, msg);
    }

    private static void SendMessageToUnity(String method, String parameters) {
        log("setSDKListener:", "method:" + method + "  parameters:" + parameters);
        if (parameters == null)
            parameters = "";
        UnityPlayer.UnitySendMessage("SDKSystemMessage", method, parameters);
    }

    // 拍摄图片
    public void takePhoto() {
        Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
        File imageFile = new File(mContext.getExternalFilesDir(null), PhotoName_Raw);
        //Uri fileUri = FileProvider7.getUriForFile(mContext, imageFile);
        //intent.putExtra(MediaStore.EXTRA_OUTPUT, fileUri);
        if (intent.resolveActivity(getPackageManager()) == null) {
            Log.i("Army_for_Korea", "TakePhoto_AppNotFound_Camera");
            Utils.SendMessageToUnity("TakePhoto_AppNotFound_Camera"); // 找不到拍摄程�??.
            return;
        }
        startActivityForResult(intent, RC_CustomHeadIcon_TakePhoto);
    }

    public void pickPhoto() {
        Intent intent = new Intent(Intent.ACTION_PICK, null);
        intent.setDataAndType(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, "image/*");
        if (intent.resolveActivity(getPackageManager()) == null) {
            Log.i("Army_for_Korea", "TakePhoto_AppNotFound_Camera");
            Utils.SendMessageToUnity("TakePhoto_AppNotFound_Pick");
            return;
        }
        startActivityForResult(intent, RC_CustomHeadIcon_PickPhoto);
    }

    private void startCrop(Uri uri) {
        Intent intent = new Intent("com.android.camera.action.CROP");
        //if (uri == null)
        //FileProvider7.setIntentDataAndType(mContext, intent, "image/*", new File(mContext.getExternalFilesDir(null), PhotoName_Raw), true);
        //else
        intent.setDataAndType(uri, "image/*");
        intent.putExtra("crop", "true");
        intent.putExtra("aspectX", 1);
        intent.putExtra("aspectY", 1);
        intent.putExtra("outputX", 128);
        intent.putExtra("outputY", 128);
        intent.putExtra("return-data", false);
        File imageFile = new File(mContext.getExternalFilesDir(null), PhotoName_Croped);
        intent.putExtra(MediaStore.EXTRA_OUTPUT, Uri.fromFile(imageFile));
        if (intent.resolveActivity(getPackageManager()) == null)
            Utils.SendMessageToUnity("TakePhoto_AppNotFound_Crop");
        else
            startActivityForResult(intent, RC_CustomHeadIcon_CropPhoto);
    }

    public void login() {
        JFSDK.getInstance().doLogin(UnityPlayerActivity.this);
    }
    public void logout() {
        JFSDK.getInstance().logoutLogin(UnityPlayerActivity.this);
    }

    public String getDirveInfo() {
        return "";
    } //先临时返回一下防止报错

    public void showFloatView() {
    }

    public void closeFloatView() {
    }

    public void createWebView(int left, int top, int right, int bottom, String url) {
    }

    public void destroyWebView() {
    }

    public String getSdkChannel() {
        return "";
    }

    public void gameExit() {
    }

    public void pay(String roleId, String roleName, String roleLevel,
                    String roleCTimeroleCTime, String serverId, String serverName,
                    String orderNo, long money, String productId, String productName,
                    String productDesc, int count, String productUnit, String extendInfo) {
        // https://blog.csdn.net/rzleilei/article/details/122700219
        if (Build.VERSION.SDK_INT >= 30 && !Environment.isExternalStorageManager()) {
            Toast.makeText(this, "请打开文件访问权限！", Toast.LENGTH_LONG).show();
            Intent intent = new Intent(Settings.ACTION_MANAGE_ALL_FILES_ACCESS_PERMISSION);
            startActivity(intent);
            return;
        }
        JfOrderInfo info = new JfOrderInfo();
        info.setLevel(roleLevel);
        info.setCpOrderId(orderNo);
        info.setRoleName(roleName);
        info.setRoleId(roleId);
        info.setServerName(serverName);
        info.setServerId(serverId);
        double price = money / 100.0;
        info.setPrice(String.valueOf(price));
        info.setGoodsName(productName);
        info.setGoodsDes(productDesc);
        info.setGoodsId(productId);
        info.setRemark(extendInfo);
        //orderInfo.setCpOrderId(String.valueOf(System.currentTimeMillis()));
        //orderInfo.setGoodsDes("10000钻石");
        //orderInfo.setGoodsName("10000钻石");
        //orderInfo.setGoodsId("348");
        //orderInfo.setLevel("1");
        //orderInfo.setPrice(String.valueOf(0.01));
        //orderInfo.setRemark("标示");
        //orderInfo.setRoleId("16000001000383");
        //orderInfo.setRoleName("沧桑湘君");
        //orderInfo.setServerId("160");
        //orderInfo.setServerName("众神远征");
        //orderInfo.setVip("0");
        JFSDK.getInstance().showPay(this, info);
    }

    public void extendInfoSubmit(String roleId, String roleName,
                                 String roleLevel, String roleCTimeroleCTime, String serverId,
                                 String serverName, String type) {
    }

    public void checkUnfinishOrders() {}
    public void finishOrders(String extInfo) {}
    public void getAnnouncementInfo() {}
    public void FBShare(int type, String path, String title) {}
    public void FBInvite(String linkUrl, String imgUrl) {}
    public void IsInstallPak(String packageName) {}
    public void copyTextToClipboard(String s) {}
    public void getTextFromClipboard() {}
    public String getSubChannel() {return "";}
    public void bindUser() {}

    public String getNetworkTypeFromStatus() {
        ConnectivityManager connectivityManager = (ConnectivityManager) this.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = connectivityManager.getActiveNetworkInfo();
        if (networkInfo != null && networkInfo.isConnected()) {
            int type = networkInfo.getType();
            if (type == ConnectivityManager.TYPE_WIFI) {
                return "WiFi";
            } else if (type == ConnectivityManager.TYPE_MOBILE) {
                TelephonyManager telephonyManager = (TelephonyManager) this.getSystemService(Context.TELEPHONY_SERVICE);
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                    if (checkSelfPermission(Manifest.permission.READ_PHONE_STATE) != PackageManager.PERMISSION_GRANTED) {
                        Utils.showToast("Not READ_PHONE_STATE Permission");
                        // TODO: Consider calling
                        //    Activity#requestPermissions
                        // here to request the missing permissions, and then overriding
                        //   public void onRequestPermissionsResult(int requestCode, String[] permissions,
                        //                                          int[] grantResults)
                        // to handle the case where the user grants the permission. See the documentation
                        // for Activity#requestPermissions for more details.
                        return "Unknown";
                    }
                }
                int networkType = telephonyManager.getNetworkType();
                switch (networkType) {
                    case TelephonyManager.NETWORK_TYPE_GPRS:
                    case TelephonyManager.NETWORK_TYPE_EDGE:
                    case TelephonyManager.NETWORK_TYPE_CDMA:
                    case TelephonyManager.NETWORK_TYPE_1xRTT:
                    case TelephonyManager.NETWORK_TYPE_IDEN:
                        return "2G";
                    case TelephonyManager.NETWORK_TYPE_UMTS:
                    case TelephonyManager.NETWORK_TYPE_EVDO_0:
                    case TelephonyManager.NETWORK_TYPE_EVDO_A:
                    case TelephonyManager.NETWORK_TYPE_HSDPA:
                    case TelephonyManager.NETWORK_TYPE_HSUPA:
                    case TelephonyManager.NETWORK_TYPE_HSPA:
                    case TelephonyManager.NETWORK_TYPE_EVDO_B:
                    case TelephonyManager.NETWORK_TYPE_EHRPD:
                    case TelephonyManager.NETWORK_TYPE_HSPAP:
                        return "3G";
                    case TelephonyManager.NETWORK_TYPE_LTE:
                        return "4G";
                    case TelephonyManager.NETWORK_TYPE_NR:
                        return "5G";
                    default:
                        return "Unknown";
                }
            }
        }
        return "Unknown";
    }
    public String getSIMStatus() {
        TelephonyManager telephonyManager = (TelephonyManager) this.getSystemService(Context.TELEPHONY_SERVICE);
        int simState = telephonyManager.getSimState();
        switch (simState) {
            case TelephonyManager.SIM_STATE_ABSENT:
                return "Unknown";
            case TelephonyManager.SIM_STATE_UNKNOWN:
                return "Unknown";
            case TelephonyManager.SIM_STATE_NETWORK_LOCKED:
                return "NetworkLocked";
            case TelephonyManager.SIM_STATE_PIN_REQUIRED:
                return "PINRequired";
            case TelephonyManager.SIM_STATE_PUK_REQUIRED:
                return "PUKRequired";
            case TelephonyManager.SIM_STATE_READY:
                return telephonyManager.getSimOperatorName();
            default:
                return "Unknown";
        }
    }

    // test
    public void commonMethod() {
		Utils.SendMessageToUnity("_commonMethod");
    }
    public String methodAndReturn() {
        Utils.Log("methodAndReturn");
        return "test methodAndReturn";
    }
}