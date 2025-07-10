using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Sprite[] playerFormImages; // プレイヤー画像配列

    // プレイヤー画像インデックス変換用
    Dictionary<PlayerForm, int> imageIndexByForm = new()
    {
        { PlayerForm.Normal, 0 },
        { PlayerForm.FormA,  1 },
        { PlayerForm.FormB,  2 },
        { PlayerForm.FormC,  3 },
        { PlayerForm.FormD,  4 },
    };

    public TextMeshProUGUI timeText; // 表示用残り時間
    float currentTime; // 現在の残り時間

    public Slider lifeSlider; // プレイヤーのライフ
    int currentLife; // 現在のプレイヤーのライフ

    public Image formImageContainer; // 表示画像
    PlayerForm currentPlayerForm; // 現在のプレイヤー

    PlayerChange playerChange;
    TimeController timeController;

    // Start is called before the first frame update
    void Start()
    {
        playerChange = GameObject.Find("Player").GetComponent<PlayerChange>();
        timeController = GetComponent<TimeController>();

        if (playerFormImages.Length == 0)
        {
            Debug.LogError("playerFormImagesを指定してください");
            enabled = false;
            return;
        }

        // ライフ初期値セット
        Invoke("SetDefaultLife", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // 残り時間を更新
        if (currentTime != timeController.remainingTime || timeText == null)
        {
            currentTime = timeController.remainingTime;
            timeText.text = Mathf.Ceil(currentTime).ToString();
        }

        // 残りライフ更新
        if (currentLife != GameController.playerLife)
        {
            currentLife = GameController.playerLife;
            lifeSlider.value = currentLife;
        }

        // プレイヤー画像を更新
        if (currentPlayerForm != playerChange.playerForm)
        {
            currentPlayerForm = playerChange.playerForm;
            int index = imageIndexByForm[currentPlayerForm];
            formImageContainer.sprite = playerFormImages[index];
        }
    }

    // ライフ初期値セット
    void SetDefaultLife()
    {
        lifeSlider.minValue = 0;
        lifeSlider.maxValue = GameController.playerLife;
        lifeSlider.value = GameController.playerLife;
    }
}
