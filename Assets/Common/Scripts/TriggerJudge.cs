using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerJudge : MonoBehaviour
{
    public static bool isApproach;
    public EnemyType enemyType;

    private void Start()
    {
        enemyType = EnemyType.None;
    }

    //敵が領域にいる時
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            isApproach = true; //フラグをON
            //相手の列挙型の種類を取得（Form0～Form4)
            enemyType = other.GetComponent<EnemyInfo>().enemyType;
        }
    }

    //敵が領域から抜けた時
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            isApproach = false; //フラグをOFF
            enemyType = EnemyType.None; //列挙型の値をNormalに
        }


    }
}
