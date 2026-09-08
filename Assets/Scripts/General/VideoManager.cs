using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Threading;
using UnityEngine.Video;
using UnityEngine;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance { get; private set; }

    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private Enums.VideoFormat _videoFormat;

    private VideoInfo _videoInfo;

    private bool _shown;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _shown = false;
        _videoPlayer.source = VideoSource.Url;
        _videoPlayer.gameObject.SetActive(_shown);
    }

    public void NewVideo(VideoInfo videoInfo)
    {
        _videoInfo = videoInfo;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();

        ShowVideo();
    }

    public async void ShowVideo()
    {
        if (_videoPlayer == null) return;

        StopVideo();

        if (!_shown)
        {
            _shown = true;
            _videoPlayer.gameObject.SetActive(_shown);
        }

        string relativePath = GetRelativeStreamingPath();
        if (string.IsNullOrEmpty(relativePath))
        {
            Debug.LogWarning("VideoInfo is missing a folder or file name.");
            return;
        }

        _videoPlayer.source = VideoSource.Url;
        _videoPlayer.url = GetStreamingUrl(relativePath);

        if (_videoPlayer != null)
        {
            _videoPlayer.gameObject.SetActive(true);
            _videoPlayer.Play();
        }

        try
        {
            await UniTask.Delay((int)(_videoInfo.Duration * 1000), false, PlayerLoopTiming.Update, _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        if (_videoPlayer == null) return;

        ShowVideo();
    }

    public void StopVideo()
    {
        _shown = false;

        if (_videoPlayer == null) return;

        if (_videoPlayer.gameObject != null)
        {
            _videoPlayer.Stop();
            _videoPlayer.gameObject.SetActive(_shown);
        }
    }

    public void PauseVideo()
    {
        if (_videoPlayer == null) return;

        _videoPlayer.Pause();
    }

    public void UnpauseVideo()
    {
        if (_videoPlayer == null) return;

        _videoPlayer.Play();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;

        try
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
        catch { }
    }

    private string GetRelativeStreamingPath()
    {
        if (string.IsNullOrEmpty(_videoInfo.Url) || string.IsNullOrEmpty(_videoInfo.FileName))
        {
            return null;
        }

        string extension = _videoFormat switch
        {
            Enums.VideoFormat.OGV => ".ogv",
            Enums.VideoFormat.WAV => ".wav",
            _ => ".mp4"
        };

        string fileName = _videoInfo.FileName;
        if (!HasVideoExtension(fileName))
        {
            fileName += extension;
        }

        return Path.Combine(_videoInfo.Url, fileName);
    }

    private static bool HasVideoExtension(string fileName)
    {
        return fileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".ogv", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetStreamingUrl(string relativePath)
    {
        string path = Path.Combine(Application.streamingAssetsPath, relativePath).Replace('\\', '/');

        // Android and WebGL already provide a usable URL (jar: / http). Other platforms need a file URL.
#if !UNITY_ANDROID && !UNITY_WEBGL
        if (!path.Contains("://"))
        {
            path = "file:///" + path;
        }
#endif

        return path;
    }
}
