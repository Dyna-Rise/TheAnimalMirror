using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDShooter : MonoBehaviour
{
    const int MaxShootStock = 5;
    const int RecoverySeconds = 1; // 回復時間定数

    int currentShootStock = MaxShootStock;

    public GameObject carrotPrefab; // Carrotプレハブ
    public float shootSpeed;

    GameObject parentObj;
    PlayerDController playerDController;
    Animator anime;
    public EnemyDSearch enemyDSearch;

    bool inAttack; // 攻撃中かどうかのフラグ

    // Start is called before the first frame update
    void Start()
    {
        parentObj = transform.parent.gameObject;
        playerDController = parentObj.GetComponent<PlayerDController>();
        anime = parentObj.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.gameState != GameState.playing) return;

        if (enemyDSearch.IsAttack()) ShootStart();
    }

    public void ShootStart()
    {
        //if (enemyDSearch.IsAttack()) return;
        if (currentShootStock <= 0 || inAttack) return;

        anime.SetTrigger("attack");

        // プレハブからCarrotオブジェクトを生成
        GameObject carrot = Instantiate(
            carrotPrefab,
            transform.position,
            Quaternion.identity
        );

        // CarrotオブジェクトのRigidbodyを取得し力を加える
        Rigidbody carrotRigidBody = carrot.GetComponent<Rigidbody>();
        carrotRigidBody.AddForce(transform.forward * shootSpeed, ForceMode.Impulse);

        Destroy(carrot, 1.0f);

        // currentShootStockを消費
        ConsumePower();

        // サウンドを再生
        //shotSound.Play();
    }

    void ConsumePower()
    {
        // currentShootStockを消費すると同時に回復のカウントをスタート
        inAttack = true;
        StartCoroutine(ShootAttack());

        currentShootStock--;
        StartCoroutine(ShootStockRecovery());
    }

    IEnumerator ShootAttack()
    {
        yield return new WaitForSeconds(0.5f);
        inAttack = false;
    }

    IEnumerator ShootStockRecovery()
    {
        // 一定秒数待った後にcurrentShootStockを回復
        yield return new WaitForSeconds(RecoverySeconds);
        currentShootStock++;
    }
}
