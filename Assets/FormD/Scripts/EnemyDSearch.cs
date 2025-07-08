using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDSearch : MonoBehaviour
{
    bool isAttack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.parent.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            isAttack = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.parent.LookAt(other.transform.position);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.parent.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
            isAttack = false;
        }
    }

    public bool IsAttack()
    {
        return isAttack;
    }
}
