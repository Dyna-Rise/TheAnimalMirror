using System.Collections;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public float detectRadius = 10f;
    public EnemyType targetType = EnemyType.TypeB; // �����������^�C�v
    public int targetEnemyId = 1; // ����������EnemyId
    public GameObject enemyPrefab;
    public LayerMask enemyLayer; // Enemy�p���C���[���w��

    public GameObject spawnEffectPrefab; //�������G�t�F�N�g
    public AudioClip spawnSE;   // ������SE
    public AudioSource audioSource; // �Đ��pAudioSource

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
                if (info.enemyType == targetType && info.enemyId == targetEnemyId)
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

            // SE���Đ�
            if (spawnSE != null && audioSource != null)
            {
                audioSource.PlayOneShot(spawnSE);
            }

            // �G�t�F�N�g�𐶐�
            if (spawnEffectPrefab != null)
            {
                GameObject fx = Instantiate(
    �@�@�@�@�@�@spawnEffectPrefab,
    �@�@�@�@�@�@transform.position,
    �@�@�@�@�@�@spawnEffectPrefab.transform.rotation );
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
