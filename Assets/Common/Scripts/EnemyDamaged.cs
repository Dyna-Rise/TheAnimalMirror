using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamaged : MonoBehaviour
{
    public int enemyLife = 3;
    public GameObject enemyModel; // 表示/非表示を切り替える対象

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet") || collision.gameObject.CompareTag("PlayerTackle"))
        {
            if (collision.gameObject.CompareTag("PlayerTackle"))
            {
                StartCoroutine(ColliderStop());
            }

            enemyLife--;

            // 点滅開始
            StartCoroutine(BlinkEffect());

            if (enemyLife <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator ColliderStop()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        GetComponent<CapsuleCollider>().enabled = true;
    }

    IEnumerator BlinkEffect(float blinkTime = 1.0f, float interval = 0.1f)
    {
        float timer = 0f;

        while (timer < blinkTime)
        {
            enemyModel.SetActive(false);
            yield return new WaitForSeconds(interval);
            enemyModel.SetActive(true);
            yield return new WaitForSeconds(interval);
            timer += interval * 2;
        }
    }
}
