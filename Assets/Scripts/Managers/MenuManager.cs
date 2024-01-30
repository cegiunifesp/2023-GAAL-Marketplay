using System.Threading;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine;

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
    [SerializeField] private Button _level1Bt;

    [Space(5)]
    [SerializeField] private Button _creditsBt;
    [SerializeField] private Button _Level2Bt;

    [Space(5)]
    [SerializeField] private Button _leaveBt;
    [SerializeField] private Button _level3Bt;

    [Space(5)]
    [SerializeField] private Button _nextTypeProductsBt;
    [SerializeField] private Button _previousTypeProductsBt;
    [SerializeField] private Button _leaveOptionsBt;

    [Space(5)]
    [SerializeField] private Button _leaveCredits;

    [Header("Others")]
    [SerializeField] MenuAudio _menuAudio;
    [SerializeField] private Loader _loader;


    private void Awake()
    {
        _firstEntry = true;

        ResetScore();

        _nextTypeProductsBt.onClick.AddListener(NextTypeProducts);
        _previousTypeProductsBt.onClick.AddListener(PreviousTypeProducts);
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
        _level1Bt.onClick.AddListener(() =>
        {
            VideoManager.Instance.StopVideo();

            var loader = Instantiate(_loader);
            loader.LoadScene(Enums.Scenes.Chapter1, instantly: false);
        });

        _Level2Bt.onClick.AddListener(() =>
        {
            VideoManager.Instance.StopVideo();

            var loader = Instantiate(_loader);
            loader.LoadScene(Enums.Scenes.Chapter2, instantly: false);
        });

        _level3Bt.onClick.AddListener(() =>
        {
            VideoManager.Instance.StopVideo();

            var loader = Instantiate(_loader);
            loader.LoadScene(Enums.Scenes.Chapter3, instantly: false);
        });

    }

    private void NextTypeProducts()
    {
        _shelf.GetChild(_typeIndex - 1).gameObject.SetActive(false);

        _typeIndex = (_typeIndex + 1) % 4;
        if (_typeIndex == 0) _typeIndex++;

        _shelf.GetChild(_typeIndex - 1).gameObject.SetActive(true);

        VideoManager.Instance.ShowVideo(_videos[_typeIndex - 1]);
        GameManager.Instance.SetTypeSelected(_typeIndex);
    }

    private void PreviousTypeProducts()
    {
        _shelf.GetChild(_typeIndex - 1).gameObject.SetActive(false);

        _typeIndex -= 1;
        if (_typeIndex <= 0) _typeIndex = 3;

        _shelf.GetChild(_typeIndex - 1).gameObject.SetActive(true);

        VideoManager.Instance.ShowVideo(_videos[_typeIndex - 1]);
        GameManager.Instance.SetTypeSelected(_typeIndex);
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

        VideoManager.Instance.ShowVideo(_videos[_typeIndex - 1]);
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
