using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EnemyVision))]
public class EnemyCore : MonoBehaviour
{
    #region VARIABLES

    public enum EState
    {
        NONE,
        STAND,
        GUARD,
        ALERTED,
        PATROL,
        SEARCH,
        ATTACK,
        ESCAPE,
        PANIC
    };

    public enum EEnemyType
    {
        MELEE,
        RANGED
    };

    [SerializeField] private EState defaultState = EState.STAND;
    [SerializeField] private EState stateAfterAlerted = EState.SEARCH;
    [SerializeField] private EState stateAfterSearch = EState.GUARD;
    [SerializeField] private EEnemyType enemyType = EEnemyType.MELEE;
    [SerializeField] private GameObject projectile = null;
    [SerializeField] private bool bSearchPlayerWhenLost = false;
    [SerializeField] private float navigationInterval = 1.0f;
    [SerializeField] private float spiderSenseRadius = 5.0f;
    [SerializeField] private float shootInterval = 5.0f;

    public EState currentState { get; private set; } = EState.NONE;
    public EEnemyType currentEnemyType { get; private set; } = EEnemyType.MELEE;
    public Vector3 spawnPosition { get; private set; } = Vector3.zero;

    private float stateTimer = 0.0f;
    private float stateTransitionTimer = 0.0f;
    private float shootIntervalTimer = 0.0f;
    private float navigationTimer = 0.0f;
    private Vector3 targetPosition = Vector3.zero;
    private Vector3 playerOffset = Vector3.zero;
    private EState previousState = EState.NONE;
    private EnemyVision vision = null;
    private NavMeshAgent agent = null;

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        playerOffset = Vector3.up * (GlobalVariables.player.GetComponent<CharacterController>().height / 2);
        spawnPosition = transform.position;
        currentState = defaultState;
        currentEnemyType = enemyType;
        vision = GetComponent<EnemyVision>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        AdvanceTimers();

        if (Vector3.Distance(transform.position, GlobalVariables.player.transform.position + playerOffset) < spiderSenseRadius)
        {
            RaycastHit hit;
            if (Physics.Raycast(
                transform.position,
                -Vector3.Normalize(transform.position - GlobalVariables.player.transform.position + playerOffset),
                out hit,
                spiderSenseRadius,
                1
                ))
            {
                if (currentState == EState.STAND
                    || currentState == EState.GUARD
                    || currentState == EState.PATROL
                    || currentState == EState.SEARCH)
                {
                    currentState = EState.ALERTED;
                    stateTimer = 6.0f;
                }
            }
        }

        switch (currentState)
        {
            case EState.NONE:                                   break;
            case EState.STAND:      AIStand();                  break;
            case EState.GUARD:      AIGuard();                  break;
            case EState.ALERTED:    AIAlerted();                break;
            case EState.PATROL:     AIPatrol();                 break;
            case EState.SEARCH:     AISearch();                 break;
            case EState.ATTACK:     AIAttack();                 break;
            case EState.ESCAPE:     AIEscape();                 break;
            case EState.PANIC:      AIPanic();                  break;
            default:                currentState = EState.NONE; break;
        }
    }

    void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up * 2.0f, currentState.ToString());
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.1f);
        Gizmos.DrawSphere(transform.position, spiderSenseRadius);
    }

    #endregion

    #region CUSTOM_METHODS

    void AdvanceTimers()
    {
        float time = Time.deltaTime;

        shootIntervalTimer      -= shootIntervalTimer > 0.0f    ? time : 0.0f;
        stateTimer              -= stateTimer > 0.0f            ? time : 0.0f;
        stateTransitionTimer    -= stateTransitionTimer > 0.0f  ? time : 0.0f;
        navigationTimer         -= navigationTimer > 0.0f       ? time : 0.0f;
    }

    public void OnHurt()
    {
        switch (currentState)
        {
            case EState.STAND:      currentState = EState.ALERTED;  stateTimer = 6.0f; break;
            case EState.GUARD:      currentState = EState.ALERTED; stateTimer = 6.0f; break;
            case EState.PATROL:     currentState = EState.ALERTED; stateTimer = 6.0f; break;
            case EState.SEARCH:     currentState = EState.ALERTED; stateTimer = 6.0f; break;
        }
    }

    public void OnDeath()
    {
        currentState = EState.NONE;
        Destroy(this.gameObject);
    }

    #endregion

    #region AI_LOGIC

    void AIStand()
    {
        if (vision.bCanSeePlayer)
        {
            currentState = EState.ALERTED;
            stateTimer = 6.0f;
            stateTransitionTimer = 1.5f;
        }
        else
        {
            if (navigationTimer <= 0.0f)
            {
                if (Vector3.Distance(transform.position, spawnPosition) > 3.0f)
                {
                    navigationTimer = navigationInterval;
                }
            }
        }
    }

    void AIGuard()
    {
        if (vision.bCanSeePlayer)
        {
            currentState = EState.ALERTED;
            stateTimer = 6.0f;
            stateTransitionTimer = 0.7f;
        }
    }

    void AIAlerted()
    {
        if (vision.bCanSeePlayer)
        {
            if (stateTransitionTimer <= 0.0f)
            {
                currentState = EState.ATTACK;
            }
        }
        else if (stateTimer <= 0.0f)
        {
            currentState = stateAfterAlerted;
        }
    }

    void AIPatrol()
    {
        if (vision.bCanSeePlayer)
        {
            if (vision.bCanSeePlayer)
            {
                currentState = EState.ALERTED;
                stateTimer = 6.0f;
                stateTransitionTimer = 0.7f;
            }
        }
    }

    void AISearch()
    {
        if (bSearchPlayerWhenLost)
        {
            if (vision.bCanSeePlayer)
            {
                currentState = EState.ALERTED;
                stateTimer = 6.0f;
                stateTransitionTimer = 0.7f;
            }
            else
            {
                if (Vector3.Distance(transform.position, vision.playerLKLocation) < 1.0f || vision.playerLKLocation == Vector3.zero)
                {
                    currentState = stateAfterSearch;
                    stateTimer = 6.0f;
                }
            }
        }
        else
        {
            currentState = stateAfterSearch;
        }
    }

    void AIAttack()
    {
        if (navigationTimer <= 0.0f)
        {
            navigationTimer = navigationInterval;
        }

        if (vision.bCanSeePlayer)
        {
            if (currentEnemyType == EEnemyType.RANGED)
            {
                if (Vector3.Distance(transform.position, GlobalVariables.player.transform.position + playerOffset) < spiderSenseRadius)
                {
                    currentState = EState.ESCAPE;
                    return;
                }
            }

            if (shootIntervalTimer <= 0.0f)
            {
                shootIntervalTimer = shootInterval;
                if (projectile != null)
                {
                    Vector3 direction = -Vector3.Normalize(transform.position + Vector3.up * 1.0f - (GlobalVariables.player.transform.position + Vector3.up * 1.0f));
                    Instantiate(projectile).GetComponent<Projectile>().Initialize(transform.position + Vector3.up * 1.0f, direction, this.gameObject);
                }
            }
        }
        else
        {
            currentState = EState.SEARCH;
        }
    }

    void AIEscape()
    {
        if (Vector3.Distance(transform.position, GlobalVariables.player.transform.position + playerOffset) > 20.0f)
        {
            currentState = EState.ATTACK;
        }
    }

    void AIPanic()
    {

    }

    #endregion
}
