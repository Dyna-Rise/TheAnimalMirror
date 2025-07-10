using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDShooter : MonoBehaviour
{
    const int MaxShootStock = 5;
    const int RecoverySeconds = 3; // 回復時間定数

    int currentShootStock = MaxShootStock;
    //AudioSource shotSound;

    public GameObject carrotPrefab; // Carrotプレハブ
    public float shootSpeed;

    public GameObject enemyD;
    EnemyDController enemyDController;
    Animator anime;
    public EnemyDSearch enemyDSearch;

    bool inAttack; // 攻撃中かどうかのフラグ

    // Start is called before the first frame update
    void Start()
    {
        enemyDController = enemyD.GetComponent<EnemyDController>();
        anime = enemyD.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.gameState != GameState.playing) return;

        //if (enemyDSearch.IsAttack()) ShootAttack();
    }

    public void ShootAttack()
    {
        // 残弾数がなくて攻撃中ならシュートしない
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
        StartCoroutine(ShootStart());

        currentShootStock--;
        StartCoroutine(ShootStockRecovery());
    }

    IEnumerator ShootStart()
    {
        yield return new WaitForSeconds(2.5f);
        inAttack = false;
    }

    IEnumerator ShootStockRecovery()
    {
        // 一定秒数待った後にcurrentShootStockを回復
        yield return new WaitForSeconds(RecoverySeconds);
        currentShootStock++;
    }
}
