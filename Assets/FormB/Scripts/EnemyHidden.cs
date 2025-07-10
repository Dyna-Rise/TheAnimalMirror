using System.Collections;
using UnityEngine;

public class EnemyHidden : MonoBehaviour
{
    public GameObject normalObj; // �ʏ�̌�����
    public GameObject hiddenObj; // �����̌�����

    public float hiddenTime = 3.0f;

    public void HideEnemy(float duration = -1f)
    {
        if (duration > 0f)
            StartCoroutine(HiddenStart(duration));
        else
            StartCoroutine(HiddenStart(hiddenTime));
    }

    IEnumerator HiddenStart(float time)
    {
        normalObj.SetActive(false);
        hiddenObj.SetActive(true);

        yield return new WaitForSeconds(time);

        normalObj.SetActive(true);
        hiddenObj.SetActive(false);
    }
}
