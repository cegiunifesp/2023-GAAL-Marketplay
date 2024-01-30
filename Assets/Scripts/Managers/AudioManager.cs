using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static Action<AudioClip, float, bool, bool> onPlaySFX { get; set; }
    public static void OnPlaySFX(AudioClip clip, float volume = -1, bool loop = false, bool Override = true)
    {
        onPlaySFX?.Invoke(clip, volume, loop, Override);
    }

    public static Action<AudioClip, float, bool> onPlayUI { get; set; }
    public static void OnPlayUI(AudioClip clip, float volume = -1, bool loop = false)
    {
        onPlayUI?.Invoke(clip, volume, loop);
    }


    public bool Muted { get; protected set; }

    [SerializeField] protected bool PlayOnAwake;
    [SerializeField] protected float sfxVolume;
    [SerializeField] protected float backgroundVolume;
    [SerializeField] protected float uiVolume;


    [SerializeField] protected AudioSource BackgroundAudioSource;
    [SerializeField] protected AudioSource AmbienceAudioSource;
    [SerializeField] protected AudioSource SfxAudioSource;
    [SerializeField] protected AudioSource UiAudioSource;

    [SerializeField] protected AudioSource[] _otherSources;

    private void Awake()
    {
        if (BackgroundAudioSource != null) BackgroundAudioSource.volume = backgroundVolume;
        if (SfxAudioSource != null) SfxAudioSource.volume = sfxVolume;
        if (UiAudioSource != null) UiAudioSource.volume = uiVolume;

        BackgroundAudioSource.playOnAwake = PlayOnAwake;

        CheckMute();
        ChangeAudioListenerVolume(0.75f);

        onPlaySFX += PlaySFXSound;
        onPlayUI += PlayUISound;
    }

    private void OnDestroy()
    {
        onPlaySFX -= PlaySFXSound;
        onPlayUI -= PlayUISound;
    }

    protected void CheckMute()
    {
        Muted = PlayerPrefs.GetInt("MUTED") == 1;

        //print($"Muted: {Muted}");

        if (BackgroundAudioSource != null) BackgroundAudioSource.mute = Muted;
        if (AmbienceAudioSource != null) AmbienceAudioSource.mute = Muted;
        if (SfxAudioSource != null) SfxAudioSource.mute = Muted;
        if (UiAudioSource != null) UiAudioSource.mute = Muted;
    }

    public void StartBackground(float time = 1)
    {
        BackgroundAudioSource.FadesIn();
        AmbienceAudioSource.FadesIn();
    }

    public void ChangeAudioListenerVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("VOLUME", volume);
    }

    public void Mute(bool value)
    {
        Muted = value;
        PlayerPrefs.SetInt("MUTED", Muted ? 1 : 0);

        if (BackgroundAudioSource != null) BackgroundAudioSource.mute = Muted;
        if (AmbienceAudioSource != null) AmbienceAudioSource.mute = Muted;
        if (SfxAudioSource != null) SfxAudioSource.mute = Muted;
        if (UiAudioSource != null) UiAudioSource.mute = Muted;

        foreach(var source in _otherSources) source.mute = Muted;
    }

    public void ToggleMute()
    {
        Mute(!Muted);
    }

    public void PlaySFXSound(AudioClip clip, float volume, bool loop = false, bool Override = false)
    {
        if (SfxAudioSource == null || clip == null) return;

        if (SfxAudioSource.isPlaying && !Override) return;

        SfxAudioSource.volume = volume == -1 ? SfxAudioSource.volume : volume;
        SfxAudioSource.PlayOneShot(clip);
        SfxAudioSource.loop = loop;
    }

    public void PlayUISound(AudioClip clip, float volume, bool loop = false)
    {
        if (UiAudioSource == null || clip == null) return;

        UiAudioSource.volume = volume == -1 ? UiAudioSource.volume : volume;
        UiAudioSource.loop = loop;
        UiAudioSource.PlayOneShot(clip);
    }

    public IEnumerator FadeAllSounds()
    {
        int faded = 0;
        int amountToFade = 0;

        if (UiAudioSource != null)
        {
            amountToFade++;
            UiAudioSource.FadesOut(() => faded++, default);
        }

        if (SfxAudioSource != null)
        {
            amountToFade++;
            SfxAudioSource.FadesOut(() => faded++, default);
        }

        if (BackgroundAudioSource != null)
        {
            amountToFade++;
            BackgroundAudioSource.FadesOut(() => faded++, default);
        }

        if (AmbienceAudioSource != null)
        {
            amountToFade++;
            AmbienceAudioSource.FadesOut(() => faded++, default);
        }

        yield return new WaitUntil(() => faded >= amountToFade);
    }

    internal void VictoryVolume()
    {
        BackgroundAudioSource.volume = BackgroundAudioSource.volume / 3;
        AmbienceAudioSource.volume = AmbienceAudioSource.volume / 3;
    }
}
