using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBController : MonoBehaviour
{
    private NavMeshAgent agent;

    public float detectRadius = 3f;
    public float moveRadius = 5f;
    public float stopTime = 1.0f;

    private float stopTimer = 0f;
    private bool isEscaping = false;

    public EnemyHidden enemyHidden;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgentが見つかりません！");
            enabled = false;
            return;
        }
        agent.isStopped = true; // 初期状態で停止！
    }

    void Update()
    {
        Transform player = PlayersManager.Instance.GetPlayer();
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < detectRadius)
        {
            // 逃走中でなければストップ
            if (!isEscaping)
            {
                agent.isStopped = true;
                stopTimer += Time.deltaTime;

                if (stopTimer >= stopTime)
                {
                    isEscaping = true;
                    stopTimer = 0f;
                    agent.isStopped = false; // 逃走開始！

                    // 消える演出
                    if (enemyHidden != null)
                        enemyHidden.HideEnemy();

                    EscapeFromPlayer(player);
                }
            }
            else
            {
                // 逃走中だけ動く
                agent.isStopped = false;
                if (agent.remainingDistance < 0.5f)   
                {
                    Debug.Log("到着");
                    isEscaping = false;
                    agent.isStopped = true; // ゴールしたらまた停止！
                }
            }
        }
        else
        {
            // プレイヤーがいない時は完全停止・タイマーリセット
            isEscaping = false;
            stopTimer = 0f;
            agent.isStopped = true;
        }
    }

    void EscapeFromPlayer(Transform player)
    {
        Vector3 dirFromPlayer = (transform.position - player.position).normalized;
        dirFromPlayer.y = 0f;
        Vector3 escapePos = transform.position + dirFromPlayer * moveRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(escapePos, out hit, moveRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            Debug.Log(hit.position);
        }
    }
}