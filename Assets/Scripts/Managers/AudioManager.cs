using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    protected bool Muted;

    [SerializeField] protected bool PlayOnAwake;

    [SerializeField] protected AudioSource BackgroundAudioSource;
    [SerializeField] protected AudioSource SfxAudioSource;
    [SerializeField] protected AudioSource UiAudioSource;

    private void Awake()
    {
        if (PlayOnAwake) BackgroundAudioSource.playOnAwake = true;

        CheckMute();
        ChangeVolume(0.75f);
    }

    protected void CheckMute()
    {
        Muted = PlayerPrefs.GetInt("MUTED") == 1;

        if (BackgroundAudioSource != null) BackgroundAudioSource.mute = Muted;
        if (SfxAudioSource != null) SfxAudioSource.mute = Muted;
        if (UiAudioSource != null) UiAudioSource.mute = Muted;
    }

    public AudioSource GetBackgroundAudioSource()
    {
        return BackgroundAudioSource;
    }

    public void StartBackgroundAt(float time)
    {
        BackgroundAudioSource.time = time;
        BackgroundAudioSource.Play();
    }

    public void ChangeVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("VOLUME", volume);
    }

    public void Mute(bool value)
    {
        Muted = value;
        PlayerPrefs.SetInt("MUTED", Muted ? 1 : 0);

        if (BackgroundAudioSource != null) BackgroundAudioSource.mute = Muted;
        if (SfxAudioSource != null) SfxAudioSource.mute = Muted;
        if (UiAudioSource != null) UiAudioSource.mute = Muted;
    }

    public void ToggleMute()
    {
        Mute(!Muted);
    }

    public void PlaySFXSound(AudioClip clip, bool loop = false)
    {
        SfxAudioSource.PlayOneShot(clip);
        SfxAudioSource.loop = loop;
    }

    public void PlayUISound(AudioClip clip, bool loop = false)
    {
        UiAudioSource.PlayOneShot(clip);
        UiAudioSource.loop = loop;
    }
}
