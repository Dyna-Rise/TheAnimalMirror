using System.Collections;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager Instance; //自分自身のインスタンス
    public Transform currentPlayer; //現在選択されているキャラクターの位置情報

    void Awake()
    {
        //もしPlayerManagerの生成が初めてなら
        if (Instance == null)
        {
            Instance = this; //最初のPlayerManagerをstaticのInstanceとする
        }

    }

    //引数にいれたキャラクターの位置情報をcurrentPlayerにセット
    public void SetCurrentPlayer(GameObject player)
    {
        currentPlayer = player.transform;
    }

    //プレイヤー情報(Transform)を獲得
    public Transform GetPlayer()
    {
        return currentPlayer;
    }



    //ゲームオ―バー処理
    //ステータスをゲームオーバーにして、2.5秒後に削除
    public static void GameOver(MonoBehaviour runner, GameObject target)
    {
        GameController.gameState = GameState.gameover;
        //Debug.Log(GameController.gameState);
        runner.StartCoroutine(DestroyStart(target));
    }

    private static IEnumerator DestroyStart(GameObject target)
    {
        yield return new WaitForSeconds(2.5f);
        UnityEngine.Object.Destroy(target);
    }

}