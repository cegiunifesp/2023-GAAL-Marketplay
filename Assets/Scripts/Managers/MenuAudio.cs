using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuAudio : AudioManager
{
    [Header("Menu")]
    [SerializeField] private AudioClip _backgroundClip;
    [SerializeField] private AudioClip _creditsClip;

    [Space(10)]
    [SerializeField] Toggle _toggleSoundBt;

    private void Start()
    {
        _toggleSoundBt.onValueChanged.AddListener((value) => Mute(!value));
        _toggleSoundBt.isOn = !Muted;
    }

    public void PlayCreditsClip()
    {
        BackgroundAudioSource.clip = _creditsClip;
        BackgroundAudioSource.loop = true;
        BackgroundAudioSource.Play();
    }

    public void PlayBackgroundClip()
    {
        BackgroundAudioSource.clip = _backgroundClip;
        BackgroundAudioSource.loop = true;
        BackgroundAudioSource.Play();
    }
}
