using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

// 鶏エネミー
public class EnemyCCntroller : MonoBehaviour
{
    public float moveRadius = 5f;   // うろつく範囲
    public float sensingRange = 6f; // プレイヤー検知範囲

    public GameObject dropObj;      // 投下物
    public int dropCount = 4;       // 投下数
    public float scatterPower = 3f; // 投下範囲

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

        if (dropObj == null)
        {
            Debug.LogError("dropObjが見つかりません！");
            enabled = false;
            return;
        }

        // 子オブジェクトのコライダーを取得
        damageArea = transform.Find("DamageArea").GetComponent<SphereCollider>();
        if (damageArea != null)
        {
            damageArea.enabled = false;
        }

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
                StartCoroutine("Attack");

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

        // 旋回
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), Time.deltaTime * 5f);

        // 時間を加算
        watchTime += Time.deltaTime;

        isWatch = false;

        yield return null;
    }

    // 攻撃
    IEnumerator Attack()
    {
        //Debug.Log("Attack");
        isAttack = true;

        // NavMeshAgentを切らないと動かない
        agent.enabled = false;

        // アニメーション更新
        animator.SetBool("jump", true);

        // 接触ダメージ有効化
        if (damageArea != null)
        {
            damageArea.enabled = true;
        }

        int dropped = 0;
        float timer = 0f;
        float velocityY = attackJumpHeight;
        float lastY = 0f;
        float initZ = 0f;
        Vector3 diffPos = Vector3.zero;

        // 空中にいる間繰り返す
        while (!controller.isGrounded || velocityY > 0f)
        {
            // 重力をかける
            velocityY += Physics.gravity.y * Time.deltaTime;

            // ゆっくり落とす
            if (velocityY < -1.5f)
            {
                velocityY = -1.5f;
            }

            // 上昇中
            if (lastY < transform.position.y)
            {
                // プレイヤー位置取得
                Transform player = PlayersManager.Instance.GetPlayer();

                // 上昇中は旋回する
                diffPos = player.position - transform.position;
                transform.rotation = Quaternion.LookRotation(new Vector3(diffPos.x, 0, diffPos.z));

                // 初回だけ移動距離をセット
                initZ = initZ > 0 ? initZ : diffPos.z;
            }
            // 下降中
            else
            {
                // 投下数だけ投下する
                if (dropped < dropCount && timer <= 0f)
                {
                    // 投下物生成
                    GameObject obj = Instantiate(
                        dropObj,
                        new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z - 1f),
                        Quaternion.Euler(0, 90, 135)
                    );
                    // 投下
                    obj.GetComponent<Rigidbody>().AddForce(
                        new Vector3(Random.Range(-scatterPower, scatterPower), -2f, Random.Range(-scatterPower, scatterPower)),
                        ForceMode.Impulse
                    );
                    // 投下数追加
                    dropped++;

                    // タイマー再セット
                    timer = 0.25f;
                }
                else
                {
                    // タイマー更新
                    timer -= Time.deltaTime;
                }
            }

            // 移動量を作る(下降中はまっすぐ移動)
            Vector3 move = new Vector3(
                diffPos.x,
                velocityY,
                initZ * 2f
            );

            // このフレームの高さを保存
            lastY = transform.position.y;

            // 移動
            controller.Move(move * Time.deltaTime);

            yield return null;
        }

        // 接触ダメージ無効化
        if (damageArea != null)
        {
            damageArea.enabled = false;
        }

        // アニメーションを戻す
        animator.SetBool("jump", false);

        // NavMeshAgentを戻す
        agent.enabled = true;

        watchTime = 0;
        isAttack = false;
    }

    // 攻撃待機時間をセット
    void SetAttackWaitTime()
    {
        attackWaitTime = Random.Range(0.2f, 1.2f);
    }
}
