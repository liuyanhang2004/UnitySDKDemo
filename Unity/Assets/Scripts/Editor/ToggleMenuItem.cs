using System;
using UnityEditor;
using UnityEngine;

public class ToggleMenuItem
{
    private string menuName;
    private string key;
    private Action<bool> onToggle;
    private bool def;

    public bool Checked => EditorPrefs.GetBool(key, def);

    public ToggleMenuItem(string menuName, Action<bool> onToggle = null, bool def = false)
    {
        this.menuName = menuName;
        this.key = $"{Application.dataPath}-{menuName}";
        this.onToggle = onToggle;
        this.def = def;
        EditorApplication.delayCall += init;
    }

    public void Toggle()
    {
        var newChecked = !Checked;
        onToggle?.Invoke(newChecked);
        EditorPrefs.SetBool(key, newChecked);
        Menu.SetChecked(menuName, newChecked);
    }

    private void init()
    {
        onToggle?.Invoke(Checked);
        Menu.SetChecked(menuName, Checked);
    }
}