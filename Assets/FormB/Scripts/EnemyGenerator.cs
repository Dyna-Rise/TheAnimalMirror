using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public int arrangeId; //����ID
    public string Enemytag; //�^�O��
    public float searchDistance = 4.0f;

    bool isActive;

    float axisH, axisV; //�����A�c���̒l
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
            //�v���C���[�̕��p��X�����AY�����Ŏ擾
            float dirX = enemy.transform.position.x - transform.position.x;
            float dirY = enemy.transform.position.y - transform.position.y;

            //�v���C���[�Ƃ̍��������ƂɊp�x�i���W�A���j���Z�o
            float angle = Mathf.Atan2(dirY, dirX);

            //���ӂ�1�Ƃ��Đ��l�����������ۂ�X��Y�����ߒ���
            axisH = Mathf.Cos(angle); //X����
            axisV = Mathf.Sin(angle); //Y����

            SearchPlayer();
        }
        else
        {
            //���G
            SearchPlayer();
        }

    }

        void SearchPlayer()
        {
            //�G�l�~�[�ƃv���C���[�@2�ҊԂ̋���
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            //��̋������߂����
            if (distance <= searchDistance)
            {
                isActive = true; //�ǐՃ��[�h
            }
            else
            {
                isActive = false; //�ǐՃ��[�hOFF
                                  //Debug.Log("�ǐ�OFF");
               
            }
        }


        IEnumerator HiddenStart()
        {
         
        yield return new WaitForSeconds(searchDistance);
        }
}
