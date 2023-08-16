using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MenuManager : MonoBehaviour
{
    private int _typeIndex;
    private bool _firstEntry;

    private CancellationTokenSource _tokenSource;

    [SerializeField] private Transform _shelf;
    [SerializeField] private VideoClip[] _clips;

    [Header("Buttons And Texts")]
    [SerializeField] private Button _play_Level1_Bt;
    [SerializeField] private TextMeshProUGUI _play_Level1_Tx;

    [SerializeField] private Button _credits_Level2_Bt;
    [SerializeField] private TextMeshProUGUI _credits_Level2_Tx;

    [SerializeField] private Button _leave_Level3_Bt;
    [SerializeField] private TextMeshProUGUI _leave_Level3_Tx;

    [SerializeField] private Button _nextTypeProductsBt;
    [SerializeField] private Button _previousTypeProductsBt;
    [SerializeField] private Button _leaveOptionsBt;

    [SerializeField] private TextMeshProUGUI _monitorTx;

    [Header("Others")]
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private Loader _loader;


    private void Awake()
    {
        _tokenSource = new CancellationTokenSource();
        _firstEntry = true;

        _nextTypeProductsBt.onClick.AddListener(NextTypeProducts);
        _previousTypeProductsBt.onClick.AddListener(PreviousTypeProducts);
        _leaveOptionsBt.onClick.AddListener(LeaveOptionsScene);

        SetButtonsToMenuOptions();
    }

    [ContextMenu("Reset Score")]
    private void ResetScore()
    {
        PlayerPrefs.SetInt("SCORE", 0);
    }

    private void SetButtonsToMenuOptions()
    {
        _play_Level1_Bt.onClick.RemoveAllListeners();
        _play_Level1_Tx.text = "Jogar";
        _credits_Level2_Bt.onClick.RemoveAllListeners();
        _credits_Level2_Tx.text = "Créditos";
        _leave_Level3_Bt.onClick.RemoveAllListeners();
        _leave_Level3_Tx.text = "Sair";

        _play_Level1_Bt.onClick.AddListener(() => {
            ShowOptionsScene();
        });

        _credits_Level2_Bt.onClick.AddListener(() => Invoke("CreditsScene", 1));

        _leave_Level3_Bt.onClick.AddListener(LeaveGame);
    }

    private void SetButtonsToChapters()
    {
        _play_Level1_Bt.onClick.RemoveAllListeners();
        _play_Level1_Tx.text = "Fase 1";
        _credits_Level2_Bt.onClick.RemoveAllListeners();
        _credits_Level2_Tx.text = "Fase 2";
        _leave_Level3_Bt.onClick.RemoveAllListeners();
        _leave_Level3_Tx.text = "Fase 3";

        _play_Level1_Bt.onClick.AddListener(() =>
        {
            _videoPlayer.gameObject.SetActive(false);

            var loader = Instantiate(_loader);
            loader.LoadScene(Enums.Scenes.Chapter1, instantly: false);
        });

        _credits_Level2_Bt.onClick.AddListener(() =>
        {
            _videoPlayer.gameObject.SetActive(false);

            var loader = Instantiate(_loader);
            loader.LoadScene(Enums.Scenes.Chapter2, instantly: false);
        });

        _leave_Level3_Bt.onClick.AddListener(() =>
        {
            _videoPlayer.gameObject.SetActive(false);

            var loader = Instantiate(_loader);
            loader.LoadScene(Enums.Scenes.Chapter3, instantly: false);
        });
    }

    private void NextTypeProducts()
    {
        _shelf.GetChild(_typeIndex).gameObject.SetActive(false);
        _typeIndex = (_typeIndex + 1) % 3;
        _shelf.GetChild(_typeIndex).gameObject.SetActive(true);

        ShowVideo();

        GameManager.Instance.SetTypeSelected(_typeIndex);
    }

    private void PreviousTypeProducts()
    {
        _shelf.GetChild(_typeIndex).gameObject.SetActive(false);

        _typeIndex -= 1;
        if (_typeIndex < 0) _typeIndex = 2;

        _shelf.GetChild(_typeIndex).gameObject.SetActive(true);

        ShowVideo();

        GameManager.Instance.SetTypeSelected(_typeIndex);
    }

    private void ShowVideo()
    {
        if (_videoPlayer.isPlaying)
        {
            _videoPlayer.Stop();
        }

        _videoPlayer.clip = _clips[_typeIndex];
        _videoPlayer.gameObject.SetActive(true);
        _videoPlayer.Play();
    }

    private void ShowOptionsScene()
    {
        _monitorTx.text = "Escolha qual tipo de produto deseja";

        _nextTypeProductsBt.gameObject.SetActive(true);
        _previousTypeProductsBt.gameObject.SetActive(true);

        if (_firstEntry)
        {
            _typeIndex = Random.Range(0, 3);
            _shelf.GetChild(_typeIndex).gameObject.SetActive(true);

            ShowVideo();

            _firstEntry = false;
        }

        SetButtonsToChapters();
    }

    private void LeaveOptionsScene()
    {
        _nextTypeProductsBt.gameObject.SetActive(false);
        _previousTypeProductsBt.gameObject.SetActive(false);

        SetButtonsToMenuOptions();
    }

    private void CreditsScene()
    {

    }

    private void LeaveGame()
    {
        Application.Quit();
    }
}
