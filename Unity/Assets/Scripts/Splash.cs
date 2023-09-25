using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class Splash : MonoBehaviour
{
    public VideoPlayer video;
    public VideoClip videoClip;
    public RawImage target;
    public float speed = 1;
    RenderTexture outTexture;

    private void Awake()
    {
        playStartupVideo();
    }

    void playStartupVideo()
    {
        video.clip = videoClip;
        outTexture = RenderTexture.GetTemporary(1920, 1080);
        video.targetTexture = outTexture;
        target.texture = outTexture;
        target.color = Color.white;
        video.loopPointReached += (_) => SceneManager.LoadScene(2);
        video.playbackSpeed = speed;
        Debug.Log(video.canSetPlaybackSpeed);
        Debug.Log(video.playbackSpeed);
        video.Play();
    }

    private void OnDestroy()
    {
        outTexture.Release();
    }
}
