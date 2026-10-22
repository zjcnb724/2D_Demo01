using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackBox : MonoBehaviour
{
    public float demage = 10f;
    public float destroyTime = 0.5f;
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }


    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.GetHit(demage);
        }
    }
}
