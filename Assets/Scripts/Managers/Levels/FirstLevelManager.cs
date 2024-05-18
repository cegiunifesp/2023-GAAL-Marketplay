using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

public class FirstLevelManager : LevelManagerBase
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
    [SerializeField] private TextMeshProUGUI _leftOrdersTx;

    private int _productsPooledThatAreNotTheOrder;
    private float _timeToActiveProducts;
    private bool _selected;

    private Queue<ProductLevel1> _productsInTreadmill = new Queue<ProductLevel1>();
    private Order _orderDesired;

    private void Start()
    {
        Events.Instance.onGameStart += HandleStartGame;
        //_videoPlayer.gameObject.SetActive(false);
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

    private void ChangeShelfSprite()
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
    }

    #region Handling Events
    [ContextMenu("Start Manually")]
    protected override void HandleStartGame()
    {
        Events.Instance.onPause += HandlePause;
        Events.Instance.onEnqueueProduct += HandleEnqueueProduct;
        Events.Instance.onProductSelected += HandleProductSelected;

        ProductsAvailables = GameManager.Instance.GetProductsAvailables();
        OrdersAvailables = new List<ProductSO>(ProductsAvailables);

        Audio.StartBackground();

        ChangeShelfSprite();

        GrowPool();

        Invoke("NewOrder", 1.5f);
        _timeToActiveProducts = Time.time;
        _gameStarted = true;
    }

    protected override void HandlePause(bool paused)
    {
        if (paused)
        {
            Time.timeScale = 0;
            VideoManager.Instance.PauseVideo();
        }
        else
        {
            Time.timeScale = 1;
            VideoManager.Instance.UnpauseVideo();
        }
    }

    private void HandleProductSelected(ProductLevel1 product)
    {
        if (!product.IsInteractable()) return;

        _monitorText.text = product.ProductName;

        if (product.ProductName == _orderDesired.ProductName)
        {
            if (_selected) return;

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
    protected override void HandleEndGame()
    {
        Events.Instance.onGameStart -= HandleStartGame;
        Events.Instance.onPause -= HandlePause;
        Events.Instance.onEnqueueProduct -= HandleEnqueueProduct;
        Events.Instance.onProductSelected -= HandleProductSelected;

        Events.Instance.OnGameEnded();
        _gameEnded = true;

        VideoManager.Instance.StopVideo();

        Audio.VictoryVolume();

        VictoryScene.Initiate();
    }
    #endregion

    #region Order
    private void NewOrder()
    {
        if (OrdersAvailables.Count == 0)
        {
            HandleEndGame();
            return;
        }

        var productDesired = OrdersAvailables.GetRandomValue();

        _orderDesired = new Order(productDesired.ProductName, 1, productDesired.SpriteSource);

        OrdersAvailables.Remove(OrdersAvailables.First(t => t.ProductName == productDesired.ProductName));

        if (OrdersAvailables.Count == 0)
        {
            _leftOrdersTx.text = $"Resta {OrdersAvailables.Count+1} produto";
        }
        else
        {
            _leftOrdersTx.text = $"Restam {OrdersAvailables.Count+1} produtos";
        }

        _monitorText.color = Color.white;
        _selected = false;

        VideoManager.Instance.NewVideo(productDesired.VideoInfo);
    }

    private void CorrectOrder()
    {
        _selected = true;
        Events.Instance.OnAddScore(100);
        ChangeMonitorTextColor(correct: true);

        destroyCancellationToken.ThrowIfCancellationRequested();

        Invoke("NewOrder", 2);
    }

    private void IncorrectOrder()
    {
        Events.Instance.OnRemoveScore(30);

        ChangeMonitorTextColor(correct: false);

        destroyCancellationToken.ThrowIfCancellationRequested();
    }

    private void CheckSequenceOfSpawning(ProductLevel1 product)
    {
        if (_orderDesired == null)
        {
            //print("Order is null");
            return;
        }

        if (product.ProductName != _orderDesired.ProductName) _productsPooledThatAreNotTheOrder++;
        else _productsPooledThatAreNotTheOrder = 0;

        if (_productsPooledThatAreNotTheOrder == 4)
        {
            //print("Forced to spawn the desired product");
            product.InitiateProduct(ProductsAvailables.First(t => t.ProductName == _orderDesired.ProductName));
            _productsPooledThatAreNotTheOrder = 0;
        }
    }

    private async void ChangeMonitorTextColor(bool correct)
    {
        if (correct) _monitorText.color = _correctColor;
        else _monitorText.color = _wrongColor;

        await UniTask.Delay(1000, false, PlayerLoopTiming.Update, destroyCancellationToken);

        _monitorText.text = ". . .";
        _monitorText.color = Color.white;
    }
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
        product.InitiateProduct(ProductsAvailables.GetRandomValue());

        CheckSequenceOfSpawning(product);

        return true;
    }
    #endregion
}
