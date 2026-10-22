using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBox : MonoBehaviour
{
    public float demage = 10f;
    public float destroyTime = 0.5f;
    void Start()
    {
        Destroy(gameObject,destroyTime);
    }

    
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            enemy.GetHit(demage);
        }
    }
}
