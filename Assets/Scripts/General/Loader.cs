using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Loader : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private GameObject _treadmill;
    [SerializeField] private TextMeshProUGUI _loadingTx;

    [SerializeField] private AudioSource _backgroundSource;

    private Enums.Scenes _scene;
    public static bool Loading { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    #region Loading
    public async void LoadScene(Enums.Scenes scene, bool instantly = true)
    {
        _backgroundSource = GameObject.Find("Audio Manager").GetComponent<AudioManager>().GetBackgroundAudioSource();

        if (!instantly) await Task.Delay(1000);

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
        print(scene);

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
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            if (operation.progress == 0.9f && !LeanTween.isTweening(gameObject))
            {
                yield return new WaitForSeconds(5);
                Debug.Log("Loading completed");
                _backgroundSource.mute = true;
                backgroundSoundTime = _backgroundSource.time + 0.1f;
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        
        SetupLevel(scene, backgroundSoundTime);
        Loading = false;

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
        GameObject.Find("Audio Manager").GetComponent<AudioManager>().StartBackgroundAt(timeBackgroundSound);
        switch (scene)
        {
            case Enums.Scenes.Menu:
                print("Menu");
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
        LeanTween.scaleY(_treadmill, 1, 0.3f).setEaseInCubic();
    }
    #endregion
}
