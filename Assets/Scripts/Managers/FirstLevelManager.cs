using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;
using TMPro;

public class FirstLevelManager : MonoBehaviour
{
    [Header("Related To Products")]
    [SerializeField] private ProductLevel1 _productPrefab;
    [SerializeField] private Transform _productsParent;
    [SerializeField] private Transform _shelf;

    [Header("Monitor")]
    [SerializeField] private Color _correctColor;
    [SerializeField] private Color _wrongColor;
    [SerializeField] private TextMeshProUGUI _monitorText;

    [Header("Others")]
    [SerializeField] private VictoryScene _finalScene;
    [SerializeField] private TextMeshProUGUI _leftOrdersTx;
    [SerializeField] private VideoPlayer _videoPlayer;

    private int _productsPooledThatAreNotTheOrder;
    private float _timeToActiveProducts;

    private bool _gameStarted;
    private bool _gameEnded;

    private Queue<ProductLevel1> _productsInTreadmill = new Queue<ProductLevel1>();
    private List<ProductSO> _productsAvailables;
    private List<ProductSO> _ordersAvailables;
    private Order _orderDesired;

    private CancellationTokenSource _tokenSource;


    private void Awake()
    {
        Events.onGameStart += HandleStart;
        Events.onPause += HandlePause;
        Events.onEnqueueProduct += HandleEnqueueProduct;
        Events.onProductSelected += HandleProductSelected;

        _tokenSource = new CancellationTokenSource();
    }

    private void Update()
    {
        if (!_gameStarted) return;
        if (_gameEnded) return;

        if (Time.time - _timeToActiveProducts > 3.5f)
        {
            if (PoolOtherProduct())
            {
                _timeToActiveProducts = Time.time;
            }
        }
    }

    [ContextMenu("Start Manually")]
    private void HandleStart()
    {
        switch (GameManager.Instance.TypeSelected)
        {
            case Enums.TypeProducts.Cafe:
                _shelf.GetChild(0).gameObject.SetActive(true);
                break;
            case Enums.TypeProducts.Almoco:
                _shelf.GetChild(1).gameObject.SetActive(true);
                break;
            case Enums.TypeProducts.Higiene:
                _shelf.GetChild(2).gameObject.SetActive(true);
                break;
        }

        _productsAvailables = GameManager.Instance.GetProductsAvailables();
        _ordersAvailables = new List<ProductSO>(_productsAvailables);

        GrowPool();

        Invoke("NewOrder", 1.5f);
        _timeToActiveProducts = Time.time;
        _gameStarted = true;
    }

    private void HandlePause(bool paused)
    {
        if (paused)
        {
            Time.timeScale = 0;
            _videoPlayer.Pause();
        }
        else
        {
            Time.timeScale = 1;
            _videoPlayer.Play();
        }
    }

    private void HandleProductSelected(ProductLevel1 product)
    {
        if (!product.IsInteractable()) return;

        if (product.ProductName == _orderDesired.ProductName)
        {
            product.ProductCorrect();
            CorrectOrder();
        }
        else
        {
            product.ProductIncorrect();
            IncorrectOrder();
        }
    }

    private void HandleEnqueueProduct(ProductLevel1 instance)
    {
        instance.gameObject.SetActive(false);
        instance.transform.eulerAngles = Vector3.zero;
        AddToPool(instance);
    }

    [ContextMenu("End Manually")]
    private void EndGame()
    {
        Events.onGameStart -= HandleStart;
        Events.onPause -= HandlePause;
        Events.onEnqueueProduct -= HandleEnqueueProduct;
        Events.onProductSelected += HandleProductSelected;

        Events.OnGameEnded();
        _gameEnded = true;
        _videoPlayer.Stop();
        _videoPlayer.gameObject.SetActive(false);

        _finalScene.Initiate();
    }


    #region Order
    private void NewOrder()
    {
        if (_ordersAvailables.Count == 0)
        {
            EndGame();
            print("Finished");
            return;
        }

        var productDesired = _ordersAvailables.GetRandomValue();

        _orderDesired = new Order(productDesired.ProductName, 1, productDesired.SpriteSource, productDesired.Clip);

        _ordersAvailables.Remove(_ordersAvailables.First(t => t.ProductName == productDesired.ProductName));
        _leftOrdersTx.text = $"Restam {_ordersAvailables.Count+1} produtos";

        ShowOrderVideo();

        _monitorText.color = Color.white;
        _monitorText.text = _orderDesired.ProductName;
    }

