using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SecondLevelManager : MonoBehaviour
{
    private List<int> _numbersLeft = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    private List<ProductSO> _productsAvailables;
    private List<ProductSO> _ordersAvailables;

    [Header("Prefabs")]
    [SerializeField] private Transform _productsDragDropParent;
    [SerializeField] private ProductLevel2 _productDragDropPrefab;
    [SerializeField] private ShelfOrderUI[] _shelfOrders;

    [Header("Others")]
    [SerializeField] private Sprite[] _numbers;
    [field: SerializeField] public Shelf[] Shelfs { get; private set; }

    void Start()
    {
        Events.onGameStart += HandleStart;

        _productsAvailables = GameManager.Instance.GetProductsAvailables();
    }

    public void HandleStart()
    {
        CreateOrders();
    }


    [ContextMenu("Create Orders Manually")]
    private void CreateOrders()
    {
        List<ProductSO> products = new List<ProductSO>();
        List<int> productsAmounts = new List<int>();

        foreach (var shelf in Shelfs)
        {
            var orders = new List<Order>();
            int amountSum = 0;
            _ordersAvailables = new List<ProductSO>(_productsAvailables);

            while (amountSum < 9)
            {
                int amountOfProduct = CheckAmountProducts(ref amountSum);

                var productSelected = _ordersAvailables.GetRandomValue(true);

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
        var shelfOrder = _shelfOrders.GetRandomValue();
        while (shelfOrder.Initiated)
        {
            shelfOrder = _shelfOrders.GetRandomValue();
        }

        shelfOrder.Initiate(shelf.GetIndex(), orders.ToArray(), _numbers);
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

        print(steps);
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
