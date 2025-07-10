using System.Collections;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public float detectRadius = 10f;
    public EnemyType targetType = EnemyType.TypeB; // 検索したいタイプ
    public int targetEnemyId = 1; // 検索したいEnemyId
    public GameObject enemyPrefab;
    public LayerMask enemyLayer; // Enemy用レイヤーを指定

    public GameObject spawnEffectPrefab; //生成時エフェクト
    public AudioClip spawnSE;   // 生成時SE
    public AudioSource audioSource; // 再生用AudioSource

    void Start()
    {
        StartCoroutine(StartGenerator());
    }

    IEnumerator StartGenerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f); // 3秒ごと
            yield return StartCoroutine(CheckAndSpawn());
        }
    }

    IEnumerator CheckAndSpawn()
    {
        // 索敵
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, enemyLayer);

        bool found = false;

        foreach (var hit in hits)
        {
            EnemyInfo info = hit.GetComponent<EnemyInfo>();
            if (info != null)
            {
                if (info.enemyType == targetType && info.enemyId == targetEnemyId)
                {
                    found = true;
                    break;
                }
            }
        }

        if (!found)
        {
            // 敵を生成
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            // SEを再生
            if (spawnSE != null && audioSource != null)
            {
                audioSource.PlayOneShot(spawnSE);
            }

            // エフェクトを生成
            if (spawnEffectPrefab != null)
            {
                GameObject fx = Instantiate(
    　　　　　　spawnEffectPrefab,
    　　　　　　transform.position,
    　　　　　　spawnEffectPrefab.transform.rotation );
                Destroy(fx, 1.5f);
            }


            EnemyInfo info = enemy.GetComponent<EnemyInfo>();
            if (info != null)
            {
                info.enemyType = targetType;
                info.enemyId = targetEnemyId;
            }


        }

        yield return null;
    }
}
