using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public float remainingTime = 100; // �c�莞��

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // �o�ߎ��Ԃ𔽉f
        remainingTime -= Time.deltaTime;

        // 0�ɂȂ�����Q�[���I�[�o�[
        if (remainingTime < 0)
        {
            remainingTime = 0;
            GameController.gameState = GameState.gameover;
        }
    }
}
