using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerATackle : MonoBehaviour
{
    public Collider normalCollider1;
    public Collider normalCollider2;
    public Collider tackleCollider;

    string originalTag;

    Animator animator;

    public Rigidbody rbody;

    

    // Start is called before the first frame update
    void Start()
    { 
        tackleCollider.enabled = false;
        animator = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        originalTag = gameObject.tag;
        if (originalTag == "PlayerTackle") return;
        else if (originalTag == "Player" )
        {
            if (Input.GetMouseButtonDown(0))
            {
                
                StartCoroutine(TackleAttack());
                
                
            }
        }

    }

    IEnumerator TackleAttack()
    {
        float duration = 0.5f;
        float timer = 0f;

        GameController.gameState = GameState.attack;
        tackleCollider.enabled = true;
        this.tag = "PlayerTackle";
        animator.SetTrigger("attack");

      
        yield return new WaitForSeconds(duration);


        GameController.gameState = GameState.playing;
        tackleCollider.enabled = false;
        this.tag = "Player";
    }

}
