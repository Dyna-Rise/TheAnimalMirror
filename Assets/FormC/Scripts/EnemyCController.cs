using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyCCntroller : MonoBehaviour
{
    private NavMeshAgent agent;

    // 移動距離
    public float moveRadius = 5f;

    // プレイヤー検知距離
    public float sensingDistance = 5f;

    bool isMove;
    bool isWander;
    bool isWatch;
    bool isAttack;

    public float watchTime;
    public float attackSpeed = 5f;
    float attackWaitTime;

    Vector3 lastForward;

    CharacterController controller;
    Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgentが見つかりません！");
            enabled = false;
            return;
        }

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // 向きを保存
        lastForward = transform.forward;

        // 攻撃待機時間セット
        SetAttackWaitTime();
    }

    void Update()
    {
        // プレイヤーの位置を取得
        Transform player = PlayersManager.Instance.GetPlayer();
        if (player == null)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, player.position);

        // AI有効化
        if (agent.isOnNavMesh && !isMove)
        {
            agent.isStopped = false;
            isMove = true;
        }
        if (!isMove)
        {
            return;
        }

        // プレイヤーから距離があるとき
        if (distance > sensingDistance) 
        {
            // 状態を初期化
            isWatch = false;
            isAttack = false;
            watchTime = 0;

            // うろつく
            if (!isWander)
            {
                isWander = true;
                StartCoroutine("Wander");
            }

            return;
        }

        if (!isAttack)
        {
            if (watchTime > attackWaitTime)
            {
                isAttack = true;
                StartCoroutine("Attack", player.position);

                // 次回の攻撃待機時間をセット
                SetAttackWaitTime();
            }
            else if (!isWatch)
            {
                isWatch = true;
                StartCoroutine("Watch", player.position);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 currentForward = transform.forward;
        float angleDiff = Vector3.Angle(lastForward, currentForward);
        lastForward = currentForward;

        // 移動中アニメ更新
        animator.SetBool("move", agent.velocity.sqrMagnitude > 0.1f || angleDiff > 1f);
    }

    // うろつく
    IEnumerator Wander()
    {
        Debug.Log("Wander");

        if (agent.isOnNavMesh && !agent.isStopped)
        {
            // ランダム座標
            Vector3 randomPos = transform.position + Random.insideUnitSphere * moveRadius;
            randomPos.y = transform.position.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, moveRadius, NavMesh.AllAreas))
            {
                transform.rotation = Quaternion.LookRotation(hit.position);
                agent.SetDestination(hit.position);
            }
        }

        yield return new WaitForSeconds(Random.Range(1f, 4f));

        isWander = false;
    }

    // プレイヤーを見つめる
    IEnumerator Watch(Vector3 targetPos)
    {
        Debug.Log("Watch");

        Vector3 pos = targetPos - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), 3f * Time.deltaTime);

        watchTime += Time.deltaTime;

        isWatch = false;

        yield return null;
    }

    // 攻撃
    IEnumerator Attack(Vector3 targetPos)
    {
        Debug.Log("Attack");

        // NavMeshAgentを切らないと動かない
        agent.enabled = false;

        float verticalVelocity = 7f;

        animator.SetBool("jump", true);
        while (!controller.isGrounded || verticalVelocity > 0f)
        {
            verticalVelocity -= Mathf.Abs(Physics.gravity.y) * Time.deltaTime;

            Vector3 jumpMove = Vector3.up * verticalVelocity + transform.forward * 3f;

            controller.Move(jumpMove * Time.deltaTime);

            yield return null;
        }

        animator.SetBool("jump", false);

        // NavMeshAgentを戻す
        agent.enabled = true;
        Debug.Log("grounded");

        watchTime = 0;
        isAttack = false;

        yield return null;
    }

    // 攻撃待機時間をセット
    void SetAttackWaitTime()
    {
        attackWaitTime = Random.Range(1f, 5f);
    }
}
