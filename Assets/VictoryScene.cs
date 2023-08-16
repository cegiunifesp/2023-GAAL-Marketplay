using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScene : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private TextMeshProUGUI _scoreTx;

    [SerializeField] private Button _nextBt;
    [SerializeField] private Button _menuBt;
    [SerializeField] private Loader _loader;

    private void Awake()
    {
        _nextBt.onClick.AddListener(NextChapter);
    }

    public void Initiate()
    {
        _group.alpha = 1;
        _group.blocksRaycasts = true;

        _scoreTx.text = PlayerPrefs.GetInt("SCORE").ToString("0000");
    }

    private void NextChapter()
    {
        var loader = Instantiate(_loader);
        loader.NextLevel();
    }
}
