using UnityEngine;

class CannonBallCollisionTracker : MonoBehaviour
{
    public bool HasCollided { get; set; } = false;

    private void OnCollisionEnter(Collision collision)
    {
        GameEvents.current.ProjectileCollision(transform);
    }
}