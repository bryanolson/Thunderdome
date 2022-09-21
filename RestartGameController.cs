using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.OnTogglePause += ActivateRestart;
        this.gameObject.SetActive(false);
    }

    private void ActivateRestart(bool paused)
    {
        this.gameObject.SetActive(true);
    }
}
