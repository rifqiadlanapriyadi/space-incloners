using UnityEngine;

public class ShipExplosionAnimationController : MonoBehaviour
{
    public Ship ship;

    public void HandleFinishExplosion()
    {
        ship.HandleFinishExplosion();
    }
}
