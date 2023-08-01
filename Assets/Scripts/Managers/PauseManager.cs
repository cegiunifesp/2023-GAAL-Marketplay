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
        Events.OnPause(true);
        _pausePanel.SetActive(true);
    }

    public void UnpauseGame()
    {
        Events.OnPause(false);
        _pausePanel.SetActive(false);
    }

    public void RestartGame()
    {
        int indexScene = SceneManager.GetActiveScene().buildIndex;
        var loader = Instantiate(_loader);
        loader.LoadScene((Enums.Scenes)indexScene);
    }

    public void GoBackToMenu()
    {
        var loader = Instantiate(_loader);
        loader.LoadScene(Enums.Scenes.Menu);
    }

    public void ToggleSound()
    {
        _muted = !_muted;

        PlayerPrefs.SetInt("MUTED", _muted ? 1 : 0);
    }

}
