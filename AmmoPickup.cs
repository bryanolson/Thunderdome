public class AmmoPickup : PickupLogic
{
    public override void PlayerPickupCallback()
    {
        GameEvents.current.AmmoCollected(gameObject.transform);
    }
}