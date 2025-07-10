using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    Transform player; // プレイヤー
    public Transform boss;   // ボス
    public float distanceFromPlayer = 2.0f; // 矢印の距離

    void Update()
    {
        //プレイヤーの取得
        player = PlayersManager.Instance.currentPlayer;

        if (player == null || boss == null) return;

        // ボスの方向ベクトルを正規化しておく
        Vector3 dirToBoss = (boss.position - player.position).normalized;

        // 矢印の位置を、プレイヤーからボス方向に一定距離だけ離す
        transform.position = (player.position + new Vector3(0,0.5f,0)) + dirToBoss * distanceFromPlayer;

        // ボスの方向を向く回転を作成（Y軸のみ向かせる）
        Vector3 flatDir = dirToBoss;
        flatDir.y = 0f;

        if (flatDir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(flatDir);

            // X軸90度を加える：オブジェクトがもともとX=90度必要な場合
            transform.rotation = lookRotation * Quaternion.Euler(90, 0, 0);
        }
    }
}
