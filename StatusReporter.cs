using UnityEngine;

/*
 * Script to allow game objects to report details
 * about their status.
 */

public class StatusReporter : MonoBehaviour
{
    private Vector3 _prevPos;
    private Vector3 _smoothingParamVel;

    public Vector3 velocity { get; private set; }

    public Vector3 smoothedVelocity { get; private set; }

    public float smothingTimeFactor = 0.5f;

    void Start()
    {
        _prevPos = transform.position;
        velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime != 0)
        {
            var position = transform.position;
            velocity = (position - _prevPos) / Time.deltaTime;

            smoothedVelocity =
                Vector3.SmoothDamp(smoothedVelocity, velocity, ref _smoothingParamVel, smothingTimeFactor);

            _prevPos = position;
        }
    }
}