using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    public GameObject gameOverPanel;

    GameState currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = GameController.gameState;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != GameController.gameState)
        {
            currentState = GameController.gameState;
            //Debug.Log("current:" + currentState);
            switch (currentState)
            {
                case GameState.gameover:
                    gameOverPanel.SetActive(true);
                    break;
            }
        }
    }
}
