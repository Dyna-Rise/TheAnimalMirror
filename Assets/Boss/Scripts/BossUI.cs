using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    public Slider bossSlider;
    public EnemyDamaged bossDamaged;

    int currentBossLife;

    public GameObject bossLifePanel;

    // Start is called before the first frame update
    void Start()
    {
        currentBossLife = bossDamaged.enemyLife;
        bossSlider.maxValue = currentBossLife;
        bossSlider.value = currentBossLife;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentBossLife != bossDamaged.enemyLife)
        {
            currentBossLife = bossDamaged.enemyLife;
            bossSlider.value = currentBossLife;


            if (bossDamaged.enemyLife <= 0) Invoke("HiddenBossLife",2.0f);
        }
    }

    void HiddenBossLife()
    {
        bossLifePanel.SetActive(false);
    }
}
