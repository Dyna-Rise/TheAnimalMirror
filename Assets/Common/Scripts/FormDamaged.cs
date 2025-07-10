using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormDamaged : MonoBehaviour
{   
    public PlayerChange playerChange;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            playerChange.playerForm = PlayerForm.Normal;
            playerChange.ChangeForm(playerChange.playerForm);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dead"))
        {
            playerChange.playerForm = PlayerForm.Normal;
            playerChange.ChangeForm(playerChange.playerForm);
        }
    }


}
