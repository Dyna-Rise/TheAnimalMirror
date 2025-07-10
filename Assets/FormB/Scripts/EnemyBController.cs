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
            //Debug.Log("距離が近い");
            // 逃走中でなければストップ
            if (!isEscaping)
            {
                stopTimer += Time.deltaTime;
                if (stopTimer < stopTime) return;
                //Debug.Log("時間経過");

                //脱出開始
                isEscaping = true;
                stopTimer = 0f;

                // 消える演出
                if (enemyHidden != null)
                    enemyHidden.HideEnemy();

                EscapeFromPlayer(player);
            }

        }
        else
        {
            // 遠い場合はタイマーリセット
            stopTimer = 0f;
        }

        if (isEscaping)
        {
            //Debug.Log("発動中");
            if (agent.remainingDistance < 0.5f)
            {
                //Debug.Log("到着");
                isEscaping = false;
                agent.isStopped = true; // ゴールしたらまた停止！
            }
        }
    }

    void EscapeFromPlayer(Transform player)
    {
        //Debug.Log("発動");

        Vector3 dirFromPlayer = (transform.position - player.position).normalized;
        //dirFromPlayer.y = 0f;
        Vector3 escapePos = transform.position + dirFromPlayer * moveRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(escapePos, out hit, moveRadius, NavMesh.AllAreas))
        {
            agent.isStopped = false; // ← これを忘れない！
            agent.SetDestination(hit.position);
            //Debug.Log(hit.position);
        }
    }
}
