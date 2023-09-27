using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;
using System.Threading;

public class UniTaskTest : MonoBehaviour
{
    public Button cancel;
    // Start is called before the first frame update
    async void Start()
    {
        var cts = new CancellationTokenSource();
        cancel.onClick.AddListener(() =>
        {
            cts.Cancel();
            Debug.LogWarning("取消等待");
        });
        await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: cts.Token).SuppressCancellationThrow();
        Debug.Log("Start End");
    }

    // Update is called once per frame
    void Update()
    {

    }
}