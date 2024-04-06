using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance { get; private set; }

    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private Enums.VideoFormat _videoFormat;

    private bool _shown;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _shown = false;
        _videoPlayer.gameObject.SetActive(_shown);
    }

    public void ShowVideo(VideoInfo videoInfo)
    {
        StopVideo();

        if (!_shown)
        {
            _shown = true;
            _videoPlayer.gameObject.SetActive(_shown);
        }

        if (GameManager.Instance.UrlVideo)
        {
            _videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, GetCorrectUrl(videoInfo.Url));
        }
        else
        {
            _videoPlayer.clip = videoInfo.Clip;
        }

        _videoPlayer.gameObject.SetActive(true);
        _videoPlayer.Play();
    }

    public void StopVideo()
    {
        if (_videoPlayer.isPlaying)
        {
            _shown = false;
            _videoPlayer.Stop();
            _videoPlayer.gameObject.SetActive(_shown);
        }
    }

    public void PauseVideo()
    {
        _videoPlayer.Pause();
    }

    public void UnpauseVideo()
    {
        _videoPlayer.Play();
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
