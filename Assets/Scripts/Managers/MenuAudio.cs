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
        _toggleSoundBt.isOn = !Muted;
        _toggleSoundBt.onValueChanged.AddListener((value) => Mute(!value));
        BackgroundAudioSource.loop = true;
    }

    public void PlayCreditsClip()
    {
        FadeBackgroundClip(_creditsClip);
        //BackgroundAudioSource.clip = _creditsClip;
        //BackgroundAudioSource.Play();
    }

    public void PlayBackgroundClip()
    {
        FadeBackgroundClip(_backgroundClip);
        //BackgroundAudioSource.clip = _backgroundClip;
        //BackgroundAudioSource.Play();
    }

    private void FadeBackgroundClip(AudioClip clip)
    {
        float tempVolume = BackgroundAudioSource.volume;

        LeanTween.sequence()
            .append(
                LeanTween.value(tempVolume, 0f, 0.5f).setOnUpdate((value) =>
                {
                    BackgroundAudioSource.volume = value;
                }).setOnComplete(() => {
                    BackgroundAudioSource.clip = clip;
                    BackgroundAudioSource.Play();
                }))
            .append(
                LeanTween.value(0f, tempVolume, 0.75f).setOnUpdate((value) =>
                {
                    BackgroundAudioSource.volume = value;
                })
            );
    }
}
