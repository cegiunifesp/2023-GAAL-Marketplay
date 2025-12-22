using Cysharp.Threading.Tasks;
using System;
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
        _videoPlayer.source = GameManager.Instance.UrlVideo ? VideoSource.Url : VideoSource.VideoClip;
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

        if (GameManager.Instance.UrlVideo)
        {

            string url = _videoFormat switch
            {
                Enums.VideoFormat.MP4 => _videoInfo.Url + _videoInfo.Clip.name + ".mp4",
                Enums.VideoFormat.OGV => _videoInfo.Url + _videoInfo.Clip.name + ".ogv",
                Enums.VideoFormat.WAV => _videoInfo.Url + _videoInfo.Clip.name + ".wav",
                _ => _videoInfo.Url + _videoInfo.Clip.name + ".mp4"
            };
            _videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, GetCorrectUrl(url));
        }
        else
        {
            _videoPlayer.clip = _videoInfo.Clip;
        }

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

    private string GetCorrectUrl(string videoUrl)
    {
        string url = _videoFormat switch
        {
            Enums.VideoFormat.MP4 => videoUrl.Substring(0, videoUrl.Length - 3) + "mp4",
            Enums.VideoFormat.OGV => videoUrl.Substring(0, videoUrl.Length - 3) + "ogv",
            Enums.VideoFormat.WAV => videoUrl.Substring(0, videoUrl.Length - 3) + "wav",
            _ => videoUrl
        };

        return url;
    }
}
