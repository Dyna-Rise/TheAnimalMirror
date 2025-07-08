using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public int arrangeId; //識別ID
    public string Enemytag; //タグ名
    public float searchDistance = 4.0f;

    bool isActive;

    float axisH, axisV; //横軸、縦軸の値
    GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        if (!(enemy == null)) return;

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(HiddenStart());

        }


        if (isActive)
        {
            //プレイヤーの方角をX成分、Y成分で取得
            float dirX = enemy.transform.position.x - transform.position.x;
            float dirY = enemy.transform.position.y - transform.position.y;

            //プレイヤーとの差分をもとに角度（ラジアン）を算出
            float angle = Mathf.Atan2(dirY, dirX);

            //長辺を1として数値をおさえた際のXとYを求め直す
            axisH = Mathf.Cos(angle); //X方向
            axisV = Mathf.Sin(angle); //Y方向

            SearchPlayer();
        }
        else
        {
            //索敵
            SearchPlayer();
        }

    }

        void SearchPlayer()
        {
            //エネミーとプレイヤー　2者間の距離
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            //基準の距離より近ければ
            if (distance <= searchDistance)
            {
                isActive = true; //追跡モード
            }
            else
            {
                isActive = false; //追跡モードOFF
                                  //Debug.Log("追跡OFF");
               
            }
        }


        IEnumerator HiddenStart()
        {
         
        yield return new WaitForSeconds(searchDistance);
        }
}
