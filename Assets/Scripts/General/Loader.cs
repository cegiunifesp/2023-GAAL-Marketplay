using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class Loader : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private GameObject _productsParent;
    [SerializeField] private TextMeshProUGUI _loadingTx;
    [SerializeField] private Canvas _canvas;

    [SerializeField] private AudioManager _backgroundSource;

    private bool _startSpinning;
    private Enums.Scenes _scene;
    public static bool Loading { get; private set; }

    private void Awake()
    {
        _canvas.worldCamera = Camera.main;
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (!_startSpinning) return;

        _productsParent.transform.Rotate(Vector3.forward * 40 * Time.deltaTime);
    }

    #region Loading
    public async void LoadScene(Enums.Scenes scene, bool instantly = true)
    {
        StartCoroutine(GameObject.Find("Audio Manager").GetComponent<AudioManager>().FadeAllSounds());

        if (!instantly) await UniTask.Delay(500, false, PlayerLoopTiming.Update, destroyCancellationToken);

        LeanTween.scaleX(_background.gameObject, 1.0f, 0.5f).setEaseInOutQuad().setOnComplete(() =>
        {
            InitiateVisual();
            SetLoadingScene(scene).Forget();

            LeanTween.value(1, 0, 1f).setOnUpdate((value) =>
            {
                Color textColor = _loadingTx.color;
                textColor.a = value;
                _loadingTx.color = textColor;
            }).setLoopPingPong();
        });
    }

    private async UniTaskVoid SetLoadingScene(Enums.Scenes scene)
    {
        _backgroundSource.StartBackground();

        Loading = true;
        _scene = scene;

        var sceneName = GetSceneName(scene);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        try
        {
            float startedAt = Time.realtimeSinceStartup;

            while (operation.progress < 0.9f)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, destroyCancellationToken);
            }

            int remainingMs = Mathf.CeilToInt((2.5f - (Time.realtimeSinceStartup - startedAt)) * 1000f);
            if (remainingMs > 0)
            {
                await UniTask.Delay(remainingMs, true, PlayerLoopTiming.Update, destroyCancellationToken);
            }

            operation.allowSceneActivation = true;

            while (!operation.isDone)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, destroyCancellationToken);
            }

            StartCoroutine(_backgroundSource.FadeAllSounds());

            await UniTask.Delay(500, true, PlayerLoopTiming.Update, destroyCancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        PerformExitAnimation();

        Loading = false;
        _productsParent.SetActive(false);
        SetupLevel(scene);
    }

    private string GetSceneName(Enums.Scenes scene)
    {
        switch (scene)
        {
            case Enums.Scenes.Menu: return "Menu";
            case Enums.Scenes.Chapter1: return "Level 1";
            case Enums.Scenes.Chapter2: return "Level 2";
            case Enums.Scenes.Chapter3: return "Level 3";
            default: throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
        }
    }

    private void PerformExitAnimation()
    {
        LeanTween.scaleX(_background.gameObject, 0.0f, 0.5f)
            .setEaseInOutQuad()
            .setOnComplete(() =>
            {
                LeanTween.cancel(gameObject);
                Destroy(gameObject);
            });
    }

    public void NextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int destiny = 0;
        if (currentSceneIndex < 3) destiny = currentSceneIndex + 1;
        
        LoadScene((Enums.Scenes)destiny); 
    }

    private void SetupLevel(Enums.Scenes scene)
    {
        switch (scene)
        {
            case Enums.Scenes.Menu:
                //print("Menu");
                break;
            default:
                Events.Instance.OnGameStart();
                break;
        }
    }
    #endregion

    #region Visual Part
    private void InitiateVisual()
    {
        _startSpinning = true;
        _productsParent.SetActive(true);
    }
    #endregion
}
