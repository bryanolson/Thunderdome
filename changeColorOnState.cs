using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeColorOnState : MonoBehaviour
{
    // Start is called before the first frame update
    private BehaviorState behaviorState ; 
    private Material emotionalMaterial ; 

    private GameObject emotionLight ; 
    void Start()
    {
        emotionLight = this.transform.Find("emoteLight").gameObject ; 
        emotionalMaterial = emotionLight.GetComponent<Renderer>().material ; 
    }

    // Update is called once per frame
    void Update()
    {
        //get enum state
        behaviorState = this.GetComponent<EnemyMovement>().currentBehaviorState;

        switch(behaviorState)
        {
            case BehaviorState.Attack :
                emotionalMaterial.color = Color.red; 
                break;
            case BehaviorState.Evade :
                emotionalMaterial.color = Color.blue; 
                break;
            case BehaviorState.Patrol :
                emotionalMaterial.color = Color.green; 
                break;
            case BehaviorState.Wait :
                emotionalMaterial.color = Color.gray; 
                break;
            case BehaviorState.Steal:
                emotionalMaterial.color = Color.magenta;
                break;
        }
        


       // Debug.Log(behaviorState);
        //change color based off enum state

    }
}
