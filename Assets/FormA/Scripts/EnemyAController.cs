using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAController : MonoBehaviour
{
    private NavMeshAgent agent;

    public float moveRadius = 5f;

    bool isMove;

    Vector3 enemyMove;

    Animator animator;

    public float activeDis = 5.0f;
    bool inActive;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgentが見つかりません！");
            enabled = false;
            return;
        }

        InvokeRepeating("Wander", 0, 5f); // 5秒ごとにランダム移動
    }

    void Update()
    {
        //プレイヤー探索
        Transform player = PlayersManager.Instance.GetPlayer();
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (agent.isOnNavMesh && !isMove)
        {
            agent.isStopped = false;
            isMove = true;
        }

        if (agent.velocity != Vector3.zero)
        {
            animator.SetBool("move", true);
        }
        else
        {
            animator.SetBool("move", false);
        }

        if (distance <= activeDis)
        {
            agent.isStopped = true;
            InvokeRepeating("EnemyTackle", 0, 3f);

        }




    }

    void Wander()
    {
        if (agent.isOnNavMesh && !agent.isStopped)
        {
       

            Vector3 randomPos = transform.position + Random.insideUnitSphere * moveRadius;
            randomPos.y = transform.position.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, moveRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

    }

    void EnemyTackle()
    {
        Transform player = PlayersManager.Instance.GetPlayer();
        float dX = transform.position.x - player.position.x;
        float dZ = transform.position.z - player.position.z;

        transform.rotation = Quaternion.Euler(dX, 0, dZ);





    }
}
