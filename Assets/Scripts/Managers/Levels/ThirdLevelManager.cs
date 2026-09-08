using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThirdLevelManager : LevelManagerBase
{
    private const int MaxVisibleProducts = 24;
    private const int MaxOrderItems = 12;
    private const int MaxOrderTypes = 3;

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

        Audio.StartBackground();

        ProductsAvailables = new List<ProductSO>();
        ProductsAvailables.AddList(GameManager.Instance.ListProducts[Enums.TypeProducts.Almoco]);
        ProductsAvailables.AddList(GameManager.Instance.ListProducts[Enums.TypeProducts.Cafe]);
        ProductsAvailables.AddList(GameManager.Instance.ListProducts[Enums.TypeProducts.Higiene]);

        SetOrder();

        _personOrderUI.Initiate(_orders.ToArray());
        _basket.SetOrders(_orders);
    }

    protected override void HandlePause(bool paused)
    {
        Time.timeScale = paused ? 0 : 1;
    }

    protected override void HandleEndGame()
    {
        Events.Instance.onGameStart -= HandleStartGame;
        Events.Instance.onPause -= HandlePause;
        Events.Instance.onGameEnded -= HandleEndGame;

        Time.timeScale = 1;

        Audio.VictoryVolume();

        VictoryScene.Initiate();
    }

    private void SetOrder()
    {
        _orders = new List<Order>();

        List<ProductLevel3> slots = _productsObjects.Where(p => p != null).ToList();
        Shuffle(slots);

        for (int i = 0; i < slots.Count; i++)
        {
            bool visible = i < MaxVisibleProducts;
            slots[i].gameObject.SetActive(visible);
        }

        slots.RemoveAll(p => !p.gameObject.activeSelf);

        List<ProductSO> ordersAvailables = GameManager.Instance.GetProductsAvailables();
        List<int> numbersLeft = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        int distractorSlots = Mathf.Max(4, slots.Count / 3);
        int maxOrderItems = Mathf.Min(MaxOrderItems, slots.Count - distractorSlots);
        if (maxOrderItems < 3) maxOrderItems = Mathf.Max(1, slots.Count / 2);

        int amountSum = 0;
        while (amountSum < maxOrderItems && _orders.Count < MaxOrderTypes && ordersAvailables.Count > 0)
        {
            int amountOfProduct = PickOrderAmount(numbersLeft, maxOrderItems - amountSum);
            if (amountOfProduct <= 0) break;

            ProductSO productSelectedSO = ordersAvailables.GetRandomValue(true);
            if (productSelectedSO == null) break;

            _orders.Add(new Order(productSelectedSO.ProductName, amountOfProduct, productSelectedSO.SpriteSource));
            amountSum += amountOfProduct;
        }

        int slotIndex = 0;
        foreach (Order order in _orders)
        {
            ProductSO productSO = ProductsAvailables.First(p => p.ProductName == order.ProductName);
            for (int i = 0; i < order.ProductAmount && slotIndex < slots.Count; i++)
            {
                slots[slotIndex].InitiateProduct(productSO);
                slotIndex++;
            }
        }

        List<ProductSO> distractors = ProductsAvailables
            .Where(p => _orders.All(order => order.ProductName != p.ProductName))
            .ToList();

        while (slotIndex < slots.Count)
        {
            ProductSO distractor = distractors.Count > 0
                ? distractors.GetRandomValue()
                : ProductsAvailables.GetRandomValue();

            slots[slotIndex].InitiateProduct(distractor);
            slotIndex++;
        }
    }

    private static int PickOrderAmount(List<int> numbersLeft, int remaining)
    {
        if (remaining <= 0) return 0;

        int amount = numbersLeft.Count > 0 ? numbersLeft.GetRandomValue(true) : Random.Range(1, Mathf.Min(4, remaining) + 1);
        if (amount <= 0) amount = 1;
        if (amount > remaining) amount = remaining;
        return amount;
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
