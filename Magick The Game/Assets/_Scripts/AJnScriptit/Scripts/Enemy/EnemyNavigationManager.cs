//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigationManager : MonoBehaviour
{
    [SerializeField] private float navigationErrorMargin = 0.5f;
    [SerializeField] private float navigationInterval = 1.0f;
    [SerializeField] private Vector3[] patrolPoints;

    private int navCurrentPoint = 0;
    private float navTimer = 0.0f;
    private EnemyCore cEnemyCore = null;
    private NavMeshAgent agent = null;

    void Start()
    {
        cEnemyCore = GetComponent<EnemyCore>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (navTimer <= 0.0f)
        {
            navTimer = navigationInterval;
            switch (cEnemyCore.currentState)
            {
                case EnemyCore.EState.STAND:    AIStand();      break;
                case EnemyCore.EState.GUARD:    AIGuard();      break;
                case EnemyCore.EState.ALERTED:  AIAlerted();    break;
                case EnemyCore.EState.PATROL:   AIPatrol();     break;
                case EnemyCore.EState.SEARCH:   AISearch();     break;
                case EnemyCore.EState.ATTACK:   AIAttack();     break;
                case EnemyCore.EState.ESCAPE:   AIEscape();     break;
                case EnemyCore.EState.PANIC:    AIPanic();      break;
                default:                                        break;
            }
        }
        else
        {
            navTimer -= Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        int patrolLength = patrolPoints.Length;

        if (patrolLength != 0)
        {
            for (int i = 0; i < patrolLength; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(patrolPoints[i], Vector3.one * 0.3f);
                if (patrolLength > 1)
                {
                    if (i == patrolLength - 1)
                    {
                        Gizmos.DrawLine(patrolPoints[i], patrolPoints[0]);
                    }
                    else
                    {
                        Gizmos.DrawLine(patrolPoints[i], patrolPoints[i + 1]);
                    }
                }
            }
        }
    }




    void AIStand()
    {
        if (Vector3.Distance(transform.position, cEnemyCore.spawnPosition) > navigationErrorMargin)
        {
            agent.SetDestination(cEnemyCore.spawnPosition);
        }
    }

    void AIGuard()
    {
        AIStand();
    }

    void AIAlerted()
    {
        Vector3 randomPosition = Vector3.zero;
        randomPosition.x = Random.Range(-1.0f, 1.0f);
        randomPosition.y = 0.0f;
        randomPosition.z = Random.Range(-1.0f, 1.0f);

        agent.SetDestination(transform.position + randomPosition);
    }

    void AIPatrol()
    {
        if (Vector3.Distance(transform.position, patrolPoints[navCurrentPoint]) < navigationErrorMargin)
        {
            navCurrentPoint++;
            if (navCurrentPoint >= patrolPoints.Length)
            {
                navCurrentPoint = 0;
            }
        }
        agent.SetDestination(patrolPoints[navCurrentPoint]);
    }

    void AISearch()
    {
        agent.SetDestination(GetComponent<EnemyVision>().playerLKLocation);
    }

    void AIAttack()
    {
        if (cEnemyCore.currentEnemyType == EnemyCore.EEnemyType.MELEE)
        {
            agent.SetDestination(GetComponent<EnemyVision>().playerLKLocation);
        }
    }

    void AIEscape()
    {
        if (Vector3.Distance(transform.position, GlobalVariables.player.transform.position) < 20.0f)
        {
            agent.SetDestination(transform.position + Vector3.Normalize(transform.position - GlobalVariables.player.transform.position) * 5.0f);
        }
    }

    void AIPanic()
    {

    }
}
