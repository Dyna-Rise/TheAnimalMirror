using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDSearch : MonoBehaviour
{
    public GameObject enemyD;
    NavMeshAgent navMeshAgent;

    bool isAttack; // 攻撃するかどうかのフラグ

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = enemyD.GetComponent<NavMeshAgent>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            navMeshAgent.isStopped = true;
            isAttack = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemyD.transform.LookAt(other.transform.position);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            navMeshAgent.isStopped = false;
            isAttack = false;
        }
    }

    public bool IsAttack()
    {
        return isAttack;
    }
}
