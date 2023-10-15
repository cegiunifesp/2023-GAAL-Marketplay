using System;
using System.Collections;
using System.Threading.Tasks;
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

    [SerializeField] private AudioSource _backgroundSource;

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
        _backgroundSource = GameObject.Find("Audio Manager").GetComponent<AudioManager>().GetBackgroundAudioSource();

        if (!instantly) await UniTask.Delay(500, false, PlayerLoopTiming.Update, destroyCancellationToken);

        LeanTween.scaleX(_background.gameObject, 1.0f, 0.5f).setEaseInOutQuad().setOnComplete(InitiateVisual);
        LeanTween.value(1, 0, 1f).setOnUpdate((value) =>
        {
            Color textColor = _loadingTx.color;
            textColor.a = value;
            _loadingTx.color = textColor;
        }).setLoopPingPong();

        StartCoroutine(SetLoadingScene(scene));
    }

    private IEnumerator SetLoadingScene(Enums.Scenes scene)
    {
        yield return null;

        Loading = true;
        _scene = scene;
        float backgroundSoundTime = 0;

        var operation = scene switch
        {
            Enums.Scenes.Menu => SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single),
            Enums.Scenes.Chapter1 => SceneManager.LoadSceneAsync("Level 1", LoadSceneMode.Single),
            Enums.Scenes.Chapter2 => SceneManager.LoadSceneAsync("Level 2", LoadSceneMode.Single),
            Enums.Scenes.Chapter3 => SceneManager.LoadSceneAsync("Level 3", LoadSceneMode.Single),
            _ => throw new ArgumentOutOfRangeException(nameof(scene), scene, null)
        };
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            var progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (operation.progress == 0.9f && !LeanTween.isTweening(gameObject))
            {
                yield return new WaitForSeconds(5);
                //Debug.Log("Loading completed");
                backgroundSoundTime = _backgroundSource.time + 0.1f;
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        
        SetupLevel(scene, backgroundSoundTime);
        Loading = false;
        _productsParent.SetActive(false);

        LeanTween.scaleX(_background.gameObject, 0.0f, 0.3f).setEaseInOutQuad().setOnComplete(() =>
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

    private void SetupLevel(Enums.Scenes scene, float timeBackgroundSound)
    {
        GameObject.Find("Audio Manager").GetComponent<AudioManager>().StartBackgroundAt(timeBackgroundSound + 0.1f);
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
