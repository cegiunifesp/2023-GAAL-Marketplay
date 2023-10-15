using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ThirdLevelManager : LevelManagerBase
{
    private int maxAmount = 15;
    private float _maxSize = 150;
    private Transform _productObjectsParent;
    private List<int> _numbersLeft = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };


    [Header("Products")]
    [SerializeField] private List<ProductLevel3> _productsObjects;
    [SerializeField] private PersonOrderUI _personOrderUI;
    [SerializeField] private ProductLevel3 _productPrefab;

    [Header("Others")]
    [SerializeField] private List<Order> _orders;
    [SerializeField] private Basket _basket;

    private void Start()
    {
        Events.Instance.onGameStart += HandleStartGame;
    }


    [ContextMenu("Initiate Manually")]
    protected override void HandleStartGame()
    {
        Events.Instance.onPause += HandlePause;
        Events.Instance.onGameEnded += HandleEndGame;

        ProductsAvailables = new List<ProductSO>();
        ProductsAvailables.AddList(GameManager.Instance.ListProducts[Enums.TypeProducts.Almoco]);
        ProductsAvailables.AddList(GameManager.Instance.ListProducts[Enums.TypeProducts.Cafe]);
        ProductsAvailables.AddList(GameManager.Instance.ListProducts[Enums.TypeProducts.Higiene]);

        _productObjectsParent = _productsObjects[0].transform.parent;

        SetOrder();

        CheckIfThereAreUndesiredProducts();

        _basket.SetOrders(_orders);
    }

    protected override void HandlePause(bool paused)
    {
        if (paused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    protected override void HandleEndGame()
    {
        Events.Instance.onGameStart -= HandleStartGame;
        Events.Instance.onPause -= HandlePause;
        Events.Instance.onGameEnded -= HandleEndGame;

        Time.timeScale = 1;

        VictoryScene.Initiate();
    }

    private void SetOrder()
    {
        _orders = new List<Order>();
        int amountSum = 0;
        var ordersAvailables = GameManager.Instance.GetProductsAvailables();

        while (amountSum < maxAmount)
        {
            int amountOfProduct = CheckAmountProducts(ref amountSum);

            if (amountOfProduct == 0)
            {
                print("Um zerado");
            }

            var productSelectedSO = ordersAvailables.GetRandomValue(true);

            if (productSelectedSO == null)
            {
                print("Erro");
                break;
            }

            Order newOrder = new Order(productSelectedSO.ProductName, amountOfProduct, productSelectedSO.SpriteSource, null);
            _orders.Add(newOrder);

            for(int i = 0; i < amountOfProduct; i++)
            {
                ProductLevel3 newProduct = _productsObjects.GetRandomValue(true);

                var rect = newProduct.GetComponent<RectTransform>().rect;

                newProduct.Size = rect.height > rect.width ? rect.width : rect.height;
                if (newProduct.Size > _maxSize) newProduct.Size = _maxSize;
                newProduct.InitiateProduct(productSelectedSO);
            }
        }

        _personOrderUI.Initiate(_orders.ToArray());
    }

    private void CheckIfThereAreUndesiredProducts()
    {
        Dictionary<string, int> ordersAmount = new Dictionary<string, int>();
        foreach (var order in _orders)
        {
            ordersAmount.Add(order.ProductName, order.ProductAmount);
        }

        foreach (Transform child in _productObjectsParent)
        {
            var product = child.GetComponent<ProductLevel3>();
            string name = product.ProductName;

            if (ordersAmount.ContainsKey(name))
            {
                if (ordersAmount[name] - 1 < 0)
                {
                    var newProduct = ProductsAvailables.GetRandomValue();

                    while (ordersAmount.ContainsKey(newProduct.ProductName))
                    {
                        newProduct = ProductsAvailables.GetRandomValue();
                    }

                    product.InitiateProduct(newProduct);
                }
                else ordersAmount[name]--;
            }

            if (!product.IsInteractable()) product.MakeItInteractable();
        }
    }

    private int CheckAmountProducts(ref int amountSum)
    {
        int amountOfProduct = _numbersLeft.GetRandomValue(true);

        if (amountSum + amountOfProduct > maxAmount)
        {
            amountOfProduct = maxAmount - amountSum;
            amountSum += amountOfProduct;
        }
        else
        {
            amountSum += amountOfProduct;
        }

        return amountOfProduct;
    }
}
