using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using Unity.Notifications;

class BuildTools
{
    static string sdkRootDir = Path.Combine(Application.dataPath, @"../../SDK").Replace(@"\", "/");
    static string buildOutRootDir = sdkRootDir.Replace(@"../../SDK", @"../../Build");

    [MenuItem("Build/Test")]
    static void test()
    {
        Debug.Log("Test.");
        Debug.Log($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}");
        Debug.Log(Application.platform);
        Debug.Log(sdkRootDir);
        Debug.Log(buildOutRootDir);
        Debug.Log(getChannel());
        buildApp();
    }

    enum Channel
    {
        QQ,
        JueFeng,
        NULL
    }

    static Channel getChannel()
    {
#if SDK && QQSDK && UNITY_ANDROID
        return Channel.QQ;
#elif SDK && JFSDK
        return Channel.JueFeng;
#else
        return Channel.NULL;
#endif
    }

    /// <summary>
    /// 构建APK
    /// </summary>
    [MenuItem("Build/Build APK")]
    static void buildAPK()
    {
        if (!exportAndroid())
            return;
        buildApp();
    }

    /// <summary>
    /// 构建IPA
    /// </summary>
    [MenuItem("Build/Build IPA")]
    static void buildIPA()
    {
        if (!exportIOS())
            return;
        buildApp();
    }

    static void buildApp()
    {
        string progressBarTitle = platformDiff("APK", "IPA");
        string buildScriptDir = platformDiff($"{sdkRootDir}/Android", $"{sdkRootDir}/IOS");
        string projectDir = platformDiff($"{buildOutRootDir}/Android/AndroidProject", $"{buildOutRootDir}/IOS/IOSProject");
        string outDir = platformDiff($"{buildOutRootDir}/Android/APK", $"{buildOutRootDir}/IOS/IPA");
        EditorUtility.DisplayProgressBar($"Build {progressBarTitle}", "Building...", 1);
        // 执行打包脚本只支持shell
        var r = CommandLine.RunFromShell($"cd {buildScriptDir} && ./build.sh {projectDir} {outDir}");
        EditorUtility.ClearProgressBar();
        if (Regex.Match(r, "Build Success").Success)
        {
            EditorUtility.DisplayDialog("", "Build Completed", "ok");
            Debug.LogWarning(r);
        }
        else
        {
            EditorUtility.DisplayDialog("", "Build Fail", "ok");
            Debug.LogError(r);
        }
    }

    /// <summary>
    /// 导出Android项目
    /// </summary>
    [MenuItem("Build/Export AndroidProject")]
    static bool exportAndroid()
    {
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            return false;
        Channel channel = getChannel();
        string outPath = $"{buildOutRootDir}/Android/AndroidProject";
        string sdkLibsDir = $"{sdkRootDir}/Android/{channel}/libs";
        string sdkSrcDir = $"{sdkRootDir}/Android/{channel}/src";
        string outLibsDir = $"{outPath}/unityLibrary/libs";
        string outSrcDir = $"{outPath}/unityLibrary/src";
        string myPackgeName = PlayerSettings.applicationIdentifier.Replace(".", "/");
        // 设为导出
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        // <消息推送指定UnityPlayerActivity位置>
        if (channel == Channel.NULL)
        {
            NotificationSettings.AndroidSettings.UseCustomActivity = false;
            NotificationSettings.AndroidSettings.CustomActivityString = "com.unity3d.player.UnityPlayerActivity";
        }
        else
        {
            NotificationSettings.AndroidSettings.UseCustomActivity = true;
            NotificationSettings.AndroidSettings.CustomActivityString = "com.lyh.t1.game.UnityPlayerActivity";
        }
        // 签名
        setAndroidKeystore();
        if (!build(BuildTarget.Android, outPath))
            return false;
        if (channel == Channel.NULL)
            goto AndroidProjectConfig;
        // sdk配置
        // libs
        FDTools.copyDirectory(sdkLibsDir, outLibsDir);
        // AndroidManifest.xml
        FDTools.copyFile($"{sdkSrcDir}/AndroidManifest.xml", $"{outSrcDir}/main");
        // codes
        FDTools.delDir($"{outSrcDir}/main/java/");
        FDTools.copyFile(Directory.GetFiles(sdkSrcDir, "*.java"), $"{outSrcDir}/main/java/{myPackgeName}/game");
    AndroidProjectConfig:
        if (channel != Channel.NULL)
        {
            foreach (var path in Directory.GetFiles(sdkLibsDir))
            {
                string ext = Path.GetExtension(path).Replace(".", null);
                string name = Path.GetFileName(path).Replace($".{ext}", null);
                FDTools.addLineAfterInFile($"{outPath}/unityLibrary/build.gradle", 7, $"    implementation(name: '{name}', ext:'{ext}')");
            }
        }
        // NOTE: Android Gradle Plugin Version要与Gradle Version匹配
        // UNITY_2019_4 Android Gradle Plugin为3.40 Gradle Version为5.1.1不同Unity版本导出Android项目使用的Gradle Plugin Version会有差别
        // 如果使用gradle命令行打包可以忽略这一步用不到gradlew
        // 使用AS来打包APK则需要保留因为AS默认会使用最新的gradle
#if UNITY_2019_4
        FDTools.copyDirectory($"{sdkRootDir}/Android/gradlew5.1.1/", $"{outPath}/gradle");
#endif
        return true;
    }

    /// <summary>
    /// 安卓签名配置
    /// </summary>
    static void setAndroidKeystore()
    {
        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = $"{sdkRootDir}/Android/keystore/r2.jks";
        PlayerSettings.Android.keystorePass = "123456";
        PlayerSettings.Android.keyaliasName = "release.keystore";
        PlayerSettings.Android.keyaliasPass = "123456";
    }

    /// <summary>
    /// 运行目标为模拟器
    /// Unity导出运行在模拟器的项目是x86的，m1芯片的设备需要设置Xcode的Rosetta模式
    /// 如果引用了没有对x86兼容的库将编译时将不会link
    /// </summary>
    // [MenuItem("Build/export IOSProject(Simulator)")]
    static bool exportIOSRunInSimulator()
    {
        return exportIOS(iOSSdkVersion.SimulatorSDK);
    }

    /// <summary>
    /// 运行目标为真机
    /// </summary>
    [MenuItem("Build/Export IOSProject")]
    static bool exportIOS()
    {
        return exportIOS(iOSSdkVersion.DeviceSDK);
    }

    /// <summary>
    /// 导出IOS项目
    /// </summary>
    /// <param name="iOSSdkVersion"></param>
    static bool exportIOS(iOSSdkVersion iOSSdkVersion)
    {
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS))
            return false;
        Channel channel = getChannel();
        string outPath = $"{buildOutRootDir}/IOS/IOSProject";
        string sdkPlugins = $"{sdkRootDir}/IOS/{channel}/Plugins/IOS";
        string outPlugins = $"{Application.dataPath}/Plugins/IOS";
        string sdkAssets = $"{sdkRootDir}/IOS/{channel}/Assets";
        string outAssets = $"{outPath}";
        string sdkReplaceFiles = $"{sdkRootDir}/IOS/{channel}/ReplaceFiles";
        string outReplaceFiles = $"{outPath}/Classes";
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion;
        PlayerSettings.iOS.appleDeveloperTeamID = "LNQXTJQ5C3";
        PlayerSettings.iOS.appleEnableAutomaticSigning = true;
#if !UNITY_2020_1_OR_NEWER
        if (iOSSdkVersion == iOSSdkVersion.SimulatorSDK)
        {
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
        }
        else
        {
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, true);
        }
