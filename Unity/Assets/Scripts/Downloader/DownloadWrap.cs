using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadWrap
{
    //
    private static string downloadPathRoot = "http://192.168.1.2:8000";
    private static string file = "TestDownload.zip";
    //

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
    /// 下载成功后才会保存
    /// </summary>
    /// <param name="path">下载路径</param>
    /// <param name="outpath">保存路径</param>
    /// <param name="onDownload"></param>
    /// <param name="onDownloadCompleted"></param>
    /// <returns></returns>
    public static IEnumerator download(string path, string outpath, Action<float, float> onDownload = null, Action onDownloadCompleted = null)
    {
        ulong totalSize;
        string downloadFilePath = path ?? $"{downloadPathRoot}/{file}";
        using (UnityWebRequest headRequest = UnityWebRequest.Head(downloadFilePath))
        {
            yield return headRequest.SendWebRequest();
            if (!checkNetworkState(headRequest))
            {
                yield break;
            }
            totalSize = ulong.Parse(headRequest.GetResponseHeader("Content-Length"));
            Debug.Log($"Download {path} Size {fileSizeByteToMB(totalSize)}M");
        }
        using (UnityWebRequest dataRequest = UnityWebRequest.Get(downloadFilePath))
        {
            dataRequest.SendWebRequest();
            while (!dataRequest.isDone)
            {
                if (!checkNetworkState(dataRequest))
                    yield break;
                onDownload?.Invoke(dataRequest.downloadProgress, fileSizeByteToMB(totalSize));
                yield return null;
            }
            onDownload?.Invoke(1, fileSizeByteToMB(totalSize));
            yield return null;
            // save
            outpath = outpath ?? $"{Application.persistentDataPath}/DonloadFile/{Path.GetFileName(downloadFilePath)}";
            var outdir = Path.GetDirectoryName(outpath);
            if (!Directory.Exists(outdir))
            {
                Directory.CreateDirectory(outdir);
            }
             File.WriteAllBytes(outpath, dataRequest.downloadHandler.data);
            onDownloadCompleted?.Invoke();
        }
    }

    /// <summary>
    /// 断点下载<服务器需要支持>
    /// 实时保存
    /// </summary>
    /// <param name="path"></param>
    /// <param name="outpath"></param>
    /// <param name="onDownload"></param>
    /// <param name="onDownloadCompleted"></param>
    /// <returns></returns>
    public static IEnumerator downloadFromBreakPoint(string path = null, string outpath = null, Action<float, float> onDownload = null, Action onDownloadCompleted = null)
    {
        ulong totalSize;
        string downloadFilePath = path ?? $"{downloadPathRoot}/{file}";
        using (UnityWebRequest headRequest = UnityWebRequest.Head(downloadFilePath))
        {
            yield return headRequest.SendWebRequest();
            if (!checkNetworkState(headRequest))
            {
                yield break;
            }
            totalSize = ulong.Parse(headRequest.GetResponseHeader("Content-Length"));
            Debug.Log($"Download {path} Size {fileSizeByteToMB(totalSize)}M");
        }
        outpath = outpath ?? $"{Application.persistentDataPath}/DonloadFile/{Path.GetFileName(downloadFilePath)}";
        var outdir = Path.GetDirectoryName(outpath);
        if (!Directory.Exists(outdir))
        {
            Directory.CreateDirectory(outdir);
        }
        FileStream fs = new FileStream(outpath, FileMode.OpenOrCreate, FileAccess.Write);
        ulong downloadSize = (ulong)fs.Length;
        Debug.Log($"已经下载 {downloadSize}");
        Debug.Log($"需要下载 {totalSize - downloadSize}");
        if (downloadSize == totalSize)
        {
            Debug.LogWarning("Have");
            onDownloadCompleted?.Invoke();
            fs.Dispose();
            yield break;
        }

        UnityWebRequest dataRequest = UnityWebRequest.Get(downloadFilePath);
        try
        {
            var downloadHandlerForBreakPoint = new DownloadHandlerForBreakPoint(fs, new byte[totalSize - downloadSize]);
            downloadHandlerForBreakPoint.onDownload = onDownload;
            dataRequest.downloadHandler = downloadHandlerForBreakPoint;
            dataRequest.SetRequestHeader("Range", $"bytes={downloadSize}-{totalSize}");
            yield return dataRequest.SendWebRequest();
            if (!checkNetworkState(dataRequest))
                yield break;
            onDownloadCompleted?.Invoke();
        }
        finally
        {
            dataRequest.downloadHandler.Dispose();
            dataRequest.Dispose();
        }
    }

    /// <summary>
    /// 优化下载实时写入获取<downloadHandler.data>GC高的问题
    /// </summary>
    public class DownloadHandlerForBreakPoint : DownloadHandlerScript
    {
        private FileStream fs;
        private ulong currSize = 0;
        private ulong totalSize = 0;
        public Action<float, float> onDownload;

        /// <param name="fs">FileStream</param>
        /// <param name="preallocatedBuffer">下载缓冲区</param>
        public DownloadHandlerForBreakPoint(FileStream fs, byte[] preallocatedBuffer) : base(preallocatedBuffer)
        {
            this.fs = fs;
            currSize = (ulong)fs.Length;
            currSize = (ulong)fs.Length;
            fs.Position = (long)currSize;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (!Application.isPlaying || data == null || data.Length < 1)
            {
                fs.Close();
                return false;
            }
            fs.Write(data, 0, dataLength);
            currSize += (ulong)dataLength;
            onDownload?.Invoke((float)currSize / totalSize, fileSizeByteToMB(totalSize));
            return true;
        }

        protected override void ReceiveContentLengthHeader(ulong contentLength)
        {
            totalSize = contentLength + currSize;
        }

        protected override void CompleteContent()
        {
            base.CompleteContent();
            fs.Close();
        }
    }
}
