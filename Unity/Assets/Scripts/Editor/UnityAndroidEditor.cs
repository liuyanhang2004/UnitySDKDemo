// using System.Collections;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.IO;
// using UnityEditor;
// using UnityEditor.Callbacks;
// using UnityEngine;

// #if UNITY_ANDROID && SDK
// public class UnityAndroidEditor 
// {
//     [PostProcessBuildAttribute(100)]
//     public static void onPostProcessBuild(BuildTarget target, string targetPath)
//     {
//         if (target != BuildTarget.Android)
//             return;

//         string destJavaFileName = Path.Combine(targetPath, "unityLibrary/src/main/java/com/unity3d/player/UnityPlayerActivity.java");
//         if (!File.Exists(destJavaFileName))
//             return;

//         string sourceJavaFile = Path.Combine(Path.GetDirectoryName(Path.Combine(Application.dataPath.Replace("Assets", ""), AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:Script {nameof(UnityAndroidEdiror)}")[0]))), "UnityPlayerActivity.java.tmp");

//         if (!File.Exists(sourceJavaFile))
//             throw new System.Exception($"not find ${sourceJavaFile}");

//         File.Copy(sourceJavaFile, destJavaFileName, true);

//         string unityRoot = Path.GetDirectoryName(EditorApplication.applicationPath);
//         string OpenJDK = Path.Combine(unityRoot, "Data/PlaybackEngines/AndroidPlayer/OpenJDK/bin/java.exe");
//         string Gradle = Path.Combine(unityRoot, "Data/PlaybackEngines/AndroidPlayer/Tools/gradle/lib/gradle-launcher-6.1.1.jar");
//         string command = $"\"{OpenJDK}\" -Xmx4096m -Dorg.gradle.appname=gradle -classpath \"{Gradle}\" org.gradle.launcher.GradleMain clean assembleRelease";

//         UnityEngine.Debug.Log(command);
//         ProcessStartInfo startInfo = new ProcessStartInfo(command);
//         startInfo.WorkingDirectory = targetPath;
//         startInfo.RedirectStandardOutput = true;
//         startInfo.UseShellExecute = false;
//         startInfo.CreateNoWindow = true;

//         Process process = new Process();
//         process.StartInfo = startInfo;
//         process.Start();

//         string output = process.StandardOutput.ReadToEnd();
//         process.WaitForExit();
//         UnityEngine.Debug.Log(output);
//     }

// }
// #endif