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
            playerChange.ChangeForm(PlayerForm.Normal);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dead"))
        {
            playerChange.ChangeForm(PlayerForm.Normal);
        }
    }


}
