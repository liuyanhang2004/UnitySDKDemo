using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CommandLine
{
    public static string Run(string command, params object[] formatArgs)
    {
        return Run(command, false, formatArgs);
    }

    //public static string RunAsAdmin(string command, params object[] formatArgs)
    //{
    //    return Run(command, true, formatArgs);
    //}

    public static string RunFromShell(string command, params object[] formatArgs)
    {
        return Run(command, false, true, formatArgs);
    }

    //public static string RunFromShellAsAdmin(string command, params object[] formatArgs)
    //{
    //    return Run(command, true, true, formatArgs);
    //}

    static string[] shellDir = new string[] { @"C:/Program Files/Git/bin/bash.exe", "E:/Tools/Git/bin/bash.exe" };

    static string Run(string command, bool asAdmin, bool windowsUseShell = false, params object[] formatArgs)
    {
        var proc = new Process();
        var stdout = new StringBuilder();

        command = string.Format(command, formatArgs);

        Debug.Log("Execute Command: " + command);

        proc.EnableRaisingEvents = true;

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (!windowsUseShell)
            {
                proc.StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = "/C \"" + command + "\"",
                    UseShellExecute = asAdmin,
                    RedirectStandardError = !asAdmin,
                    RedirectStandardOutput = !asAdmin,
                    Verb = asAdmin ? "runas" : "",
                    CreateNoWindow = !asAdmin,
                    WorkingDirectory = Environment.CurrentDirectory,
                    StandardOutputEncoding = Encoding.GetEncoding("GB2312"),
                    StandardErrorEncoding = Encoding.GetEncoding("GB2312")
                };
            }
            else
            {
                string shellPath = default;
                foreach (var p in shellDir)
                {
                    if (!File.Exists(p))
                        continue;
                    shellPath = p;
                    break;
                }
                if (string.IsNullOrEmpty(shellPath))
                    throw new Exception("无法使用Shell，未安装或不在查找目录");
                proc.StartInfo = new ProcessStartInfo()
                {
                    FileName = shellPath,
                    Arguments = "-c \"" + command + "\"",
                    UseShellExecute = asAdmin,
                    RedirectStandardError = !asAdmin,
                    RedirectStandardOutput = !asAdmin,
                    Verb = asAdmin ? "runas" : "",
                    CreateNoWindow = !asAdmin,
                    WorkingDirectory = Environment.CurrentDirectory,
                };
            }
        }
        else
        {
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = "/bin/bash",
                Arguments = "-c \"" + command + "\"",
                UseShellExecute = asAdmin,
                RedirectStandardError = !asAdmin,
                RedirectStandardOutput = !asAdmin,
                CreateNoWindow = !asAdmin,
                WorkingDirectory = Environment.CurrentDirectory
            };
        }

        if (!asAdmin)
        {
            proc.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    stdout.AppendLine(args.Data);
            };

            proc.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    stdout.AppendLine(args.Data);
            };
        }

        if (proc.Start())
        {
            if (!asAdmin)
            {
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
            }

            proc.WaitForExit();
            if (proc.ExitCode == 0)
            {
                // Debug.Log(stdout);
                return stdout.ToString().Trim();
            }

            throw new Exception(string.Format("Command {0} exited with code {1} stdout: {2}", command, proc.ExitCode, stdout));
        }

        throw new Exception(string.Format("Failed to start process for {0}", command));
    }
}