    private void CorrectOrder()
    {
        Events.OnAddScore(100);

        ChangeMonitorTextColor(correct: true);

        _tokenSource.Cancel();

        Invoke("NewOrder", 2);
    }

    private void IncorrectOrder()
    {
        Events.OnRemoveScore(30);

        ChangeMonitorTextColor(correct: false);

        _tokenSource.Cancel();
    }

    private void CheckSequenceOfSpawning(ProductLevel1 product)
    {
        if (_orderDesired == null)
        {
            print("Order is null");
            return;
        }

        if (product.ProductName != _orderDesired.ProductName) _productsPooledThatAreNotTheOrder++;
        else _productsPooledThatAreNotTheOrder = 0;

        if (_productsPooledThatAreNotTheOrder == 4)
        {
            print("Forced to spawn the desired product");
            product.InitiateProduct(_productsAvailables.First(t => t.ProductName == _orderDesired.ProductName));
            _productsPooledThatAreNotTheOrder = 0;
        }
    }

    private async void ChangeMonitorTextColor(bool correct)
    {
        if (correct) _monitorText.color = _correctColor;
        else _monitorText.color = _wrongColor;

        await Task.Delay(1000);

        if (correct) _monitorText.text = ". . .";
        _monitorText.color = Color.white;
    }
    #endregion

    #region Video
    private void SetVideoPlayer(VideoClip clip)
    {
        _videoPlayer.clip = clip;
        _videoPlayer.Play();
    }

    private void StopVideoPlayer()
    {
        if (_videoPlayer.isPlaying) _videoPlayer.Stop();
    }

    private void ShowOrderVideo()
    {
        //_tokenSource.Cancel();

        SetVideoPlayer(_orderDesired.Clip);
        //_tokenSource = new CancellationTokenSource();

        //try
        //{
        //    await Task.Delay((int)_orderDesired.Clip.length * 1005, _tokenSource.Token);

        //    _videoPlayer.gameObject.SetActive(false);

        //    RepeatVideo();
        //}
        //catch (TaskCanceledException)
        //{
        //    StopVideoPlayer();
        //    _videoPlayer.gameObject.SetActive(false);
        //    _tokenSource = new CancellationTokenSource();
        //    print("Interrupted the first time of video");
        //    return;
        //}
    }

    //private async void RepeatVideo()
    //{
    //    try
    //    {
    //        await Task.Delay(14000, _tokenSource.Token);

    //        SetVideoPlayer(_orderDesired.Clip);

    //        await Task.Delay((int)_orderDesired.Clip.length * 1005, _tokenSource.Token);

    //        _videoPlayer.gameObject.SetActive(false);
    //    }
    //    catch (TaskCanceledException)
    //    {
    //        StopVideoPlayer();
    //        _videoPlayer.gameObject.SetActive(false);
    //        _tokenSource = new CancellationTokenSource();
    //        print("Interrupted the second time of video");
    //        return;
    //    }
    //}
    #endregion

    #region Pool
    private void GrowPool()
    {
        for(int i = 0; i < 6; i++)
        {
            ProductLevel1 instance = Instantiate(_productPrefab, _productsParent);
            AddToPool(instance);
        }
    }

    private void AddToPool(ProductLevel1 instance)
    {
        instance.gameObject.SetActive(false);
        _productsInTreadmill.Enqueue(instance);
    }

    private ProductLevel1 GetFromPool()
    {
        if (_productsInTreadmill.Count == 0)
        {
            print("Queue is empty");
            return null;
        }

        var instance = _productsInTreadmill.Dequeue();
        instance.gameObject.SetActive(true);
        return instance;
    }

    private bool PoolOtherProduct()
    {
        var product = GetFromPool();

        if (product == null) return false;

        product.transform.position = _productsParent.position;
        product.InitiateProduct(_productsAvailables.GetRandomValue());

        CheckSequenceOfSpawning(product);

        return true;
    }
    #endregion
}
