using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("何か");
        if (collision.gameObject.CompareTag("PlayerTackle"))
        {
            //Debug.Log("ぶつかった");
            Destroy(gameObject);
        }
    }
}
