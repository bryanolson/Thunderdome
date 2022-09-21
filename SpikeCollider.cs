using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCollider : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        var other = collision.collider;
        if (other.CompareTag("Player"))
        {
            //todo: minor health deficit here?
            GameEvents.current.PlayerCollidedWithSpike();            
        }        
    }
}
