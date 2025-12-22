using UnityEngine;

public class Treadmill : MonoBehaviour
{
    [SerializeField] private float _limitX = 260;
    [SerializeField] private float _speedRolling = 1;

    [SerializeField] private Transform _child;

    [SerializeField] private RectTransform _rectCollider;
    [SerializeField] private BoxCollider2D _collider;

    private bool _stopRolling;
    private Vector3 _initialPosition;

    private AudioSource _audioSource;

    private void Start()
    {
        _collider.size = new Vector2(_rectCollider.rect.width, _rectCollider.rect.height);

        _initialPosition = _child.position;

        _audioSource = GetComponent<AudioSource>();

        Events.Instance.onPause += Paused;
        Events.Instance.onGameStart += GameStarted;
        Events.Instance.onGameEnded += HandleGameEnded;
    }

    private void OnDisable()
    {
        if (Events.Instance == null) return;

        Events.Instance.onPause -= Paused;
        Events.Instance.onGameStart -= GameStarted;
        Events.Instance.onGameEnded -= HandleGameEnded;
    }

    // Update is called once per frame
    void Update()
    {
        if (_stopRolling) return;

        _child.position += Vector3.right * Time.deltaTime * _speedRolling;
        if (_child.localPosition.x > _limitX)
        {
            _child.position = _initialPosition;
        }
    }

    private void GameStarted()
    {
        _audioSource.Play();
    }

    private void Paused(bool value)
    {
        if (value)
        {
            _stopRolling = true;
            _audioSource.Stop();
        }
        else
        {
            _stopRolling = false;
            _audioSource.Play();
        }
    }

    private void HandleGameEnded()
    {
        _stopRolling = true;
        _audioSource.Stop();
    }
}
