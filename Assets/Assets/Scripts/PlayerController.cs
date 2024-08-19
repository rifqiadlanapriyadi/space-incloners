using UnityEngine;

public class PlayerController : Ship
{
    public int startingLives = 3;
    private int _lives;

    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode shootKey = KeyCode.Space;

    public float velocity = 3f;
    private Rigidbody2D _rb;
    private Collider2D _col;

    public float shootIntervalSeconds = 1f;
    private float lastShotTime;

    private Vector2 _screenBounds;
    private float _halfWidth;

    private InputManager _im;
    private UIManager _ui;

    protected override void Awake()
    {
        base.Awake();
        _im = InputManager.instance;
        _ui = UIManager.instance;
    }

    protected override void Start()
    {
        base.Start();

        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        lastShotTime = Time.time - shootIntervalSeconds;

        Camera mainCamera = Camera.main;
        _screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        _halfWidth = _col.bounds.size.x / 2;
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
    {
        int velMultiplier = 0;
        if (_im.IsPressed(leftKey)) velMultiplier -= 1;
        if (_im.IsPressed(rightKey)) velMultiplier += 1;
        _rb.velocity = Vector2.right * velocity * velMultiplier;
    }

    void HandleShooting()
    {
        if (_im.IsPressed(shootKey) && Time.time - lastShotTime >= shootIntervalSeconds)
        {
            lastShotTime = Time.time;
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject fireball = BulletPool.instance.GetNextFire(team);
        fireball.transform.position = firePoint.transform.position;
    }

    void LateUpdate()
    {
        HandleScreenClamp();
    }

    void HandleScreenClamp()
    {
        Vector2 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, -_screenBounds.x + _halfWidth, _screenBounds.x - _halfWidth);
        transform.position = viewPos;
    }

    public override void HandleHit()
    {
        Lives--;
        if (Lives <= 0)
            HandleDeath();
        _ui.UpdateLives(Lives);
    }

    void HandleDeath()
    {
        _as.clip = explosionSound;
        _as.Play();
        _rb.simulated = false;
        _col.enabled = false;
        animator.SetTrigger("Explode");
        _gm.HandlePlayerDeath();
    }

    public void Reset(int lives)
    {
        Lives = lives;
        _ui.UpdateLives(Lives);
        animator.SetTrigger("Revive");
    }

    public void Reset()
    {
        Lives = startingLives;
        _ui.UpdateLives(Lives);
        animator.SetTrigger("Revive");
    }

    public int Lives
    {
        get { return _lives; }
        set { _lives = value; }
    }
}
