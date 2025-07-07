using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnNormal : MonoBehaviour
{
    public float returnTime = 20.0f;
    public PlayerChange playerChange;
    float currentTime;
    public bool isCountDown;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = returnTime;    
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCountDown) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0 && (GameController.gameState == GameState.playing))
        {
            isCountDown = false;
            currentTime = returnTime;
            playerChange.ChangeForm(PlayerForm.Normal);
            GameController.isPlayerHide = false;
        }
    }
}
