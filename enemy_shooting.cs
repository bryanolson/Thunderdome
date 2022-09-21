using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_shooting : MonoBehaviour
{
    // Start is called before the first frame update
    public object shell; // the cannonball or cannonball variant
    public float rotatespeed = 75.0f;
    private Transform target; // the player or object to be shot at 

    public Transform gunNeck; // we could potentially use multiple guns.
    public Transform gunNose;
    public float rateOfFire = .5f; // how fast it will shoot at you should probably be slow
    private float intrPeriod = 1f;

    public float accuracy = 5f ; // scatter in the shot 

    public GameObject cannonball;

    public Transform shotPoint;

    public float blastPower = 5;

    
    private BehaviorState behaviorState ; 
    


    void Start()
    {
          
    }

    // Update is called once per frame
    void Update()
    {
        target = GameObject.Find("Player").transform;
        behaviorState = this.GetComponent<EnemyMovement>().currentBehaviorState;
        //Debug.Log(behaviorState);
        switch (behaviorState)
        {
            case BehaviorState.Attack:
            Vector3 targetDir = target.position - gunNeck.position;
            targetDir.y = 0; 
            Vector3 tranf = Vector3.RotateTowards(gunNeck.forward, targetDir, rotatespeed * Time.deltaTime, 0.0f); 
            gunNeck.rotation = Quaternion.LookRotation(tranf);
            gunNeck.transform.Rotate(0,0,-90);
            if (Time.time > intrPeriod ) 
            {
                intrPeriod =  Time.time + rateOfFire;  //can add a little randomness, not sure if needed

                GameObject createdCannonball = Instantiate(cannonball, shotPoint.position, shotPoint.rotation);
                createdCannonball.GetComponent<Rigidbody>().velocity = shotPoint.transform.forward * blastPower * 15f;
                // if the cannonball doesn't hit anything it will self destroy after 2 seconds.
                Destroy(createdCannonball, 2.0f);
                }
        
                break;
            case BehaviorState.Patrol:
            gunNeck.rotation = Quaternion.LookRotation(Vector3.RotateTowards(gunNeck.forward, this.transform.forward, rotatespeed/2 * Time.deltaTime, 0.0f));
            gunNeck.transform.Rotate(0,0,-90);
                break;
            case BehaviorState.Evade:
            gunNeck.rotation = Quaternion.LookRotation(Vector3.RotateTowards(gunNeck.forward, this.transform.forward, rotatespeed/2 * Time.deltaTime, 0.0f));
            gunNeck.transform.Rotate(0,0,-90);
                break;
                
        }
        //var lookPos = target.position - gunNeck.position;
        //lookPos.x = 0;
        //lookPos.x = 0;
        //var rotation = Quaternion.LookRotation(lookPos);
        //rotation.y += 45 ; 
        //gunNeck.rotation = Quaternion.Slerp(gunNeck.rotation, rotation, Time.deltaTime * .1f);
        

        // Vector3 noseTarget = new Vector3( target.transform.position.x, target.position.y, target.position.z ) ;
        // gunNose.transform.Rotate(0,0,-90);
        // gunNose.LookAt(noseTarget); 
        
        // gunNeck.transform.eulerAngles = new Vector3(gunNeck.transform.,0,0);
        // fixes the 90 degree off rotation.
        
        
        // if (Time.time > intrPeriod ) {
        // intrPeriod +=  rateOfFire;  //can add a little randomness, not sure if needed

        //  GameObject createdCannonball = Instantiate(cannonball, shotPoint.position, shotPoint.rotation);
        //     createdCannonball.GetComponent<Rigidbody>().velocity =
        //         shotPoint.transform.forward * blastPower * 15f;
        //     // if the cannonball doesn't hit anything it will self destroy after 2 seconds.
        //     Destroy(createdCannonball, 5.0f);
        // }

            //PewPew(createdCannonball.transform.position); //plays the sound
    }
    
}
