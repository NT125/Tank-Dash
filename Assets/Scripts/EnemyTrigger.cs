using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [SerializeField] private float offset = 15f;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameObject enemy = ObjectPool.sharedInstance.GetPooledObject();

            if (enemy != null)
            {
                Debug.Log("Creado");
                Vector2 spawnLocation = (Vector2)transform.position + Vector2.right * offset;
                enemy.transform.position = spawnLocation;

                enemy.SetActive(true);
            }
        }
    }
}
