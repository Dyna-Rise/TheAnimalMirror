using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChange : MonoBehaviour
{
    public GameObject[] Forms; //配列で作成したオブジェクト達を管理
    public GameObject currentForm; //現状のフォーム

    public GameObject transformEffect; //変身時に使うエフェクト

    public PlayerSEPlay playerSePlay;

    EnemyType enemyType; //TriggerJudgeクラス経由で獲得した敵のFormタイプ
    public PlayerForm playerForm; //enemyTypeにあわせたプレイヤーのForm

    void Start()
    {
        //最初期のフォームはForms配列先頭のオブジェクト（NormalPlayer）
        currentForm = Forms[0]; 

        //PlayerManagerクラスの力でcurerntFormのキャラのTransformをPlayerManagerのcurrentFormにセット
        PlayersManager.Instance.SetCurrentPlayer(currentForm); 
    }

    void Update()
    {
        //右クリックを押す　&& Normalの姿であれば
        if (Input.GetKeyDown(KeyCode.Mouse1) && currentForm == Forms[0])
        {
            //ゲームオーバーでなければ
            if (GameController.playerLife <= 0) return;

            // NormalPlayerにTriggerJudge が存在するか確認
            TriggerJudge trigger = currentForm.GetComponent<TriggerJudge>();
 
            //Triggerがなければ無効
            if (trigger == null)
            {
                Debug.LogWarning("TriggerJudge が currentForm に見つかりません。");
                return;
            }
            //TriggerJudgeのisApproachがONなら（敵の近くであれば）
            else if (TriggerJudge.isApproach)
            {
                // enemyForm(近くにいる敵のForm情報）が null でも問題ないように対処
                if (trigger.enemyType != EnemyType.None)
                {
                    //自作列挙型のFormの値をValueで取り出す
                    enemyType = trigger.enemyType;

                    //enemyTypeに応じてPlayerのFormも決める
                    switch (enemyType)
                    {
                        case EnemyType.None:
                            playerForm = PlayerForm.Normal;
                            break;
                        case EnemyType.TypeA:
                            playerForm = PlayerForm.FormA;
                            break;
                        case EnemyType.TypeB:
                            playerForm = PlayerForm.FormB;
                            break;
                        case EnemyType.TypeC:
                            playerForm = PlayerForm.FormC;
                            break;
                        case EnemyType.TypeD:
                            playerForm = PlayerForm.FormD;
                            break;
                        default:
                            playerForm = PlayerForm.Normal;
                            break;
                    }

                    //playerForm型の値を引数にメソッド発動して変身
                    ChangeForm(playerForm);
                    playerSePlay.PlaySe("henshin5");
                }
                else
                {
                    Debug.LogWarning("enemyType が null です。");
                }
            }
        }
    }

    //変身メソッド（引数にForm型の値を指定）
    public void ChangeForm(PlayerForm fm)
    {
        //現在のForm(NormalPlayer)の位置を取得
        Vector3 currentPos = currentForm.transform.position;

        // 一旦すべてのオブジェクトを非表示（無効）
        for (int i = 0; i < Forms.Length; i++) Forms[i].SetActive(false);

        // あらためて対象をアクティブする
        int index = (int)fm; //Form型の番号をintに変換
        if (index >= 0 && index < Forms.Length)
        {
            //次の対象オブジェクトをnextに獲得
            GameObject next = Forms[index];
            //新しく選ばれたオブジェクトの位置をもとのNormalPlayerの位置と同じにする
            next.transform.position = currentPos;　

            next.SetActive(true); //オブジェクトを表示（有効）
            

            currentForm = next; //currentFormに新しく選ばれたオブジェクト情報を格納


            // Form1〜4の場合は変身時間のカウントダウンのフラグをON
            //ReturnNormalコンポーネントのカウントダウンを開始
            if (fm != PlayerForm.Normal)
            {
                var rn = currentForm.GetComponent<ReturnNormal>();
                if (rn != null)
                {
                    rn.isCountDown = true;
                }
            }
        }
        else
        {
            Debug.LogWarning("指定された Form が範囲外です");
        }        

        //変身エフェクトの発生
        StartCoroutine(TransformEffectStart(currentForm));

        //PlayerManagerが管理しているcurrentPlayer(Transform型)に情報を格納
        PlayersManager.Instance.SetCurrentPlayer(currentForm); 

    }

    //変身エフェクトのスタート
    IEnumerator TransformEffectStart(GameObject shilhuette)
    {
            transformEffect.transform.position = shilhuette.transform.position + new Vector3(0, 0.6f, 0);
        transformEffect.SetActive(true);
        transformEffect.transform.SetParent(shilhuette.transform);
        yield return new WaitForSeconds(0.2f);
        transformEffect.SetActive(false);
        transformEffect.transform.SetParent(transform);
    }


}
