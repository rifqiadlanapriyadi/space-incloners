using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float velocity = 5f;
    public Team originTeam;
    public Team targetTeam;
    public Animator animator;
    public AudioClip shootSound;
    public AudioClip hitSound;

    private AudioSource _as;
    private Rigidbody2D _rb;
    private Collider2D _col;

    private void Awake()
    {
        _as = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        if (_as != null && shootSound != null)
        {
            _as.clip = shootSound;
            _as.Play();
        }

        _rb.simulated = true;
        _col.enabled = true;
        _rb.velocity = Vector2.up * velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ship ship = collision.gameObject.GetComponent<Ship>();
        if (ship != null && ship.team == targetTeam)
        {
            if (_as != null && hitSound != null)
            {
                _as.clip = hitSound;
                _as.Play();
            }
            ship.HandleHit();
            HandleHit();
        } else if (collision.CompareTag("Bounds"))
            HandleHit();
    }

    void HandleHit()
    {
        _rb.simulated = false;
        _col.enabled = false;
        animator.SetTrigger("Explode");
    }

    public void HandleFinishExplosion()
    {
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        BulletPool.instance.Enqueue(gameObject);
    }
}
