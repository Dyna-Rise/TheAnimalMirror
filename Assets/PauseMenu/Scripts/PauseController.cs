using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public GameObject pausePanel;

    bool inPause; // ポーズ中かどうかのフラグ

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!inPause)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    void Pause()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        inPause = true;
    }

    void Resume()
    {
        Time.timeScale = 1.0f;
        pausePanel.SetActive(false);
        inPause = false;
    }
}
