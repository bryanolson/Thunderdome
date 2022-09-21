using UnityEngine;

public abstract class PickupLogic : MonoBehaviour
{
    private bool _pickedUp;

    void Start()
    {
        GameEvents.current.OnLevelComplete += LevelComplete;
    }

    private void LevelComplete()
    {
        Pickup(true);
    }

    public void Pickup(bool wasPlayer)
    {
        //Needs local pickup tracking to avoid multiple collisions causing issues 
        if (!_pickedUp)
        {
            _pickedUp = true;
            if (wasPlayer)
            {
                PlayerPickupCallback();
            }

            GameEvents.current.Pickup(gameObject);
        }
    }

    private void OnDestroy()
    {
        GameEvents.current.OnLevelComplete -= LevelComplete;
    }

    private void OnTriggerEnter(Collider collider)
    {
        bool wasPlayer = collider.CompareTag("Player");
        if (wasPlayer || collider.CompareTag("Enemy"))
        {
            //TODO: Maybe add a boolean to determine what to do based on if the event is triggered from auto, vs actual collect?
            Pickup(wasPlayer);
        }
    }

    public abstract void PlayerPickupCallback();
}