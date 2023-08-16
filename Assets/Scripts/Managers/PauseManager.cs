using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private bool _muted;

    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Loader _loader;

    private void Awake()
    {
        _muted = PlayerPrefs.GetInt("MUTED") == 1;
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

    public void ToggleSound()
    {
        _muted = !_muted;

        PlayerPrefs.SetInt("MUTED", _muted ? 1 : 0);
    }

}
