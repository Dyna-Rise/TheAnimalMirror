using System.Collections;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public float detectRadius = 10f;
    public int targetType = 0; // �����������^�C�v
    public int targetEnemyId = 1; // ����������EnemyId
    public GameObject enemyPrefab;
    public LayerMask enemyLayer; // Enemy�p���C���[���w��

    void Start()
    {
        StartCoroutine(StartGenerator());
    }

    IEnumerator StartGenerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f); // 3�b����
            yield return StartCoroutine(CheckAndSpawn());
        }
    }

    IEnumerator CheckAndSpawn()
    {
        // ���G
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
            // �G�𐶐�
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
