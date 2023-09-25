using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class MacroItem
{
    public MacroItem(string name, bool enable)
    {
        Name = name;
        Enable = enable;
    }
    public string Name;
    public bool Enable;
}

public class MacroEditor : EditorWindow
{
    /// <summary>
    /// 支持平台
    /// </summary>
    static List<BuildTargetGroup> platforms = new List<BuildTargetGroup>
    {
        BuildTargetGroup.Standalone,
        BuildTargetGroup.Android,
        BuildTargetGroup.iOS,
    };
    Dictionary<BuildTargetGroup, List<MacroItem>> macroList = new Dictionary<BuildTargetGroup, List<MacroItem>>();
    Dictionary<BuildTargetGroup, List<string>> macroHistoryList = new Dictionary<BuildTargetGroup, List<string>>();
    bool isChange = false;

    [MenuItem("Tools/MacroEditor")]
    static void show() => CreateWindow<MacroEditor>();

    private void OnEnable()
    {
        foreach (var p in platforms)
        {
            InitMacro(p);
        }
    }

    private void OnLostFocus()
    {
        if (!EditorApplication.isCompiling)
        {
            saveMacro();
        }
        if (EditorApplication.isCompiling)
        {
            // 阻止窗口关闭
            FocusWindowIfItsOpen<MacroEditor>();
        }
    }

    private string filter(string v)
    {
        if (string.IsNullOrEmpty(v))
            return null;
        int index = v.LastIndexOf(';');
        if (index != v.Length - 1)
            return v;
        v = v.Substring(0, index);
        return v;
    }

    private List<string> macroStrToList(string str)
    {
        var v = filter(str);
        if (v == null)
            return new List<string>();
        return v.Split(';').ToList();
    }

