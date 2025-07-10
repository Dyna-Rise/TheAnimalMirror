using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public float remainingTime = 100; // 残り時間

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 経過時間を反映
        remainingTime -= Time.deltaTime;

        // 0になったらゲームオーバー
        if (remainingTime < 0)
        {
            remainingTime = 0;
            GameController.gameState = GameState.gameover;
        }
    }
}
