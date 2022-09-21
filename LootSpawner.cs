using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    public GameObject gold;
    public GameObject ammo;
    public float despawnSeconds;
    public float baseScaleModifier = 2;
    public float goldWeight;

    private Queue<KeyValuePair<GameObject, float>> _spawned = new Queue<KeyValuePair<GameObject, float>>();

    void Start()
    {
        GameEvents.current.OnEnemyKilled += spawnRandomLoot;
        GameEvents.current.OnAmmoGone += spawnAmmo;
        GameEvents.current.OnPickup += HandlePickup;
    }

    private void spawnRandomLoot(Transform spawnLocation)
    {
        float choice = Random.Range(0f, 1f);
        spawnLoot(spawnLocation, choice);
    }

    private void spawnAmmo(Transform spawnLocation)
    {
        // 1 is ammo. Might want to use enum if there's an easy way to randomize.
        spawnLoot(spawnLocation, 1);
    }

    private void spawnLoot(Transform spawnLocation, float choice)
    {
        GameObject pickup;
        float scaleModifier;
        //TODO: Different despawn times for different objs?
        float despawnTime = Time.time + despawnSeconds;
        if (choice < goldWeight)
        {
            // TODO: Balance?!
            pickup = gold;
            scaleModifier = 2f;
        }
        else
        {
            pickup = ammo;
            scaleModifier = 11f;
        }

        GameObject newPickup = Instantiate(pickup, spawnLocation.position, Quaternion.identity);
        newPickup.transform.localScale *= scaleModifier * baseScaleModifier;
        _spawned.Enqueue(new KeyValuePair<GameObject, float>(newPickup, despawnTime));
    }

    private void FixedUpdate()
    {
        if (_spawned.Count > 0 && _spawned.Peek().Value < Time.time)
        {
            Destroy(_spawned.Dequeue().Key);
        }

        // This should keep the player from being totally stuck
        // Related code in GameState that will trigger "AmmoGone" when the player runs out of ammo.
        if (GameState.current.CurrentAmmo == 0 && _spawned.Count == 0)
        {
            spawnAmmo(Spawner.GetRandomSpawnOption());
        }
    }

    private void HandlePickup(GameObject pickedUp)
    {
        // Remove the picked up item from the queue
        _spawned = new Queue<KeyValuePair<GameObject, float>>(_spawned.Where(item => item.Key != pickedUp));
        Destroy(pickedUp);
    }
}