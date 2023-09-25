using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UILog : MonoBehaviour
{
    static UILog inst;
    [SerializeField]
    ScrollRect scrollRect;
    [SerializeField]
    GameObject logIPrefab, logWPrefab, logEPrefab;

    void Awake() => inst = this;

    static void instLog(GameObject logPrefab, object v)
    {
        Instantiate(logPrefab, inst.scrollRect.content).GetComponent<Text>().text = toString($"[{DateTime.Now.ToLongTimeString()}] {v}");
        LayoutRebuilder.ForceRebuildLayoutImmediate(inst.scrollRect.content);
        inst.scrollRect.verticalNormalizedPosition = 0;
    }

    public static void i(object v)
    {
        instLog(inst.logIPrefab, v);
        if (!Debug.isDebugBuild)
            return;
        Debug.Log(v);
    }
    public static void w(object v)
    {

        instLog(inst.logWPrefab, v);
        if (!Debug.isDebugBuild)
            return;
        Debug.LogWarning(v);
    }

    public static void e(object v)
    {
        instLog(inst.logEPrefab, v);
        if (!Debug.isDebugBuild)
            return;
        Debug.LogError(v);
    }

    public static void clear()
    {
        for (int i = 0; i < inst.scrollRect.content.childCount; i++)
            Destroy(inst.scrollRect.content.GetChild(i).gameObject);
    }

    static string toString(object message)
    {
        if (message == null)
        {
            return "Null";
        }

        IFormattable formattable = message as IFormattable;
        if (formattable != null)
        {
            return formattable.ToString(null, CultureInfo.InvariantCulture);
        }
        return message.ToString();
    }
}
