using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductBase : MonoBehaviour
{
    protected float _defaultSize;
    protected bool Interactable;

    [SerializeField] protected float Size = 125;

    [SerializeField] protected ProductSO InfoFromSO;
    [SerializeField] protected Image ImageSource;
    [SerializeField] protected RectTransform Rect;
    [SerializeField] protected BoxCollider2D BoxCollider;

    public string ProductName => InfoFromSO.ProductName;

    [ContextMenu("Initiate Product Manually")]
    private void Init()
    {
        if (ImageSource == null) ImageSource = GetComponent<Image>();
        if (Rect == null) Rect = GetComponent<RectTransform>();
        if (BoxCollider == null) BoxCollider = GetComponent<BoxCollider2D>();

        InitiateProduct(InfoFromSO);
    }

    public bool IsInteractable() => Interactable;

    public virtual void InitiateProduct(ProductSO info)
    {
        if (ImageSource == null) ImageSource = GetComponent<Image>();
        if (Rect == null) Rect = GetComponent<RectTransform>();
        if (BoxCollider == null) BoxCollider = GetComponent<BoxCollider2D>();

        InfoFromSO = info;

        AdjustImage();
    }

    protected void AdjustImage()
    {
        ImageSource.sprite = InfoFromSO.SpriteSource;
        ImageSource.SetNativeSize();

        var size = ImageSource.rectTransform.sizeDelta;
        AdjustSize(size);

        size = ImageSource.rectTransform.sizeDelta;
        AdjustPositionOverTreadmill(size);

        BoxCollider.size = size;
    }

    private void AdjustPositionOverTreadmill(Vector2 size)
    {
        Vector3 newPosition = Vector3.zero;
        if (size.y / 2 > 50) newPosition = Vector3.up * size.y / 2.75f;
        else if (size.y / 2 < 50 && size.y / 2 > 25) newPosition = Vector3.up * size.y / 7f;

        transform.localPosition = newPosition;
    }

    private void AdjustSize(Vector2 size)
    {
        float proportion = size.x / size.y;

        if (InfoFromSO.AdjustHorizontally) ImageSource.rectTransform.sizeDelta = new Vector2(Size * proportion, Size);
        else ImageSource.rectTransform.sizeDelta = new Vector2(Size, Size / proportion);
    }
}
