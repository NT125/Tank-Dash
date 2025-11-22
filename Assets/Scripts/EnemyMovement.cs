using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float movementSpeed = 7f;
    public float maxDistanceFromPlayer = 25f;

    private Rigidbody2D rb;
    private Transform player;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Move();
        CheckDistance();
    }

    private void Move()
    {
        transform.position += Vector3.left * movementSpeed * Time.deltaTime;
    }

    private void CheckDistance()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;

            return;
        }

        if (Vector2.Distance(transform.position, player.position) > maxDistanceFromPlayer)
        {
            gameObject.SetActive(false);
        }
    }
}
