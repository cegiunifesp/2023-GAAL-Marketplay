using System.Threading;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class MenuManager : MonoBehaviour
{
    private int _typeIndex;
    private bool _firstEntry;

    [SerializeField] private GameObject _creditsScene;
    [SerializeField] private GameObject _levelsSelection;

    [SerializeField] private Transform _shelf;
    [SerializeField] private VideoInfo[] _videos;

    [Header("Buttons And Texts")]
    [SerializeField] private Button _playBt;
    [SerializeField] private Button _creditsBt;
    [SerializeField] private Button _leaveBt;

    [Space(5)]
    [SerializeField] private Button _breakfastBt;
    [SerializeField] private Button _lunchBt;
    [SerializeField] private Button _hygieneBt;

    [Space(5)]
    [SerializeField] private Button _startGameplayBt;
    [SerializeField] private Button _leaveOptionsBt;

    [Space(5)]
    [SerializeField] private Button _leaveCredits;

    [Header("Others")]
    [SerializeField] MenuAudio _menuAudio;
    [SerializeField] private Loader _loader;

    private int _lastTypeSelected;

    private void Awake()
    {
        _firstEntry = true;

        ResetScore();

        _startGameplayBt.onClick.AddListener(StartChapter1Scene);
        _leaveOptionsBt.onClick.AddListener(LeaveLevelSelectionScene);

        SetButtonsToMenuOptions();
        SetLevelSelectionButtons();
    }

    [ContextMenu("Reset Score")]
    private void ResetScore()
    {
        PlayerPrefs.SetInt("SCORE", 0);
    }

    private void SetButtonsToMenuOptions()
    {
        _playBt.onClick.AddListener(() => ShowLevelSelectionScene());
        _creditsBt.onClick.AddListener(CreditsScene);
        _leaveBt.onClick.AddListener(LeaveGame);
        _leaveCredits.onClick.AddListener(LeaveCredits);
    }

    private void SetLevelSelectionButtons()
    {
        _breakfastBt.onClick.AddListener(() =>
        {
            TypeSelected(1);
        });

        _lunchBt.onClick.AddListener(() =>
        {
            TypeSelected(2);
        });

        _hygieneBt.onClick.AddListener(() =>
        {
            TypeSelected(3);
        });

    }

    private void StartChapter1Scene()
    {
        VideoManager.Instance.StopVideo();

        var loader = Instantiate(_loader);
        loader.LoadScene(Enums.Scenes.Chapter1, instantly: false);
    }

    private void TypeSelected(int typeIndex)
    {
        if (_lastTypeSelected > 0)
        {
            _shelf.GetChild(0).gameObject.SetActive(false);
            _shelf.GetChild(1).gameObject.SetActive(false);
            _shelf.GetChild(2).gameObject.SetActive(false);
        }
        _shelf.GetChild(typeIndex - 1).gameObject.SetActive(true);
        _lastTypeSelected = typeIndex;

        VideoManager.Instance.NewVideo(_videos[typeIndex - 1]);
        GameManager.Instance.SetTypeSelected(typeIndex);
    }

    private void ShowLevelSelectionScene()
    {
        if (_firstEntry)
        {
            GameManager.Instance.SetTypeSelected(-1);
            _typeIndex = (int)GameManager.Instance.TypeSelected;
            _shelf.GetChild(_typeIndex - 1).gameObject.SetActive(true);

            _firstEntry = false;
        }

        _levelsSelection.SetActive(true);

        VideoManager.Instance.NewVideo(_videos[_typeIndex - 1]);
    }

    private void LeaveLevelSelectionScene()
    {
        _levelsSelection.SetActive(false);
    }

    private void CreditsScene()
    {
        _menuAudio.PlayCreditsClip();
        _creditsScene.gameObject.SetActive(true);
    }

    private void LeaveCredits()
    {
        _menuAudio.PlayBackgroundClip();
        _creditsScene.SetActive(false);
    }

    private void LeaveGame()
    {
        Application.Quit();
    }
}
