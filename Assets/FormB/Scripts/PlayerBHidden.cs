using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBHidden : MonoBehaviour
{
    public GameObject normalObj;
    public GameObject hiddenObj;
    public float hiddenTime = 3.0f;
    public static bool isPlayerHide = false;

    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(HiddenStart());

        }



    }

    IEnumerator HiddenStart()
    {
        GameController.isPlayerHide = true;

        GameController.gameState = GameState.hidden;
        
        normalObj.SetActive(false);
        hiddenObj.SetActive(true);


        yield return new WaitForSeconds(hiddenTime);

        normalObj.SetActive(true);
        hiddenObj.SetActive(false);
        
        GameController.isPlayerHide = false;
        GameController.gameState = GameState.playing;
    }
    
}


