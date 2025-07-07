using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBall : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    //ぶつかった相手がプレイヤーなら即消滅
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