#else
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, true);
#endif
        if (channel == Channel.NULL)
        {
            if (!build(BuildTarget.iOS, outPath))
                return false;
            goto XCodeConfig;
        }
        // 将新增的sdk code放到ios目录下导出ios项目可以减少xcode配置
        var pluginsInfo = FDTools.copyDirectory(sdkPlugins, outPlugins);
        AssetDatabase.Refresh();
        if (!build(BuildTarget.iOS, outPath))
            return false;
        FDTools.copyDirectory(sdkAssets, outAssets);
        FDTools.copyDirectory(sdkReplaceFiles, outReplaceFiles);
        foreach (var path in pluginsInfo)
        {
            if (Directory.Exists(path))
                continue;
            File.Delete(path);
            File.Delete($"{ path}.meta");
        };
        AssetDatabase.Refresh();
    XCodeConfig:
        setXCodeConfig(outPath);
        return true;
    }

    static bool build(BuildTarget buildTarget, string path)
    {
        FDTools.delDir(path);
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.locationPathName = path;
        buildPlayerOptions.target = buildTarget;
        bool isDebug = EditorUserBuildSettings.development;
        // Debug包打开Script Debugging
        buildPlayerOptions.options = isDebug ? BuildOptions.Development | BuildOptions.AllowDebugging : BuildOptions.None;
        buildPlayerOptions.scenes = EditorBuildSettings.scenes.Select((scene) => scene.path).ToArray();
        BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log($"BuildTarget:{buildTarget} Channel:{getChannel()}");
        if (buildReport.summary.result == BuildResult.Succeeded)
            Debug.Log("Build Succeeded.");
        else
            Debug.LogError("Build Failed.");
        return buildReport.summary.result == BuildResult.Succeeded;
    }

    /// <summary>
    /// Xcode配置
    /// </summary>
    /// <param name="path"></param>
    /// <param name="channel"></param>
    static void setXCodeConfig(string path)
    {
        PBXProject project = new PBXProject();
        project.ReadFromFile(PBXProject.GetPBXProjectPath(path));
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(Path.Combine(path, "Info.plist"));
        string mainTarget = project.GetUnityMainTargetGuid();
        string unityFrameworkTarget = project.GetUnityFrameworkTargetGuid();
        switch (getChannel())
        {
            case Channel.QQ:
                break;
            case Channel.JueFeng:
                // BuildSettings
                // https://help.apple.com/xcode/mac/current/#/itcaec37c2a6
                project.SetBuildProperty(mainTarget, "ENABLE_BITCODE", "NO");
                project.SetBuildProperty(unityFrameworkTarget, "ENABLE_BITCODE", "NO");
                project.SetBuildProperty(mainTarget, "ONLY_ACTIVE_ARCH", "NO");
                project.SetBuildProperty(unityFrameworkTarget, "ONLY_ACTIVE_ARCH", "NO");
                // todo Signing
                project.SetTeamId(mainTarget, "LNQXTJQ5C3");
                // BuildPhases
                // 加入至CopyBundleResources
                string filePath = Path.Combine("Resource", "JFGAMESDK.bundle");
                string fileGuid = project.AddFile(filePath, filePath, PBXSourceTree.Source);
                project.AddFileToBuild(mainTarget, fileGuid);
                // 加入至CompileSources
                // filePath = Path.Combine("Apple", "AppleLogin.m");
                // fileGuid = project.AddFile(filePath, filePath, PBXSourceTree.Source);
                // var sourcesBuildPhase = project.GetSourcesBuildPhaseByTarget(mainTarget);
                // project.AddFileToBuildSection(target, sourcesBuildPhase, fileGuid);
                // 加入至LinkBinaryWithLibraries
                // 添加系统Framework
                project.AddFrameworkToProject(unityFrameworkTarget, "AdSupport.framework", false); // 弱引用为false时对应xcode中的Required
                // TODO 添加SwiftPackageManager中的Framework
                // 添加本地Framework
                filePath = "JFGAMESDK.framework";
                fileGuid = project.AddFile(filePath, "Frameworks/" + filePath, PBXSourceTree.Source);
                project.AddFileToBuild(mainTarget, fileGuid);
                project.AddFileToBuild(unityFrameworkTarget, fileGuid);
                // mbed&Sign
                PBXProjectExtensions.AddFileToEmbedFrameworks(project, mainTarget, fileGuid);
                PBXProjectExtensions.AddFileToEmbedFrameworks(project, unityFrameworkTarget, fileGuid);
                // 添加本地Framework时需要手动设置SEARCH_PATHS不然会找不到库
                // 在xcode中添加本地库时会自动设置这些(unity2019.4添加库时不会自动设置)
                project.SetBuildProperty(mainTarget, "LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
                project.SetBuildProperty(mainTarget, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
                project.SetBuildProperty(unityFrameworkTarget, "LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
                project.SetBuildProperty(unityFrameworkTarget, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
                project.SetBuildProperty(mainTarget, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
                project.AddBuildProperty(mainTarget, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)");
                project.SetBuildProperty(unityFrameworkTarget, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
                project.AddBuildProperty(unityFrameworkTarget, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)");
                // Info配置
                // URLType
                PlistElementArray urlType = plist.root.CreateArray("CFBundleURLTypes");
                PlistElementDict u1 = urlType.AddDict();
                u1.SetString("CFBundleTypeRole", "Editor");
                u1.CreateArray("CFBundleURLSchemes").AddString("JFGAME81205");
                PlistElementDict u2 = urlType.AddDict();
                u2.SetString("CFBundleTypeRole", "Editor");
                u2.SetString("CFBundleURLName", "yinxinyun");
                u2.CreateArray("CFBundleURLSchemes").AddString("yinxinyunClient.yiigames.com");
                // AppTransportSecuritySettings
                plist.root["NSAppTransportSecurity"].AsDict().SetBoolean("NSAllowsArbitraryLoads", true);
                // LSApplicationQueriesSchemes/Queried URL Schemos
                PlistElementArray quslist = plist.root.CreateArray("LSApplicationQueriesSchemes");
                quslist.AddString("alipay");
                quslist.AddString("alipayshare");
                quslist.AddString("weixin");
                quslist.AddString("weichat");
                break;
            case Channel.NULL:
                break;
        }
        // <通知>需要的库
        project.AddFrameworkToProject(unityFrameworkTarget, "UserNotifications.framework", false);
        plist.WriteToFile(Path.Combine(path, "Info.plist"));
        project.WriteToFile(PBXProject.GetPBXProjectPath(path));
    }

    /// <summary>
    /// 区分不同平台不同的值
    /// </summary>
    /// <typeparam name="T">CurrentPlatform</typeparam>
    /// <param name="p1">Android</param>
    /// <param name="p2">IOS</param>
    /// <returns></returns>
    static T platformDiff<T>(T p1, T p2)
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                return p1;
            case BuildTarget.iOS:
                return p2;
            default:
                throw new Exception("no match platform");
        }
    }

    [MenuItem("ShowInExplorer/SDK/Android")]
    static void openSDK_AndroidDir() => FDTools.showInExplorer(sdkRootDir + "/Android");
    [MenuItem("ShowInExplorer/SDK/IOS")]
    static void openSDK_IOSDir() => FDTools.showInExplorer(sdkRootDir + "/IOS");
    [MenuItem("ShowInExplorer/Build/Android")]
    static void openBuild_AndroidDir() => FDTools.showInExplorer(buildOutRootDir + "/Android");
    [MenuItem("ShowInExplorer/Build/IOS")]
    static void openBuild_IOSDir() => FDTools.showInExplorer(buildOutRootDir + "/IOS");
    [MenuItem("ShowInExplorer/PersistentData")]
    static void openPersistentDataPath() => FDTools.showInExplorer(Application.persistentDataPath);
}