using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float jumpForce = 21f;
    [SerializeField] private float movementSpeed = 3f;
    private Rigidbody2D playerRB;
    private bool isOnGround;
    public AudioSource[] playerAS; // Para obtener un array que contenga los AudioSources del objeto

    // Game Loop
    void Start()
    {
        playerAS = GetComponents<AudioSource>();
        playerRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump();
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpForce);
            playerAS[0].Play();
            isOnGround = false;
        }
    }

    private void Move()
    {
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            playerRB.transform.Translate(Vector2.right * movementSpeed * Time.deltaTime);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            playerRB.transform.Translate(Vector2.left * movementSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // Chequeando colisión con objetos dañinos
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Spike"))
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, 11f);
            playerAS[1].Play();
        }

        // Chequeando colisión con el suelo
        if (col.gameObject.CompareTag("Map"))
        {
            isOnGround = true;
        }
    }
}
