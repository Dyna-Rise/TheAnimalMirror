using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.AI;

// 鶏エネミー
public class EnemyCCntroller : MonoBehaviour
{
    public float moveRadius = 5f;   // うろつく範囲
    public float sensingRange = 8f; // プレイヤー検知範囲

    public GameObject damageObj; // 投下するやつ
    public int dropCount = 5;    // 投下数
    public float dropRange = 5f; // 投下範囲

    public float attackJumpHeight = 7f; // 攻撃時のジャンプの高さ

    bool isMove;   // AI動作中か
    bool isWander; // うろつき中か
    bool isWatch;  // 攻撃待機中か
    bool isAttack; // 攻撃中か

    float watchTime;       // 攻撃待機時間(累積)
    float attackWaitTime;  // 攻撃待機時間

    Vector3 lastForward; // 旋回検知用の向き

    Collider damageArea; // プレイヤーにあたるとダメージが入る部分
    NavMeshAgent agent;
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

        if (damageObj == null)
        {
            Debug.LogError("damageObjが見つかりません！");
            enabled = false;
            return;
        }

        // 子オブジェクトのコライダーを取得
        damageArea = transform.Find("DamageArea").GetComponent<SphereCollider>();
        damageArea.enabled = false;

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // 初期状態の向きを保存
        lastForward = transform.forward;

        // 攻撃待機時間セット
        SetAttackWaitTime();
    }

    void Update()
    {
        // AI有効化
        if (agent.isOnNavMesh && !isMove)
        {
            agent.isStopped = false;
            isMove = true;
        }

        // プレイヤーとの距離を取得
        Transform player = PlayersManager.Instance.GetPlayer();
        if (player == null)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, player.position);
        //Debug.Log(distance);

        // プレイヤーと距離があるとき
        if (distance > sensingRange) 
        {
            // 攻撃状態を初期化
            isWatch = false;
            isAttack = false;
            watchTime = 0;

            // うろつく
            if (!isWander)
            {
                StartCoroutine("Wander");
            }

            return;
        }

        // 攻撃中以外
        if (!isAttack)
        {
            // 待機時間待った
            if (watchTime > attackWaitTime)
            {
                // 攻撃
                StartCoroutine("Attack", player);

                // 次回の攻撃待機時間をセット
                SetAttackWaitTime();
            }
            // プレイヤーを見て攻撃待機
            else if (!isWatch)
            {
                StartCoroutine("Watch", player.position);
            }
        }
    }

    private void FixedUpdate()
    {
        // 旋回してるか判定
        Vector3 currentForward = transform.forward;
        float angleDiff = Vector3.Angle(lastForward, currentForward);

        // 移動中アニメ更新
        animator.SetBool("move", agent.velocity.sqrMagnitude > 0.1f || angleDiff > 1f);

        // 次回チェック用に現在の向きを保存
        lastForward = currentForward;
    }

    // うろつく
    IEnumerator Wander()
    {
        //Debug.Log("Wander");
        isWander = true;

        // AIが有効
        if (agent.isOnNavMesh && !agent.isStopped)
        {
            // ランダム座標取得
            Vector3 randomPos = transform.position + Random.insideUnitSphere * moveRadius;
            randomPos.y = transform.position.y;

            // 移動
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, moveRadius, NavMesh.AllAreas))
            {
                transform.rotation = Quaternion.LookRotation(hit.position);
                agent.SetDestination(hit.position);
            }
        }

        // それっぽく停止
        yield return new WaitForSeconds(Random.Range(1f, 4f));

        isWander = false;
    }

    // プレイヤーの方を見て攻撃待機
    IEnumerator Watch(Vector3 targetPos)
    {
        //Debug.Log("Watch");
        isWatch = true;

        // 座標を相対化
        Vector3 pos = targetPos - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), Time.deltaTime * 5f);

        // 時間を加算
        watchTime += Time.deltaTime;

        isWatch = false;

        yield return null;
    }

    // 攻撃
    IEnumerator Attack(Transform target)
    {
        //Debug.Log("Attack");
        isAttack = true;

        // NavMeshAgentを切らないと動かない
        agent.enabled = false;

        // アニメーション更新
        animator.SetBool("jump", true);

        // 接触ダメージ有効化
        damageArea.enabled = true;

        Vector3 pos = target.position - transform.position;
        float velocityY = attackJumpHeight;
        float lastY = 0f;
        bool isDone = false;

        // 空中にいる間繰り返す
        while (!controller.isGrounded || velocityY > 0f)
        {
            // 前フレーム時点の高さを保存
            lastY = transform.position.y;

            // 重力をかける
            velocityY += Physics.gravity.y * Time.deltaTime;

            // 旋回
            transform.rotation = Quaternion.LookRotation(pos);

            // 移動
            Vector3 move = Vector3.up * velocityY + transform.forward * 7f;
            controller.Move(move * Time.deltaTime);

            // 最高点なら何かを投下
            if (!isDone && lastY > transform.position.y)
            {
                for (int i = 0; i < dropCount; i++)
                {
                    GameObject obj = Instantiate(
                        damageObj,
                        new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z - (i * 0.75f)),
                        Quaternion.Euler(0, 90, 90)
                    );
                    obj.GetComponent<Rigidbody>().AddForce(
                        new Vector3(Random.Range(-dropRange, dropRange), -3f, Random.Range(-dropRange, dropRange)),
                        ForceMode.Impulse
                    );
                }
                // 投下済みフラグオン
                isDone = true;
            }

            yield return null;
        }

        // 接触ダメージ無効化
        damageArea.enabled = false;

        // アニメーションを戻す
        animator.SetBool("jump", false);

        // NavMeshAgentを戻す
        agent.enabled = true;

        // 攻撃状態を初期化
        watchTime = 0;
        isAttack = false;
    }

    // 攻撃待機時間をセット
    void SetAttackWaitTime()
    {
        attackWaitTime = Random.Range(0f, 1.5f);
    }
}
