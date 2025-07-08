using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDCntroller : MonoBehaviour
{
    private NavMeshAgent agent;

    public float moveRadius = 5f;

    bool isMove;

    Animator anime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgentが見つかりません！");
            enabled = false;
            return;
        }

        InvokeRepeating("Wander", 0, 5f); // 5秒ごとにランダム移動

        anime = GetComponent<Animator>();
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

        bool isMoving = agent.velocity.sqrMagnitude > 0.01f; // わずかに動いてる場合もtrue、それ以外はfalse
        anime.SetBool("move", isMoving);
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
}