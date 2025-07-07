using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalPlayerDamaged : MonoBehaviour
{
    bool isDamage;
    public float damageTime = 0.2f;
    public SkinnedMeshRenderer body;
    public float knockbackForce = 10.0f;

    public NormalPlayerController controller; // Rigidbody版コントローラに変更
    private Rigidbody rb;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<NormalPlayerController>();

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDamage)
        {
            float value = Mathf.Sin(Time.time * 50.0f);
            if (body != null)
                body.enabled = value > 0;

            if (!controller.isDamage) controller.isDamage = true;
        }
        else
        {
            if (controller.isDamage) controller.isDamage = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("EnemyBullet"))
        {

            GameController.playerLife--;
            //Debug.Log(GameController.playerLife);

            if (GameController.playerLife > 0)
                DamageStart(other.transform);
            else
            {
                GetComponent<CapsuleCollider>().enabled = false;
                GetComponent<Rigidbody>().isKinematic = true;
                PlayersManager.GameOver(this, transform.parent.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dead"))
        {
            PlayersManager.GameOver(this, transform.parent.gameObject);
        }
    }

    void DamageStart(Transform enemy)
    {
        //Debug.Log("ダメージ");
        if (isDamage) return;

        isDamage = true;

        Vector3 diff = transform.position - enemy.position;
        diff.y = 0;
        Vector3 knockbackDir = diff.normalized;

        rb.velocity = new Vector3(0, rb.velocity.y, 0); // 横方向止めてから
        rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);

        Invoke(nameof(endDamage), damageTime);
    }

    void endDamage()
    {
        isDamage = false;
        if (body != null) body.enabled = true;
    }

}
