public class GoldPickup : PickupLogic
{
    public override void PlayerPickupCallback()
    {
        GameEvents.current.GoldCollected(gameObject.transform);
    }
}