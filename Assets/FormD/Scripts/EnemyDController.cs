using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDController : MonoBehaviour
{
    private NavMeshAgent agent;

    public float moveRadius = 5f;

    bool isMove;

    public float searchDistance;
    bool inSearch;
    public EnemyDShooter enemyDShooter;
    Animator anime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null) return;

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

        if (distance <= searchDistance)
        {
            agent.isStopped = true;

            if (!inSearch)
            {
                StartCoroutine(Search(player));
            }
        }
        else
        {
            agent.isStopped = false;
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

    IEnumerator Search(Transform player)
    {
        inSearch = true;

        // プレイヤー方向のベクトル（XZ平面のみ）
        Vector3 direction = player.position - transform.position;
        direction.y = 0f; // 上下方向は無視して水平だけ向く

        // 向きを決める
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float duration = 0.25f; // 回転にかける時間
        float elapsed = 0f;

        //一定時間かけて線形補間でなめらかにPlayerを見る
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        enemyDShooter.ShootAttack();
        yield return new WaitForSeconds(2.5f);
        inSearch = false;
    }
}