using UnityEngine;

class EnmyCannonBallCollisionTracker : MonoBehaviour
{
	public bool HasCollided { get; set; } = false;
	public void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            HasCollided = true;
			GameEvents.current.PlayerRangedAttacked(this.transform);
            Destroy(gameObject);
            //Debug.Log("cannonball shoudl be gone");

			
			
            
        }
    }
}


