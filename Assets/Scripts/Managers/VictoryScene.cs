using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class VictoryScene : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private TextMeshProUGUI _scoreTx;
    [SerializeField] private TextMeshProUGUI _preScoreTx;

    [SerializeField] private Button _nextBt;
    [SerializeField] private Button _menuBt;
    [SerializeField] private Loader _loader;

    private const string DefaultPreScoreText = "Parabéns sua pontuação é ";
    private const string FinalPreScoreText = "Você completou todas as fases!\nSua pontuação é ";

    private void Awake()
    {
        _nextBt.onClick.AddListener(NextChapter);

        _menuBt.onClick.AddListener(BackToMenu);
    }

    public void Initiate()
    {
        _group.alpha = 1;
        _group.blocksRaycasts = true;

        _scoreTx.text = PlayerPrefs.GetInt("SCORE").ToString("0000");

        bool finishedAllLevels = SceneManager.GetActiveScene().buildIndex == (int)Enums.Scenes.Chapter3;
        ConfigureFinalVictory(finishedAllLevels);
    }

    private void ConfigureFinalVictory(bool finishedAllLevels)
    {
        if (_nextBt != null)
        {
            _nextBt.gameObject.SetActive(!finishedAllLevels);
        }

        if (_preScoreTx != null)
        {
            _preScoreTx.text = finishedAllLevels ? FinalPreScoreText : DefaultPreScoreText;
        }

        RectTransform menuRect = _menuBt != null ? _menuBt.GetComponent<RectTransform>() : null;
        if (menuRect == null) return;

        if (finishedAllLevels)
        {
            menuRect.anchorMin = new Vector2(0.4065f, 0.14334449f);
            menuRect.anchorMax = new Vector2(0.5935f, 0.363f);
        }
        else
        {
            menuRect.anchorMin = new Vector2(0.2f, 0.14334449f);
            menuRect.anchorMax = new Vector2(0.3864774f, 0.363f);
        }

        menuRect.anchoredPosition = Vector2.zero;
        menuRect.sizeDelta = Vector2.zero;
    }

    private void NextChapter()
    {
        var loader = Instantiate(_loader);
        loader.NextLevel();
    }

    private void BackToMenu()
    {
        var loader = Instantiate(_loader);
        loader.LoadScene(Enums.Scenes.Menu);
    }
}
