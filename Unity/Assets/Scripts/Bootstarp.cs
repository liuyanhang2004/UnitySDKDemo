using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstarp : MonoBehaviour
{
    public GameObject Reporter;
    private void Awake()
    {
        initLogView();
        SDKMgr.CreateInstance();
        SDKMgr.inst.gameObject.AddComponent<LocalNotificationTest>();
#if UNITY_EDITOR
        if (PlayerPrefs.GetInt("EnableSplash", 0) == 1)
            SceneManager.LoadScene(1);
        else
            SceneManager.LoadScene(2);
#else
        SceneManager.LoadScene(1);
#endif
    }

    private void initLogView()
    {
        if (Debug.isDebugBuild)
            DontDestroyOnLoad(Reporter);
        else
            Destroy(Reporter);
    }
}
