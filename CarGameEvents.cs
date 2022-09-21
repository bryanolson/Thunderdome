using UnityEngine;

public class CarGameEvents : MonoBehaviour
{
    private GameObject _buySpawnPoint;
    private GameObject _carController;

    public bool debugTriggerSpawnPoint;

    void Start()
    {
        GameEvents.current.OnLevelComplete += HandleLevelComplete;
        _buySpawnPoint = GameObject.Find("BuySpawnPoint");
        _carController = GameObject.Find("ctrlSphere");
        // Debug.Log(_buySpawnPoint.transform.position);
    }

    private void Update()
    {
        if (debugTriggerSpawnPoint)
        {
            HandleLevelComplete();
        }
    }

    void HandleLevelComplete()
    {
        Debug.Log("Triggered CarGameEvents for LevelComplete");
        _carController.transform.position = _buySpawnPoint.transform.position;
        var controllerRb = _carController.GetComponent<Rigidbody>();
        controllerRb.velocity = Vector3.zero;
        controllerRb.angularVelocity = Vector3.zero;
        transform.rotation = _buySpawnPoint.transform.rotation;
        // TODO: Handle rotation
        // Start of rotation, but issues b/c controller is sphere
        //_carController.transform.eulerAngles = new Vector3(_carController.transform.eulerAngles.x, 180, _carController.transform.eulerAngles.z);
    }
}