using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ProductBase : MonoBehaviour
{
    protected float _defaultSize;
    protected bool Interactable;

    public float Size { get; set; } = 125;

    [SerializeField] protected ProductSO InfoFromSO;
    [SerializeField] protected Image ImageSource;
    [SerializeField] protected RectTransform Rect;
    [SerializeField] protected BoxCollider2D BoxCollider;

    public string ProductName => InfoFromSO.ProductName;
    public Sprite ProductSprite => InfoFromSO.SpriteSource;


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

    protected virtual void AdjustImage()
    {
        ImageSource.sprite = InfoFromSO.SpriteSource;
        ImageSource.SetNativeSize();

        var size = ImageSource.rectTransform.sizeDelta;
        AdjustSize(size);

        BoxCollider.size = ImageSource.rectTransform.sizeDelta;
    }

    private void AdjustSize(Vector2 size)
    {
        float proportion = size.x / size.y;

        if (InfoFromSO.AdjustHorizontally) ImageSource.rectTransform.sizeDelta = new Vector2(Size * proportion, Size);
        else ImageSource.rectTransform.sizeDelta = new Vector2(Size, Size / proportion);
    }

    public Vector2 GetSizeVector2()
    {
        return ImageSource.rectTransform.sizeDelta;
    }

    public void ResizeProportionally(float proportion)
    {
        var size = ImageSource.rectTransform.sizeDelta * proportion;
        AdjustSize(size);

        BoxCollider.size = ImageSource.rectTransform.sizeDelta;
    }
}