    private void InitMacro(BuildTargetGroup buildTarget)
    {
        // 读取PlayerSetting中的宏
        var macroStrList = macroStrToList(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget));
        macroList.Add(buildTarget, new List<MacroItem>());
        // 读取历史记录
        var _macroHistoryList = macroStrToList(EditorPrefs.GetString($"{buildTarget}Macro"));
        macroHistoryList.Add(buildTarget, _macroHistoryList);
        // 添加PlayerSetting中的宏
        macroList[buildTarget].AddRange(macroStrList.Select(str => new MacroItem(str, true)));
        // 获取不在PlayerSetting中的宏
        macroList[buildTarget].AddRange(_macroHistoryList.Where((str) => !macroStrList.Contains(str)).ToList()
            .Select(str => new MacroItem(str, false)));
        // 获取不在记录中，重新存储
        _macroHistoryList.AddRange(macroStrList.Where((str) => !_macroHistoryList.Contains(str)));
        EditorPrefs.SetString($"{buildTarget}Macro", string.Join(";", _macroHistoryList));
        // Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget));
        // Debug.Log(EditorPrefs.GetString($"{buildTarget}Macro"));
    }

    private void OnGUI()
    {
        GUI.enabled = !EditorApplication.isCompiling;
        drawTab();
        drawContent();
    }

    List<string> tabList = Enum.GetNames(typeof(BuildTargetGroup))
        .Where(str => platforms.Select(v => v.ToString()).Contains(str)).Select(str => str == "iPhone" ? "iOS" : str).ToList();
    int currentTabIndex = 0;
    BuildTargetGroup currentTab = BuildTargetGroup.Standalone;
    private void drawTab()
    {
        tabList.Sort();
        tabList.Sort((a, b) =>
        {
            if (a == BuildPipeline.GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget))
                return -1;
            else
                return 1;
        });
        currentTabIndex = GUILayout.Toolbar(currentTabIndex, tabList.ToArray(), GUILayout.Height(30));
        currentTab = (BuildTargetGroup)Enum.Parse(typeof(BuildTargetGroup), tabList[currentTabIndex]);
    }

    Vector2 scrollPosition = Vector2.zero;
    string addInput = "";
    private void drawContent()
    {
        if (EditorApplication.isCompiling)
        {
            GUI.enabled = true;
            var labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.red;
            labelStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label("编译中···", labelStyle);
            GUI.enabled = !EditorApplication.isCompiling;
        }
        if (isChange)
        {
            GUI.enabled = true;
            var labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.green;
            labelStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label("更改未保存", labelStyle);
            GUI.enabled = !EditorApplication.isCompiling;
        }
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        MacroItem tempRemoveItem = null;
        foreach (var item in macroList[currentTab])
        {
            GUILayout.BeginHorizontal("box");
            setMacroState(item, GUILayout.Toggle(item.Enable, item.Name, GUILayout.Height(20)));
            if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
            {
                tempRemoveItem = item;
            }
            GUILayout.EndHorizontal();
        }
        // 处理删除
        // 一次OnGUI重绘不可能删除大于1个
        if (tempRemoveItem != null)
        {
            isChange = true;
            macroList[currentTab].Remove(tempRemoveItem);
            macroHistoryList[currentTab].Remove(tempRemoveItem.Name);
        }
        GUILayout.BeginHorizontal();
        addInput = GUILayout.TextField(addInput, GUILayout.Height(20), GUILayout.MinWidth(150));
        if (GUILayout.Button("添加", GUILayout.Height(20), GUILayout.Width(120)))
        {
            if (string.IsNullOrEmpty(addInput) || Regex.IsMatch(addInput, @";{2,}") || addInput.Contains(" "))
            {
                Debug.LogWarning("输入不合法");
                return;
            }
            List<string> macroStrList = macroStrToList(addInput);
            // 处理新增
            macroStrList = macroStrList.Where(str =>
            {
                foreach (var item in macroList[currentTab])
                {
                    if (item.Name != str)
                        continue;
                    setMacroState(item, true);
                    return false;
                }
                return true;
            }).ToList();
            macroList[currentTab].AddRange(macroStrList.Select(str => new MacroItem(str, true)));
            macroHistoryList[currentTab].AddRange(macroStrList);
            if (!isChange)
                isChange = macroStrList.Count > 0;
            addInput = null;
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("多个宏使用 ; 分隔");
        if (GUILayout.Button("关闭所有宏", GUILayout.Height(20)))
            closeMacro();
        GUILayout.Space(1);
        if (GUILayout.Button("清除未启用的宏", GUILayout.Height(20)))
            clearHistory();
        GUILayout.Space(1);
        if (GUILayout.Button("保存", GUILayout.Height(20)))
            saveMacro();
        GUILayout.EndScrollView();
    }

    /// <summary>
    /// 更改宏状态
    /// </summary>
    /// <param name="macroItem"></param>
    /// <param name="v"></param>
    private void setMacroState(MacroItem macroItem, bool v)
    {
        if (macroItem.Enable == v)
            return;
        macroItem.Enable = v;
        isChange = true;
    }

    /// <summary>
    /// MacroItemToStr
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    private string getMacroToStr(List<MacroItem> m)
    {
        string macro = default;
        foreach (var item in m)
        {
            if (!item.Enable)
                continue;
            macro += string.Format("{0};", item.Name);
        }
        return filter(macro);
    }

    /// <summary>
    /// 保存
    /// </summary>
    private void saveMacro()
    {
        isChange = false;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTab, getMacroToStr(macroList[currentTab]));
        EditorPrefs.SetString($"{currentTab}Macro", string.Join(";", macroHistoryList[currentTab]));
    }

    /// <summary>
    /// 清除未启用宏的记录
    /// </summary>
    private void clearHistory()
    {
        List<MacroItem> removeList = new List<MacroItem>();
        foreach (var item in macroList[currentTab])
        {
            if (item.Enable)
                continue;
            removeList.Add(item);
        }
        foreach (var item in removeList)
        {
            macroList[currentTab].Remove(item);
            macroHistoryList[currentTab].Remove(item.Name);
        }
        if (!isChange)
            isChange = removeList.Count > 0;
    }

    /// <summary>
    /// 关闭所有的宏
    /// </summary>
    private void closeMacro()
    {
        if (!isChange)
            isChange = macroList[currentTab].Find((v) => v.Enable) != null;
        foreach (var item in macroList[currentTab])
        {
            item.Enable = false;
        }
    }
}