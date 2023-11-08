using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Diagnostics;

/// <summary>
/// File Dir Tool
/// </summary>
public class FDTools
{
    public static void hasFile(string path)
    {
        if (!File.Exists(path))
            throw new System.Exception($"{path} not exist.");
    }
    public static void hasDir(string path)
    {
        if (!Directory.Exists(path))
            throw new System.Exception($"{path} not exist.");
    }
    /// <summary>
    /// safe delete
    /// </summary>
    /// <param name="path"></param>
    public static void delDir(string path, bool self = true)
    {
        if (!Directory.Exists(path))
            return;
        Directory.Delete(path, true);
        if (!self)
            Directory.CreateDirectory(path);
    }
    /// <summary>
    /// safe delete
    /// </summary>
    /// <param name="path"></param>
    public static void delFile(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
    /// <summary>
    /// 复制一个文件
    /// 文件存在可替换
    /// </summary>
    /// <param name="sourceFilePath"></param>
    /// <param name="destinationDirectory"></param>
    public static void copyFile(string sourceFilePath, string destinationDirectory, bool overwrite = true)
    {
        hasFile(sourceFilePath);
        string fileName = Path.GetFileName(sourceFilePath);
        string destinationPath = Path.Combine(destinationDirectory, fileName);
        File.Copy(sourceFilePath, destinationPath, overwrite); // 'true' to overwrite if file already exists
                                                               // Debug.Log($"File {fileName} copied to {destinationPath}");
    }
    public static void copyFile(string[] sourceFilePath, string destinationDirectory, bool overwrite = true)
    {
        Directory.CreateDirectory(destinationDirectory);
        foreach (var f in sourceFilePath)
        {
            copyFile(f, destinationDirectory, overwrite);
        }
    }
    /// <summary>
    /// 复制一个文件夹
    /// 目录存在可删除
    /// 文件存在可替换
    /// </summary>
    /// <param name="sourceDirectory"></param>
    /// <param name="destinationDirectory"></param>
    public static List<string> copyDirectory(string sourceDirectory, string destinationDirectory, bool overwriteFile = true, List<string> _info = null)
    {
        if (_info == null)
            _info = new List<string>();
        hasDir(sourceDirectory);
        DirectoryInfo sourceDirInfo = new DirectoryInfo(sourceDirectory);
        //if (Directory.Exists(destinationDirectory) && isDeleteDestinationDirectory)
        //{
        //    Directory.Delete(destinationDirectory, true);
        //    Directory.CreateDirectory(destinationDirectory);
        //}
        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }
        foreach (var sourceFile in sourceDirInfo.GetFiles())
        {
            copyFile(sourceFile.FullName, destinationDirectory, overwriteFile);
            _info.Add(Path.Combine(destinationDirectory, sourceFile.Name));
        }

        foreach (var sourceSubDir in sourceDirInfo.GetDirectories())
        {
            string subDirDestination = Path.Combine(destinationDirectory, sourceSubDir.Name);
            _info.Add(subDirDestination);
            copyDirectory(sourceSubDir.FullName, subDirDestination, overwriteFile, _info);
        }
        return _info;
    }

    /// <summary>
    /// 替换文件某一行
    /// </summary>
    /// <param name="path"></param>
    /// <param name="lineN"></param>
    /// <param name="text"></param>
    public static void replaceLineInFile(string path, int lineN, string text)
    {
        hasFile(path);
        string[] lines = File.ReadAllLines(path);
        if (lineN <= 0 || lineN > lines.Length)
            throw new System.Exception("Invalid target line number.");
        lines[lineN - 1] = text;
        File.WriteAllLines(path, lines);
    }
    /// <summary>
    /// 追加一行
    /// </summary>
    /// <param name="path"></param>
    /// <param name="text"></param>
    public static void addLineInFile(string path, string text)
    {
        hasFile(path);
        List<string> lines = new List<string>(File.ReadAllLines(path));
        lines.Add(text);
        File.WriteAllLines(path, lines);
    }
    /// <summary>
    /// 在某一行之前添加一行
    /// </summary>
    /// <param name="path"></param>
    /// <param name="lineN"></param>
    /// <param name="text"></param>
    public static void addLineBeforeInFile(string path, int lineN, string text)
    {
        hasFile(path);
        List<string> lines = new List<string>(File.ReadAllLines(path));
        if (lineN <= 0 || lineN > lines.Count)
            throw new System.Exception("Invalid target line number.");
        lines.Insert(lineN - 1, text);
        File.WriteAllLines(path, lines);
    }
    /// <summary>
    /// 在某一行之后添加一行
    /// </summary>
    /// <param name="path"></param>
    /// <param name="lineN"></param>
    /// <param name="text"></param>
    public static void addLineAfterInFile(string path, int lineN, string text)
    {
        hasFile(path);
        List<string> lines = new List<string>(File.ReadAllLines(path));
        if (lineN <= 0 || lineN > lines.Count)
            throw new System.Exception("Invalid target line number.");
        lines.Insert(lineN, text);
        File.WriteAllLines(path, lines);
    }

    public static void showInExplorer(string path)
    {
#if UNITY_EDITOR_WIN
        Process.Start("explorer.exe", Path.GetFullPath(path));
#elif UNITY_EDITOR_OSX
        Process.Start("open", "-R " + Path.GetFullPath(path));
#else
        EditorUtility.RevealInFinder(path);
#endif
    }
}
