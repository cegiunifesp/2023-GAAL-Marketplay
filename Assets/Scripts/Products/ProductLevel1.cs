using UnityEngine;
using UnityEngine.Video;

public class ProductLevel1 : ProductBase
{
    private const float SpeedOverTreadmill = 1;

    private bool _move;
    private bool _isCorrectProduct = false;

    public VideoClip Clip => InfoFromSO.Clip;


    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private GameObject _check;

    [Header("AudioClips")]
    [SerializeField] private AudioClip _correctProduct;
    [SerializeField] private AudioClip _incorrectProduct;
    [SerializeField] private AudioClip _inTreadmill;
    [SerializeField] private AudioClip _inCart;

    private void Start()
    {
        Events.Instance.onGameEnded += HandleGameEnded;
    }

    private void Update()
    {
        if (!_move) return;

        transform.Translate(Vector3.right * Time.deltaTime * SpeedOverTreadmill);
    }


    public override void InitiateProduct(ProductSO info)
    {
        base.InitiateProduct(info);

        _move = true;
        Interactable = true;
        _isCorrectProduct = false;
        _check.SetActive(false);
    }

    protected override void AdjustImage()
    {
        base.AdjustImage();

        var size = ImageSource.rectTransform.sizeDelta;
        AdjustPositionOverTreadmill(size);
    }

    private void AdjustPositionOverTreadmill(Vector2 size)
    {
        Vector3 newPosition = Vector3.zero;
        if (size.y / 2 > 50) newPosition = Vector3.up * size.y / 2.75f;
        else if (size.y / 2 < 50 && size.y / 2 > 25) newPosition = Vector3.up * size.y / 7f;

        transform.localPosition = newPosition;
    }

    private void HandleGameEnded()
    {
        _move = false;
        Events.Instance.onGameEnded -= HandleGameEnded;

    }

    public void ProductIncorrect()
    {
        transform.eulerAngles = Vector3.forward * 10;
        LeanTween.rotateZ(gameObject, -10, 0.2f).setLoopPingPong(1).setOnComplete(() =>
        {
            LeanTween.rotateZ(gameObject, 0, 0.2f);
        });

        _rigidbody.AddForce(Vector2.up * Random.Range(3, 7), ForceMode2D.Impulse);
        BoxCollider.isTrigger = true;
        Interactable = false;

        AudioManager.OnPlaySFX(_incorrectProduct);
    }

    public void ProductCorrect()
    {
        _check.SetActive(true);
        Interactable = false;
        _isCorrectProduct = true;

        AudioManager.OnPlaySFX(_correctProduct);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Limit"))
        {
            Events.Instance.OnEnqueueProduct(this);
            _move = false;
        }

        if (collision.gameObject.CompareTag("Treadmill"))
        {
            AudioManager.OnPlaySFX(_inTreadmill);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Treadmill"))
        {
            Interactable = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Limit"))
        {
            Events.Instance.OnEnqueueProduct(this);
            BoxCollider.isTrigger = false;
            _move = false;
        }
        else if (collision.gameObject.CompareTag("Shopping Cart"))
        {
            if (_isCorrectProduct)
            {
                collision.gameObject.GetComponent<ShoppingCart>().AddItem(this);
                AudioManager.OnPlaySFX(_inCart);
                Events.Instance.OnEnqueueProduct(this);
                _move = false;
            }
        }
    }
}
