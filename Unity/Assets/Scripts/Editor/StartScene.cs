using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class StartScene
{
    const string MENU1 = "Switches/Set Bootstarp as StartScene";
    const string MENU2 = "Switches/Splash";

    private static ToggleMenuItem Bootstarp;
    private static ToggleMenuItem Splash;

    static StartScene()
    {
        Bootstarp = new ToggleMenuItem(MENU1, OnBootstarpToggle);
        Splash = new ToggleMenuItem(MENU2, OnSplashToggle);
    }

    [MenuItem(MENU1, priority = 2)]
    static void SetBootStrap()
    {
        Bootstarp.Toggle();
    }

    static void OnBootstarpToggle(bool b)
    {
        if (!b)
        {
            EditorSceneManager.playModeStartScene = null;
        }
        else if (EditorSceneManager.playModeStartScene == null)
        {
            var bs = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Bootstarp.unity");
            EditorSceneManager.playModeStartScene = EditorSceneManager.playModeStartScene == bs ? null : bs;
        }
    }


    [MenuItem(MENU2, priority = 3)]
    static void SetSplash()
    {
        Splash.Toggle();
    }

    static void OnSplashToggle(bool b)
    {
        if (!b)
        {
            PlayerPrefs.SetInt("EnableSplash", 0);
        }
        else
        {
            PlayerPrefs.SetInt("EnableSplash", 1);
        }
    }
}