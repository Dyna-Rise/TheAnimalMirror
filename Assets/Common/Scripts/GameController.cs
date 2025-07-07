using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    playing,
    attack,
    hidden,
    pause,
    gameover,
    gameclear,
    end
}

//変身フォームの種類
public enum PlayerForm
{
    Normal,
    FormA,
    FormB,
    FormC,
    FormD
}

//敵のタイプ
public enum EnemyType
{
    None,
    TypeA,
    TypeB,
    TypeC,
    TypeD
}

public class GameController : MonoBehaviour
{
    public static GameState gameState;　//ゲームステータス

    const int PlayerLife = 5; //プレイヤーのLifeの定数
    public static int playerLife; //変動するLife

    public static bool isPlayerHide; //プレイヤーが隠れているかどうか

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.playing;
        
        //プレイヤーライフのリセット
        playerLife = PlayerLife;
        //Debug.Log(playerLife);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
