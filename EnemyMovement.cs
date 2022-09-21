using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    Melee,
    Ranged
}

public enum BehaviorState
{
    Patrol,
    Wait,
    Attack,
    Evade,
    Steal
}

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent enemyAgent;
    private Transform playerTransform;
    private Transform enemyTransform;

    // state: management
    public EnemyType unitType = EnemyType.Melee;
    public BehaviorState initialBehaviorState = BehaviorState.Patrol;
    public BehaviorState currentBehaviorState;

    // state: patrolling
    public float minPatrolDistance;
    public float maxPatrolDistance;
    public float patrolWaypointDistance = .25f;
    public float patrolSpeed = 10f;
    public float patrolAcceleration = 8f;
    public Vector3 currentPatrolWayPoint;

    //state: evading
    public Vector3 evadeWaypoint;
    public float evadeMaxAngle = 20;
    public float evadeMinDistance = 10;
    public float evadeMaxDistance = 20;

    // state: attacking
    public float attackSpeed = 20f;
    public bool attackEnabled = true;

    public float attackAccelleration = 15f;
    public float breakOffAttackDistance = 5.0f; // after player is this far away - break off attack

    //proximity detection
    public float proximityAttackDistance = 5.0f; // Proximity that a player will be detected regardless of FOV
    public bool playerInProximity; // bool for determining gamestate

    // FOV 
    public float fovRoutineWaitSeconds = .33f; // time betweeen searches (dude to raycast cam impact performance
    public float fovRadius = 5f; // radius of FOV search
    [Range(0, 360)] public float fovViewAngle = 90f; // angle that player can see

    public LayerMask fovTargetMask; // what to look for
    public LayerMask fovObstructionMask; // what enemy cannot see through
    public bool fovCanSeePlayer; // what enemy cannot see through

    public Vector3
        fovLastSeenPlayerPosition =
            Vector3.zero; // tracking player position - agent will continue here after they can no longer see the player

    public float maxHealth = 100;
    public float currentHealth;
    public HealthBarSlider healthBarSlider;

    //Stealing
    public float pickUpTheftDistance = 10f;
    public float pickupScanRoutineWaitSeconds = .5f;
    public bool canSeePickup = false;
    public Vector3 pickupWaypoint;

    // Attack values
    [Range(0, 100)] public float meleeAttackPower = 5;

    //Bail if "stuck" using a timer
    private float _timeInState;

    public AudioSource engineSound;
    public float basePitch = 0.5f;
    public float spikeDamageForAi = 1f;


    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyTransform = GetComponent<Transform>();
        currentBehaviorState = BehaviorState.Patrol;
        
        healthBarSlider.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;

        StartCoroutine(PlayerScanRoutine());
        StartCoroutine(PickupScanRoutine());

        engineSound.volume = basePitch;
        engineSound.Play();
        GameEvents.current.OnTogglePause += StopSoundOnPause;
    }

    private void StopSoundOnPause(bool paused)
    {
        if (paused)
        {
            engineSound.Stop();
        }
        else if (!engineSound.isPlaying)
        {
            engineSound.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: If we have lots of enemies might want to make this a scheduled coroutine to reduce perf hit. 
        CheckState();
        switch (currentBehaviorState)
        {
            case BehaviorState.Patrol:
                Patrol();
                break;
            case BehaviorState.Attack:
                Attack();
                break;
            case BehaviorState.Evade: // do nothing this is a quick navmesh redirect
            case BehaviorState.Steal:
                break;
            default:
                currentBehaviorState = BehaviorState.Patrol;
                break;
        }

        //TODO: Speed (or state?) based sound
        // engineSound.pitch = Math.Abs(speedInput) / 200 + basePitch;
    }

    public void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cannonball"))
        {
            var playerBlastPower = GameState.current.PlayerAttackPower;
            var otherCollisionTracker = other.GetComponent<CannonBallCollisionTracker>();


            if (!otherCollisionTracker.HasCollided)
            {
                GameEvents.current.ProjectileCollision(other.transform);
                if (currentBehaviorState == BehaviorState.Patrol)
                    Evade();

                // somehow we need to access shooting projectile blastpower variable.
                var alive = RemoveHealthAndReportIfAlive(playerBlastPower);
                if(!alive)                
                    return;
                                
                otherCollisionTracker.HasCollided = true;
            }

            Destroy(other.gameObject);
        }
        else if (attackEnabled && other.CompareTag("Player"))
        {
            //todo: minor health deficit here?
            GameEvents.current.PlayerMeleeAttacked(transform);
            currentBehaviorState = BehaviorState.Evade;
        }
        else if(other.CompareTag("SpikeWall"))
        {
            var alive = RemoveHealthAndReportIfAlive(spikeDamageForAi);
            if (!alive)
                return;
        }
    }

    public bool RemoveHealthAndReportIfAlive(float power)
    {
        currentHealth -= power;
        healthBarSlider.UpdateHealth(currentHealth);
        if (currentHealth <= 0.0f)
        {
            Destroy(enemyTransform.gameObject);
            GameEvents.current.EnemyKilled(enemyTransform);
            return false;
        }
        return true;
    }

    private void CheckState()
    {
        var startingState = currentBehaviorState;
        // loitering: check for opportunity to attack
        if (currentBehaviorState == BehaviorState.Patrol
            || currentBehaviorState == BehaviorState.Steal)
        {
            if (PlayerFound())
                currentBehaviorState = BehaviorState.Attack;

            if (currentBehaviorState == BehaviorState.Steal)
            {
                if (enemyAgent.remainingDistance < .25 && !enemyAgent.pathPending)
                {
                    currentBehaviorState = BehaviorState.Patrol;
                }
                else if (canSeePickup)
                {
                    enemyAgent.SetDestination(pickupWaypoint);
                    currentBehaviorState = BehaviorState.Steal;
                }
            }
        }

        // attacking: break off if conditions are met
        else if (currentBehaviorState == BehaviorState.Attack)
        {
            if (enemyAgent.remainingDistance < breakOffAttackDistance && !enemyAgent.pathPending)
                currentBehaviorState = BehaviorState.Patrol;
        }

        else if (currentBehaviorState == BehaviorState.Evade)
        {
            // ran away now try to circle back and attack
            if (enemyAgent.remainingDistance < 1.0f && !enemyAgent.pathPending)
                currentBehaviorState = BehaviorState.Attack;
        }

        if (startingState == currentBehaviorState)
        {
            _timeInState += Time.deltaTime;
            if (_timeInState > MAX_STATE_TIME)
            {
                _timeInState = 0f;
                ResetPatrolWaypoint();
                currentBehaviorState = BehaviorState.Patrol;
                //Assume we're chasing a point we can't get to.
            }
        }
    }

    private const double MAX_STATE_TIME = 10;

    // behaviors
    private void Attack()
    {
        enemyAgent.acceleration = attackAccelleration;
        enemyAgent.speed = attackSpeed;

        if (unitType == EnemyType.Melee)
            Chase();
        if (unitType == EnemyType.Ranged)
            CloseRange();
        // todo: if ranged enemy chase and fire??
    }

    private void Evade()
    {
        if (currentBehaviorState != BehaviorState.Evade)
        {
            var randomVector = new Vector3(Random.Range(evadeMaxAngle * -1, evadeMaxAngle), 0,
                Random.Range(evadeMaxAngle * -1, evadeMaxAngle));
            var runDirection = (transform.position - playerTransform.position).normalized + randomVector;
            var directionOnly = runDirection / runDirection.magnitude;
            var distance = Random.Range(evadeMinDistance, evadeMaxDistance);
            var runTarget = transform.position + (directionOnly.normalized * distance);

            NavMesh.SamplePosition(runTarget, out NavMeshHit hit, 1.0f, NavMesh.AllAreas);
            evadeWaypoint = hit.position; // this is the closest point on the navmesh


            fovLastSeenPlayerPosition = playerTransform.position;
            currentBehaviorState = BehaviorState.Evade;
        }

        enemyAgent.acceleration = attackAccelleration;
        enemyAgent.speed = attackSpeed;
        enemyAgent.SetDestination(evadeWaypoint);
    }

    private void Chase()
    {
        var predictedPosition = fovLastSeenPlayerPosition;
        if (PlayerFound())
        {
            var playerVelocity = playerTransform
                .gameObject
                .GetComponent<StatusReporter>()
                .smoothedVelocity;

            fovLastSeenPlayerPosition = playerTransform.position;
            var lookAhead = Vector3.Distance(fovLastSeenPlayerPosition, transform.position) / enemyAgent.speed;
            predictedPosition = fovLastSeenPlayerPosition + lookAhead * playerVelocity;
        }

        Vector3 targetPosition;

        //TODO I'm not sure this is right? We likely will want to change exactly how we figure out tracking if player is 
        // off the nav mesh. Alternatively - Should we update the navmesh to just be everywhere to avoid cheese?        
        NavMesh.SamplePosition(predictedPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas);
        targetPosition = hit.position; // this is the closest point on the navmesh
        enemyAgent.SetDestination(targetPosition);
    }
    private void CloseRange()
    {
        var predictedPosition = fovLastSeenPlayerPosition;
        if (PlayerFound())
        {
            var playerVelocity = playerTransform
                .gameObject
                .GetComponent<StatusReporter>()
                .smoothedVelocity;

            fovLastSeenPlayerPosition = playerTransform.position;
            var lookAhead = Vector3.Distance(fovLastSeenPlayerPosition, transform.position) / enemyAgent.speed;
            predictedPosition = fovLastSeenPlayerPosition + lookAhead * playerVelocity;
        }

        Vector3 targetPosition;

        //TODO I'm not sure this is right? We likely will want to change exactly how we figure out tracking if player is 
        // off the nav mesh. Alternatively - Should we update the navmesh to just be everywhere to avoid cheese?
        if (NavMesh.SamplePosition(predictedPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            targetPosition = fovLastSeenPlayerPosition;
        else
            targetPosition = hit.position; // this is the closest point on the navmesh
        
        Debug.Log(Vector3.Distance(fovLastSeenPlayerPosition, transform.position));
        if (Vector3.Distance(fovLastSeenPlayerPosition, transform.position) >= 35.0f ) // makes so goes twards but dosnt go on top, makes strafe 
        {
            enemyAgent.SetDestination(fovLastSeenPlayerPosition);
        }
        
    }


    //TODO: Playtest feedback - Might want to look around a bit more to try and get player in FOV before patrolling.
    private void Patrol()
    {
        enemyAgent.acceleration = patrolAcceleration;
        enemyAgent.speed = patrolSpeed;

        if (canSeePickup)
        {
            enemyAgent.SetDestination(pickupWaypoint);
            currentBehaviorState = BehaviorState.Steal;
        }

        if (currentPatrolWayPoint == null
            || enemyAgent.pathStatus != NavMeshPathStatus.PathComplete
            || (enemyAgent.remainingDistance < patrolWaypointDistance
                && !enemyAgent.pathPending))
        {
            ResetPatrolWaypoint();
        }

        enemyAgent.SetDestination(currentPatrolWayPoint);
    }

    private void ResetPatrolWaypoint()
    {
        var legDistance = Random.Range(minPatrolDistance, maxPatrolDistance);
        var randomPerturbation = Mathf.RoundToInt(Vector3.Distance(transform.position, playerTransform.position) / 5);
        var randomVector = new Vector3(Random.Range(randomPerturbation * -1, randomPerturbation), 0,
            Random.Range(randomPerturbation * -1, randomPerturbation));
        var headingToPlayer = (playerTransform.position - transform.position) + randomVector;
        var directionToPlayer = headingToPlayer / headingToPlayer.magnitude;
        var waypointTowardsPlayer = transform.position + (directionToPlayer.normalized * legDistance);

        NavMeshHit hit;
        NavMesh.SamplePosition(waypointTowardsPlayer, out hit, legDistance, 1);

        currentPatrolWayPoint = hit.position;
    }

    private bool PlayerFound()
    {
        return (fovCanSeePlayer || playerInProximity);
    }

    private IEnumerator PlayerScanRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(fovRoutineWaitSeconds);

        while (true)
        {
            yield return wait;
            PlayerScan();
        }
    }

    private IEnumerator PickupScanRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(fovRoutineWaitSeconds * 20);

        while (true)
        {
            yield return wait;
            PickUpScan();
        }
    }


    // Citation: Derived From Video Tutorial https://www.youtube.com/watch?v=j1-OyLo77ss
    private void PlayerScan()
    {
        playerInProximity = Vector3.Distance(playerTransform.position, transform.position) < proximityAttackDistance;

        if (playerInProximity)
            return; // short circuit here to save performance of raycast

        var collidersInRange = new Collider[1];
        int colliderCount =
            Physics.OverlapSphereNonAlloc(transform.position, fovRadius, collidersInRange, fovTargetMask);

        if (colliderCount > 0)
        {
            var player = collidersInRange[0].transform; // due to targetMask can only be player
            var directionToPlayer = (player.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToPlayer) < fovViewAngle / 2)
            {
                var distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, fovObstructionMask))
                {
                    fovCanSeePlayer = true;
                    return;
                }
            }
        }

        fovCanSeePlayer = false;
    }

    private void PickUpScan()
    {
        GameObject[] pickUps = GameObject.FindGameObjectsWithTag("PickUp");
        foreach (var pickup in pickUps)
        {
            // TODO: Let a pickup hang out for some time before going to get it
            var distanceToPickup = Vector3.Distance(transform.position, pickup.transform.position);
            var directionToPickup = (pickup.transform.position - transform.position).normalized;
            if (distanceToPickup <= pickUpTheftDistance)
            {
                if (!Physics.Raycast(transform.position, directionToPickup, distanceToPickup, fovObstructionMask))
                {
                    canSeePickup = true;
                    pickupWaypoint = pickup.transform.position;
                    return;
                }
            }
        }

        canSeePickup = false;
    }

    private void OnDestroy()
    {
        GameEvents.current.OnTogglePause -= StopSoundOnPause;
    }
}