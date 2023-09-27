using System;
using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 文件下载
/// </summary>
public class DownloadWrap
{
    private static string downloadPathRoot = "http://192.168.1.2:8000";
    private static string file = "TestDownload.zip";

    private static bool checkNetworkState(UnityWebRequest uwr)
    {
        if (uwr.isHttpError || uwr.isNetworkError)
        {
            Debug.LogError(uwr.error);
            return false;
        }
        if (!string.IsNullOrEmpty(uwr.error))
        {
            Debug.LogError($"Not Find File {uwr.uri}");
            return false;
        }
        return true;
    }

    public static float fileSizeByteToMB(ulong byteSize)
    {
        return byteSize / 1024 / 1024;
    }

    /// <summary>
    /// 下载一个文件
    /// </summary>
    /// <param name="downloadPath">文件下载地址</param>
    /// <param name="outpath">文件保存路径</param>
    /// <param name="isBreakPoint">断点续传，需要服务器支持</param>
    /// <param name="cancellationToken">取消</param>
    /// <param name="onDownload">下载进度<进度,总大小></param>
    /// <param name="onDownloadCompleted">下载完成</param>
    /// <returns></returns>
    public static async UniTaskVoid downloadFile(string downloadPath, string outpath, bool isBreakPoint, CancellationToken cancellationToken,
        Action<float, float> onDownload = null, Action onDownloadCompleted = null)
    {
        ulong totalSize = 0, downloadSize = 0;
        string downloadFilePath = downloadPath ?? $"{downloadPathRoot}/{file}";
        outpath = outpath ?? $"{Application.persistentDataPath}/DonloadFile/{Path.GetFileName(downloadFilePath)}";

        // get download file info
        using (UnityWebRequest headRequest = UnityWebRequest.Head(downloadFilePath))
        {
            await headRequest.SendWebRequest();
            if (!checkNetworkState(headRequest))
                return;
            totalSize = ulong.Parse(headRequest.GetResponseHeader("Content-Length"));
            Debug.Log($"Download {downloadPath} Size {fileSizeByteToMB(totalSize)}M");
        }

        if (isBreakPoint)
        {
            downloadSize = File.Exists(outpath) ? (ulong)new FileInfo(outpath).Length : 0;
            Debug.Log("BreakPoint");
            Debug.Log($"已经下载 {downloadSize}");
            Debug.Log($"需要下载 {totalSize - downloadSize}");
            if (downloadSize == totalSize)
            {
                Debug.LogWarning("已下载");
                onDownloadCompleted?.Invoke();
                return;
            }
        }

        // download
        using (UnityWebRequest dataRequest = UnityWebRequest.Get(downloadFilePath))
        {
            // downloadHandler
            DownloadHandlerFile downloadHandler = new DownloadHandlerFile(outpath, isBreakPoint);
            downloadHandler.removeFileOnAbort = !isBreakPoint;
            dataRequest.downloadHandler = downloadHandler;
            if (isBreakPoint)
            {
                dataRequest.SetRequestHeader("Range", $"bytes={downloadSize}-{totalSize}");
            }
            var progress = Progress.Create<float>(x =>
                onDownload?.Invoke((x * (totalSize - downloadSize) + downloadSize) / totalSize, fileSizeByteToMB(totalSize))
            );
            await dataRequest.SendWebRequest().ToUniTask(progress: progress, cancellationToken: cancellationToken);
            if (!checkNetworkState(dataRequest))
                return;
            onDownloadCompleted?.Invoke();
        }
    }
}