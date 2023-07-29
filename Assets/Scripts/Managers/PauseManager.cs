using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject _pausePanel;
    [SerializeField] Loader _loader;

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

    }

    public void GoBackToMenu()
    {

    }

}
