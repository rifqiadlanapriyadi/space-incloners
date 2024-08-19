using UnityEngine;

public enum Team
{
    Friend,
    Enemy
}

public class Ship : MonoBehaviour
{
    public Team team;

    public GameObject firePoint;

    public Animator animator;
    public AudioClip explosionSound;

    protected AudioSource _as;

    protected GameManager _gm;

    protected virtual void Awake()
    {
        _gm = GameManager.instance;
    }

    protected virtual void Start()
    {
        _as = GetComponent<AudioSource>();
    }

    public virtual void HandleHit()
    {
        Debug.Log("HIT!");
    }

    public virtual void HandleFinishExplosion()
    {
        Destroy(gameObject);
    }
}
