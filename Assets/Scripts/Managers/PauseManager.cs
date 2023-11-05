using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static Action onNeedToRestart { get; set; }
    public static void NeedToRestart()
    {
        onNeedToRestart?.Invoke();
        onNeedToRestart = null;
    }

    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Toggle _soundTg;
    [SerializeField] private Loader _loader;

    private bool _isMuted;

    private void Start()
    {
        _soundTg.onValueChanged.AddListener((value) =>
        {
            ToggleSound(value);
        });

        _isMuted = _audioManager.Muted;
        _soundTg.isOn = !_isMuted;

        onNeedToRestart += RestartGame;
    }

    public void PauseGame()
    {
        Events.Instance.OnPause(true);
        _pausePanel.SetActive(true);
    }

    public void UnpauseGame()
    {
        Events.Instance.OnPause(false);
        _pausePanel.SetActive(false);
    }

    public void RestartGame()
    {
        Events.Instance.OnPause(false);
        int indexScene = SceneManager.GetActiveScene().buildIndex;
        var loader = Instantiate(_loader);
        loader.LoadScene((Enums.Scenes)indexScene);
    }

    public void GoBackToMenu()
    {
        Events.Instance.OnPause(false);
        var loader = Instantiate(_loader);
        loader.LoadScene(Enums.Scenes.Menu);
    }

    private void ToggleSound(bool value)
    {
        _isMuted = !value;
        _audioManager.Mute(_isMuted);
    }

}
