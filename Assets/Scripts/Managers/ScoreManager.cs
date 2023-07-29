using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreTx;

    private int _score;

    private void Start()
    {
        Events.onAddScore += HandleUpdateScore;
        Events.onGameEnded += HandleGameEnded;
    }

    private void HandleUpdateScore(int amount)
    {
        _score += amount;
        if (_score < 0) _score = 0;

        _scoreTx.text = $"Pontuação : {_score.ToString("0000")}";
    }

    private void HandleGameEnded()
    {
        PlayerPrefs.SetInt("SCORE", _score);
    }
}
