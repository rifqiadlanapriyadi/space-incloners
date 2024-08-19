using UnityEngine;

public class ExplosionAnimationController : MonoBehaviour
{
    public Fireball fireball;

    public void HandleFinishExplosion()
    {
        fireball.HandleFinishExplosion();
    }
}
