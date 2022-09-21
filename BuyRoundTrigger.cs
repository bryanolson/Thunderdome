using UnityEngine;

public class BuyRoundTrigger : MonoBehaviour
{
    private GameObject Player;
    public bool _buyingPhase { get; private set; } = false;

    private void Start()
    {
        //Get Player game object
        Player = GameObject.Find("ctrlSphere");
        GameEvents.current.OnLevelComplete += HandleLevelComplete;
    }

    private void HandleLevelComplete()
    {
        _buyingPhase = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If other is player car
        if (other == Player.GetComponent<Collider>())
        {
            if (_buyingPhase)
            {
                //GameEvent for Buy Phase = true
                GameEvents.current.ToggleBuyRound(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If other is player car
        if (other == Player.GetComponent<Collider>())
        {
            _buyingPhase = false;
            //GameEvent for Buy Phase = false
            GameEvents.current.ToggleBuyRound(false);
        }
    }
}