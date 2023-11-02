using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SecondLevelManager : LevelManagerBase
{
    private List<int> _numbersLeft = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    private bool[] _shelfsCompleted;

    [Header("Prefabs")]
    [SerializeField] private Transform _productsDragDropParent;
    [SerializeField] private ProductLevel2 _productDragDropPrefab;
    [SerializeField] private ShelfOrderUI[] _shelfOrders;

    [Header("Others")]
    [SerializeField] private Sprite[] _numbers;
    [field: SerializeField] public Shelf[] Shelfs { get; private set; }

    private void Start()
    {
        Events.Instance.onGameStart += HandleStartGame;
    }

    #region Handling Events
    [ContextMenu("Star Game Manually")]
    protected override void HandleStartGame()
    {
        Events.Instance.onShelfCompleted += HandleShelfCompletion;

        Audio.StartBackground();

        ProductsAvailables = GameManager.Instance.GetProductsAvailables();

        _shelfsCompleted = new bool[3];
        CreateOrders();
    }

    public void HandleShelfCompletion(int index)
    {
        _shelfsCompleted[index - 1] = true;

        if (_shelfsCompleted.All(completed => completed == true))
        {
            HandleEndGame();
        }
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
        Events.Instance.OnGameEnded();
        Time.timeScale = 1;

        if (VictoryScene == null)
        {
            VictoryScene = FindObjectOfType<VictoryScene>();
        }

        Audio.VictoryVolume();

        VictoryScene.Initiate();

        Events.Instance.onGameStart -= HandleStartGame;
        Events.Instance.onPause -= HandlePause;
        Events.Instance.onShelfCompleted -= HandleShelfCompletion;
    }
    #endregion


    private void CreateOrders()
    {
        List<ProductSO> products = new List<ProductSO>();
        List<int> productsAmounts = new List<int>();

        foreach (var shelf in Shelfs)
        {
            var orders = new List<Order>();
            int amountSum = 0;
            OrdersAvailables = new List<ProductSO>(ProductsAvailables);

            while (amountSum < 9)
            {
                int amountOfProduct = CheckAmountProducts(ref amountSum);

                var productSelected = OrdersAvailables.GetRandomValue(true);

                if (productSelected == null)
                {
                    print("Erro");
                }

                Order newOrder = new Order(productSelected.ProductName, amountOfProduct, productSelected.SpriteSource, null);
                orders.Add(newOrder);
                products.Add(productSelected);
                productsAmounts.Add(amountOfProduct);
            }

            shelf.SetOrders(orders);

            SetOrderInSomeRandomPaper(shelf, orders);
        }

        InstantiateProducts(products, productsAmounts);
    }

    private void SetOrderInSomeRandomPaper(Shelf shelf, List<Order> orders)
    {
        var paperOrder = _shelfOrders.GetRandomValue();
        while (paperOrder.Initiated)
        {
            paperOrder = _shelfOrders.GetRandomValue();
        }

        paperOrder.Initiate(shelf.GetIndex(), orders.ToArray(), _numbers);
    }

    private void InstantiateProducts(List<ProductSO> products, List<int> productsAmounts)
    {
        int steps;
        bool instantiate = true;
        steps = 100;

        while (instantiate && steps > 0)
        {
            if (productsAmounts.All(t => t == 0)) break;

            int indexRandom;
            indexRandom = Random.Range(0, products.Count);

            while (productsAmounts[indexRandom] == 0)
            {
                indexRandom = Random.Range(0, products.Count);
            }

            productsAmounts[indexRandom]--;
            var newProduct = Instantiate(_productDragDropPrefab, _productsDragDropParent);
            newProduct.InitiateProduct(products[indexRandom]);

            steps--;
        }
    }

    private int CheckAmountProducts(ref int amountSum)
    {
        int amountOfProduct = _numbersLeft.GetRandomValue(true);

        if (amountSum + amountOfProduct > 9)
        {
            amountOfProduct = 9 - amountSum;
            amountSum += amountOfProduct;
        }
        else
        {
            amountSum += amountOfProduct;
        }

        return amountOfProduct;
    }
}
