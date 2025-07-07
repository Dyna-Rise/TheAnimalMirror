using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    Transform player; 
    public Vector3 offset = new Vector3(0, 3, -8); // プレイヤーとの相対位置
    public float followSpeed = 5f; // カメラの追従スピード

    void Start()
    {
    }

    private void Update()
    {
        //プレイヤー探索
        player = PlayersManager.Instance.GetPlayer();
    }

    void LateUpdate()
    {
        if (player == null) return;

        // カメラの目標位置
        Vector3 targetPos = player.position + offset;

        // カメラを滑らかに追従
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // プレイヤーを見る（必要なら）
        Vector3 lookTarget = player.position + Vector3.up * 1.5f; // 1.5の高さ分だけ上を見させる
        transform.LookAt(lookTarget); transform.LookAt(player);
    }
}
