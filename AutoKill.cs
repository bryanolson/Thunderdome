using UnityEngine;

public class AutoKill : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GameEvents.current.PlayerKilled();
        }
    }
}