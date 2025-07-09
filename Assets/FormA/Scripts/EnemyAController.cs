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

    public Rigidbody rbody;
    Animator animator;

    public float activeDis = 5.0f;
    bool inActive;
    bool isTackle;
    public float AtkInterval = 3.0f;

    public float tackleForce = 5.0f;

    public Collider tackleCollider;

    void Start()
    {
        tackleCollider.enabled = false;
        agent = GetComponent<NavMeshAgent>();
        rbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgentが見つかりません！");
            enabled = false;
            return;
        }
        isTackle = false;

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
            Debug.Log("攻撃範囲内");
            agent.isStopped = true;
            if (!isTackle)
            {
                //InvokeRepeating("EnemyTackle", 0, 3f);
                StartCoroutine(EnemyTackle());


            }


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

    IEnumerator EnemyTackle()
    {
        Debug.Log("攻撃コルーチン開始");
        isTackle = true;
        Transform player = PlayersManager.Instance.GetPlayer();

        //プレイヤー方向のベクトル取得
        Vector3 direction = player.position - transform.position;
        direction.y = 0;　//上下方向を0にする

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float duration = 1.5f;
        float elapsed = 0f;
        float tackleTime = 0f;

        //一定時間かけて線形補間でなめらかにPlayerを見る
        while (elapsed < duration)
        {
            Debug.Log("プレイヤーの方を向く");
            float t = elapsed / duration;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
            elapsed += Time.deltaTime;
            yield return null;

        }

        Debug.Log("タックル");
        tackleCollider.enabled = true;
        animator.SetTrigger("attack");
        while (tackleTime <= 0.5f)
        {
            rbody.AddForce(transform.forward * tackleForce, ForceMode.Impulse);
            tackleTime += Time.deltaTime;
            yield return null;
        }

        tackleCollider.enabled = false;

        yield return new WaitForSeconds(AtkInterval);
        isTackle = false;


    }
}
