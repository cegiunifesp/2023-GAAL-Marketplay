using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] private float _swingIntensity;
    [SerializeField] private Animator _anim;
    [SerializeField] private Collider2D _scannerCollider;

    private bool _stopWorking;
    private Vector3 _lastMousePosition;

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
        if (_scannerCollider == null) _scannerCollider = GetComponent<Collider2D>();

        Events.Instance.onPause += HandlePause;
        Events.Instance.onGameEnded += HandleGameEnded;
    }

    private void Update()
    {
        if (_stopWorking) return;

        Vector3 currentMousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousedelta = currentMousePosition - _lastMousePosition;

        float rotationAmount = -mousedelta.x * _swingIntensity;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationAmount);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        _lastMousePosition = currentMousePosition;

        Vector2 newPosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = newPosition;

        if (Input.GetMouseButtonDown(0))
        {
            TryScanProductAtCursor();
        }
    }

    private void TryScanProductAtCursor()
    {
        if (_scannerCollider == null) return;

        Physics2D.SyncTransforms();

        float radius = Mathf.Max(_scannerCollider.bounds.extents.x, _scannerCollider.bounds.extents.y);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        ProductLevel1 closestProduct = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null || hit == _scannerCollider) continue;
            if (!hit.CompareTag("Product")) continue;
            if (!hit.TryGetComponent(out ProductLevel1 product)) continue;
            if (!product.IsInteractable()) continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestProduct = product;
            }
        }

        if (closestProduct == null) return;

        Events.Instance.OnProductSelected(closestProduct);
        _anim.SetTrigger("Scan");
    }

    private void HandlePause(bool paused)
    {
        _stopWorking = paused;
    }

    private void HandleGameEnded()
    {
        _stopWorking = true;
    }
}
