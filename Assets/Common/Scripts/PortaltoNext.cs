using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextStage : MonoBehaviour
{
    public string sceneName; //ジャンプ先
    Coroutine nextStage; //コルーチン情報

    public GameObject fadeCanvas; //対象Canvas
    public Image fadeImage;         // 白い膜
    public float fadeDuration = 2f; // フェードにかける時間

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("触れた");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("プレイヤーと触れた");
            if(nextStage == null) 
            nextStage = StartCoroutine(ToNextStage());
        }
    }

    IEnumerator ToNextStage()
    {
        fadeCanvas.SetActive(true);
        float time = 0f;
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(time / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
