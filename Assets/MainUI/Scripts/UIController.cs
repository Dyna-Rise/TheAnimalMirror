using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Sprite[] playerFormImages; // �v���C���[�摜�z��

    // �v���C���[�摜�C���f�b�N�X�ϊ��p
    Dictionary<PlayerForm, int> imageIndexByForm = new()
    {
        { PlayerForm.Normal, 0 },
        { PlayerForm.FormA,  1 },
        { PlayerForm.FormB,  2 },
        { PlayerForm.FormC,  3 },
        { PlayerForm.FormD,  4 },
    };

    public TextMeshProUGUI timeText; // �\���p�c�莞��
    float currentTime; // ���݂̎c�莞��

    public Slider lifeSlider; // �v���C���[�̃��C�t
    int currentLife; // ���݂̃v���C���[�̃��C�t

    public Image formImageContainer; // �\���摜
    PlayerForm currentPlayerForm; // ���݂̃v���C���[

    PlayerChange playerChange;
    TimeController timeController;

    // Start is called before the first frame update
    void Start()
    {
        playerChange = GameObject.Find("Player").GetComponent<PlayerChange>();
        timeController = GetComponent<TimeController>();

        if (playerFormImages.Length == 0)
        {
            Debug.LogError("playerFormImages���w�肵�Ă�������");
            enabled = false;
            return;
        }

        // ���C�t�����l�Z�b�g
        Invoke("SetDefaultLife", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // �c�莞�Ԃ��X�V
        if (currentTime != timeController.remainingTime || timeText == null)
        {
            currentTime = timeController.remainingTime;
            timeText.text = Mathf.Ceil(currentTime).ToString();
        }

        // �c�胉�C�t�X�V
        if (currentLife != GameController.playerLife)
        {
            currentLife = GameController.playerLife;
            lifeSlider.value = currentLife;
        }

        // �v���C���[�摜���X�V
        if (currentPlayerForm != playerChange.playerForm)
        {
            currentPlayerForm = playerChange.playerForm;
            int index = imageIndexByForm[currentPlayerForm];
            formImageContainer.sprite = playerFormImages[index];
        }
    }

    // ���C�t�����l�Z�b�g
    void SetDefaultLife()
    {
        lifeSlider.minValue = 0;
        lifeSlider.maxValue = GameController.playerLife;
        lifeSlider.value = GameController.playerLife;
    }
}
