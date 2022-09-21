using input;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootProjectile : MonoBehaviour
{
    public GameObject cannonball;
    public Transform shotPoint;
    public AudioSource laserSource;
    private StatusReporter playerReporter;
    private InputController inputController;

    private void Awake()
    {
        inputController = new InputController();
    }

    private void OnEnable()
    {
        playerReporter = GameObject.Find("Player").transform.GetComponent<StatusReporter>();
        inputController.Player.Fire.performed += Fire;
        inputController.Player.Fire.Enable();
    }

    private void OnDisable()
    {
        inputController.Player.Fire.performed -= Fire;
    }

    public void Fire(InputAction.CallbackContext inputAction)
    {
        if (canShoot())
        {
            GameObject createdCannonball = Instantiate(cannonball, shotPoint.position, shotPoint.rotation);
            createdCannonball.GetComponent<Rigidbody>().velocity =
                shotPoint.transform.forward * GameState.current.PlayerShotSpeed + playerReporter.velocity;
            // if the cannonball doesn't hit anything it will self destroy after 2 seconds.
            Destroy(createdCannonball, 2.0f);

            PewPew(createdCannonball.transform.position);
            GameEvents.current.ShotFired(createdCannonball.transform);
        }
    }

    private bool canShoot()
    {
        return GameState.current.CurrentAmmo > 0 && !GameState.current.BuyingBool && !GameState.current.Paused &&
               GameState.current.PlayerAlive;
    }

    //TODO: Event based sound system?
    private void PewPew(Vector3 clipPosition)
    {
        laserSource.PlayOneShot(laserSource.clip);
    }
}