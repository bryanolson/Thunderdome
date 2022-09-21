using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public GameObject explosion;
    public GameObject destroyedGameObject;
    private AudioSource _explosionAudioSource;
    private float _basePitch = 0.5f;

    private void Awake()
    {
        _explosionAudioSource = GameObject.Find("ExplosionSound").GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision c)
    {
        if (gameObject.CompareTag("WhiskyBottle"))
        {
            if (c.gameObject.CompareTag("Cannonball") || c.gameObject.CompareTag("Player") || 
                c.gameObject.CompareTag("Enemy") || c.gameObject.CompareTag("Terrain"))
            { 
                TriggerExplosion(c.gameObject);
            }
        }
        else
        {
            if (c.gameObject.CompareTag("Cannonball") || c.gameObject.CompareTag("Player") || 
                c.gameObject.CompareTag("Enemy"))
            {
                TriggerExplosion(c.gameObject);
            }    
        }
    }

    private void TriggerExplosion(GameObject gameObj)
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Instantiate(destroyedGameObject, transform.position, transform.rotation);
        _explosionAudioSource.volume = _basePitch;
        _explosionAudioSource.Play();
        // this gameObject refers to explosive object not the one colliding.
        Destroy(gameObject);
        
        if (gameObj.CompareTag("Player"))
        {
            GameEvents.current.ExplosionDamage(gameObj);
        }
        else if (gameObj.CompareTag("Enemy"))
        {
            var enemyMovement = gameObj.GetComponent<EnemyMovement>();
            enemyMovement.RemoveHealthAndReportIfAlive(25);
        }
    }
}
