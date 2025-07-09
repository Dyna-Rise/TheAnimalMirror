using System.Collections;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public float detectRadius = 10f;
    public int targetType = 0; // 検索したいタイプ
    public int targetEnemyId = 1; // 検索したいEnemyId
    public GameObject enemyPrefab;
    public LayerMask enemyLayer; // Enemy用レイヤーを指定

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
                if ((int)info.enemyType == targetType && info.enemyId == targetEnemyId)
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
            EnemyInfo info = enemy.GetComponent<EnemyInfo>();
            if (info != null)
            {
                info.enemyType = (EnemyType)targetType;
                info.enemyId = targetEnemyId;
            }
        }

        yield return null;
    }
}
