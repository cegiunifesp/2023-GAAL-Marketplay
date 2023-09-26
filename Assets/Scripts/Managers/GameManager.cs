using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(Resources.Load("Prefabs/Game Manager")).GetComponent<GameManager>();
            }
            return _instance;
        }
        set
        {
            if (_instance == null) _instance = value;
        }
    }

    [SerializeField] private List<ProductSO> _breakfastProducts;
    [SerializeField] private List<ProductSO> _lunchProducts;
    [SerializeField] private List<ProductSO> _hygieneProducts;

    public Dictionary<Enums.TypeProducts, List<ProductSO>> ListProducts { get; private set; }

    public Enums.TypeProducts TypeSelected { get; private set; }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _instance = this;

        ListProducts = new Dictionary<Enums.TypeProducts, List<ProductSO>> { { Enums.TypeProducts.Cafe, _breakfastProducts},
            { Enums.TypeProducts.Almoco, _lunchProducts }, {Enums.TypeProducts.Higiene,  _hygieneProducts } };
    }

    public List<ProductSO> GetProductsAvailables()
    {
        return ListProducts[TypeSelected];
    }

    public void SetTypeSelected(int index)
    {
        if (index == -1)
        {
            TypeSelected = (Enums.TypeProducts)Random.Range(0, 3);
        }
        else
        {
            TypeSelected = (Enums.TypeProducts)index;
        }
    }
}
