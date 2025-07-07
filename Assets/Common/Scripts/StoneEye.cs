using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StoneEye : MonoBehaviour
{
    public float searchDistance = 10.0f; //探索距離
    public LayerMask searchLayer; //対象のレイヤー

    public GameObject bulletPrefabs; //生成する弾
    public GameObject gate; //発射口

    bool isDiscovered; //発見状態化どうか
    GameObject player; //対象プレイヤー
    public float bulletSpeed; //弾丸の速度

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        bool search = Physics.Raycast(
            transform.position + new Vector3(0, 0.5f, 0), //どこから
            transform.forward, //どの方角に
            searchDistance, //どのくらいの距離
            searchLayer　//どのレイヤーを見つけるか
            );


        if (search && !isDiscovered && !GameController.isPlayerHide)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            InvokeRepeating("ShootEnemyBullet", 0.5f, 0.1f); //一度発見されて以降は弾を連射
            isDiscovered = true; //サーチがtrueになったら発見済み
        }

    }

    //デスボールを射出
    void ShootEnemyBullet()
    {
        if (player != null)
        {
            if (!GameController.isPlayerHide)
            {
                //ゲートの位置から発射
                GameObject deathBall = Instantiate(bulletPrefabs, gate.transform.position, Quaternion.identity);
                Rigidbody rbody = deathBall.GetComponent<Rigidbody>();

                //プレイヤーの方向に飛ばす
                Vector3 v = (player.transform.position - transform.position).normalized;
                rbody.AddForce(v * bulletSpeed, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 fromPos = transform.position + new Vector3(0, 0.5f, 0);
        Vector3 toPos = fromPos + transform.forward * searchDistance;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(fromPos, toPos);
    }
}
