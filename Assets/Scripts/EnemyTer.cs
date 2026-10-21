using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTer : MonoBehaviour
{
    public EnemyBase enemyBase;
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            enemyBase.FindPlayer(collider.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            enemyBase.PlayerOut();
        }
    }

}
