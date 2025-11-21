using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float jumpForce = 21f;
    [SerializeField] public float movementSpeed = 3f;

    public Rigidbody2D playerRB;
    public bool isOnGround;
    public AudioSource[] playerAS;

    // Este axis viene desde UI (si no hay input móvil, queda en 0)
    [HideInInspector] public float uiAxis = 0f;
    [HideInInspector] public bool uiJumpPressed = false;

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

    private void Move()
    {
        // PC
        float horizontal = Input.GetAxisRaw("Horizontal");

        // Si móvil manda input, tiene prioridad
        if (uiAxis != 0)
            horizontal = uiAxis;

        if (horizontal > 0)
            playerRB.transform.Translate(Vector2.right * movementSpeed * Time.deltaTime);

        else if (horizontal < 0)
            playerRB.transform.Translate(Vector2.left * movementSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        // PC
        bool wantJump = Input.GetButtonDown("Jump");

        // Mobile
        if (uiJumpPressed)
        {
            wantJump = true;
            uiJumpPressed = false; // se consume una vez
        }

        if (wantJump && isOnGround)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpForce);
            playerAS[0].Play();
            isOnGround = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Spike"))
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, 11f);
            playerAS[1].Play();
        }

        if (col.gameObject.CompareTag("Map"))
        {
            isOnGround = true;
        }
    }
}

