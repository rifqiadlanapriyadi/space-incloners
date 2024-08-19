using UnityEngine;

public class EnemyController : Ship
{
    public override void HandleHit()
    {
        animator.SetTrigger("Explode");
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Collider2D>().enabled = false;
        if (_as.enabled)
        {
            _as.clip = explosionSound;
            _as.Play();
        }
        EnemiesController.instance.HandleDestroyed(gameObject);
    }

    public void Shoot()
    {
        GameObject fireball = BulletPool.instance.GetNextFire(team);
        fireball.transform.position = firePoint.transform.position;
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.CompareTag("Player Zone")) _gm.HandleEnemyEnterPlayerZone();
    }
}
