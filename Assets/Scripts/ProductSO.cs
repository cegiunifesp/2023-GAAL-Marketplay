using UnityEngine.Video;
using UnityEngine;

[CreateAssetMenu(fileName = "New Product", menuName = "SO/Products")]
public class ProductSO : ScriptableObject
{
    [field: SerializeField] public string ProductName { get; private set; }
    [field: SerializeField] public Enums.TypeProducts TypeProduct { get; private set; }
    [field: SerializeField] public Sprite SpriteSource { get; private set; }
    [field: SerializeField] public VideoClip Clip { get; private set; }

    [field: SerializeField, Tooltip("True ( Y = Constant ) / False ( X = Constant )")] public bool AdjustHorizontally { get; private set; }
}
