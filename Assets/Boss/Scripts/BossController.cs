using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    public float tackleDistance = 8f;

    public float shootInterval = 4f;
    public float dashInterval = 5f;

    public float tackleInterval = 8f;
    float nextInterval;

    public GameObject bulletPrefab;
    public Transform shootPoint;

    private NavMeshAgent agent;

    private Coroutine currentCoroutine = null;
    private Coroutine actionCoroutine = null;

    //private bool isInvisible;

    private EnemyDamaged enemyDamaged;
    private int initialLife;

    public CapsuleCollider body1;
    public BoxCollider body2;
    public BoxCollider attackBody;

    Animator anime;

    public GameObject visibleBody;  // 元の見えるボディ
    public GameObject invisibleBody; // 透明ボディ

    public float escapeDistance = 15f;
    public float escapeDuration = 2f;

    void Start()
    {
        anime = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();

        enemyDamaged = GetComponent<EnemyDamaged>();
        if (enemyDamaged != null)
        {
            initialLife = enemyDamaged.enemyLife;
        }
        else
        {
            Debug.LogError("EnemyDamaged コンポーネントが見つかりません！");
        }
    }

    void Update()
    {
        if (PlayersManager.Instance == null || PlayersManager.Instance.currentPlayer == null) return;
        if (enemyDamaged == null) return;


        Transform player = PlayersManager.Instance.currentPlayer;

        //コルーチンが走っている時は向きが止まる
        if (currentCoroutine == null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f; // 水平方向のみ回転

            if (direction.magnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }

        if (currentCoroutine != null) return; // 行動中は他のアクション不可
       
        if(actionCoroutine == null)
        {
            actionCoroutine = StartCoroutine(AttackCoroutine(player));
        }

    }

    IEnumerator AttackCoroutine(Transform player)
    {
        bool isLowHP = enemyDamaged.enemyLife <= initialLife / 2;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > tackleDistance)
        {
            if (!isLowHP)
            {
                int rand = Random.Range(0, 2);
                if(rand == 0)  // 1/2の確率でダッシュしてくる
                {
                    currentCoroutine = StartCoroutine(DashToPlayerCoroutine(player));
                    nextInterval = dashInterval;
                }
                else
                {
                    currentCoroutine = StartCoroutine(ShootAtPlayerCoroutine());
                    nextInterval = shootInterval;
                }
            }
            else
            {
                currentCoroutine = StartCoroutine(ShootAtPlayerCoroutine());
                nextInterval = shootInterval;
            }
        }
        else
        {
            if (isLowHP)
            {
                int rand = Random.Range(0, 3);
                if (rand == 0) //　1/3の確率で逃げる
                {
                    currentCoroutine = StartCoroutine(DisappearAndTeleportAwayCoroutine(player));
                    nextInterval = tackleInterval;
                }
                else
                {
                    currentCoroutine = StartCoroutine(TackleAndReturnCoroutine(player));
                    nextInterval = tackleInterval;
                }
            }
            else
            {
                currentCoroutine = StartCoroutine(TackleAndReturnCoroutine(player));
                nextInterval = tackleInterval;
            }
        }

        yield return new WaitForSeconds(nextInterval);
        actionCoroutine = null;
    }

    IEnumerator ShootAtPlayerCoroutine()
    {
        yield return new WaitForSeconds(1);

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        Vector3 targetPos = PlayersManager.Instance.currentPlayer.position + (Vector3.up * 0.5f);
        Vector3 dir = (targetPos - shootPoint.position).normalized;
        bullet.GetComponent<Rigidbody>().AddForce(dir * 15f, ForceMode.Impulse);

        currentCoroutine = null;
    }

    IEnumerator TackleAndReturnCoroutine(Transform player)
    {
        //Debug.Log("突進前：一瞬だけ後退");
        agent.isStopped = true; // NavMeshAgent 無効化

        float preBackSpeed = 1.2f;
        float preBackDuration = 2.0f;
        float elapsed = 0f;

        anime.SetBool("walk", true);

        while (elapsed < preBackDuration)
        {
            transform.position -= transform.forward * preBackSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        //Debug.Log("タックル突進");
        agent.isStopped = true; // NavMeshAgent 無効化

        float tackleSpeed = 8.0f;
        float tackleDuration = 1.0f;
        elapsed = 0f;

        string currentTag = gameObject.tag;
        body1.enabled = false;
        body2.enabled = false;
        attackBody.enabled = true;
        gameObject.tag = "EnemyBullet";

        anime.SetTrigger("attack");

        while (elapsed < tackleDuration)
        {
            transform.position += transform.forward * tackleSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        //Debug.Log("後退開始");
        body1.enabled = true;
        body2.enabled = true;
        attackBody.enabled = false;
        gameObject.tag = "Enemy";

        float backSpeed = 5f;
        float backDuration = 1.0f;
        elapsed = 0f;

        while (elapsed < backDuration)
        {
            transform.position -= transform.forward * backSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        //Debug.Log("戻り完了");

        anime.SetBool("walk", false);

        currentCoroutine = null;
    }

    IEnumerator DashToPlayerCoroutine(Transform player)
    {
        agent.isStopped = true;
        anime.SetBool("wait",true);

        // ダッシュ準備（少し前傾 or SE）
        yield return new WaitForSeconds(1.0f);

        // タグと攻撃当たり判定セット
        string originalTag = gameObject.tag;
        body1.enabled = false;
        body2.enabled = false;
        attackBody.enabled = true;
        gameObject.tag = "EnemyBullet";

        // 向きをプレイヤーに固定（突進方向）
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        // アニメーション再生（例: dash）
        anime.SetBool("dash",true);

        // 前方に突進！
        float dashSpeed = 15f;
        float dashTime = 1.2f;
        float elapsed = 0f;

        while (elapsed < dashTime)
        {
            transform.position += transform.forward * dashSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ダッシュ後の処理（その場停止・復帰）
        body1.enabled = true;
        body2.enabled = true;
        attackBody.enabled = false;
        gameObject.tag = originalTag;

        anime.SetBool("dash", false);
        yield return new WaitForSeconds(1.0f);

        anime.SetBool("wait", false);

        currentCoroutine = null;
    }

    IEnumerator DisappearAndTeleportAwayCoroutine(Transform player)
    {
        GetComponent<EnemyDamaged>().enabled = false;
        //isInvisible = true;

        visibleBody.SetActive(false);
        invisibleBody.SetActive(true);
        gameObject.tag = "Untagged";

        agent.isStopped = false;

        // ランダムな方向へ逃げる
        Vector3 randomDir = (transform.position - player.position).normalized;
        randomDir = Quaternion.Euler(0, Random.Range(-60f, 60f), 0) * randomDir; // プレイヤーの背後～横方向へ逃げる

        Vector3 targetPos = transform.position + randomDir * escapeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        float elapsed = 0f;
        while (elapsed < escapeDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }


        // 移動停止・元に戻す
        GetComponent<EnemyDamaged>().enabled = true;

        agent.isStopped = true;
        visibleBody.SetActive(true);
        invisibleBody.SetActive(false);

        //isInvisible = false;
        currentCoroutine = null;
    }
}
