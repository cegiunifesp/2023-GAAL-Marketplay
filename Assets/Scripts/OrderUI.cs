using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    [SerializeField] private float _size;
    [field: SerializeField] public Image NumberSource { get; private set; }
    [field: SerializeField] public Image ProductSource {  get; private set; }

    public void Initiate(Sprite number, Sprite product)
    {
        NumberSource.sprite = number;
        ProductSource.sprite = product;

        AdjustSize();
    }

    [ContextMenu("Adjust Image Manually")]
    private void AdjustSize()
    {
        ProductSource.SetNativeSize();

        var size = ProductSource.rectTransform.sizeDelta;
        float proportion = size.x / size.y;

        if (proportion < 1) ProductSource.rectTransform.sizeDelta = new Vector2(_size * proportion, _size);
        else ProductSource.rectTransform.sizeDelta = new Vector2(_size, _size / proportion);
    }
}
