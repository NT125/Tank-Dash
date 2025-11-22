using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsManager : MonoBehaviour
{
    public Rigidbody2D playerRB;
    public Animator playerAnimator;

    // Game Loop
    void Start()
    {
        // Obteniendo la referencia a los componentes
        playerRB = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        checkJump();
        checkFall();
    }

    // Función que verifica si el jugador salta según la velocidad en Y de su RigidBody2D
    private void checkJump()
    {
        if (playerRB.linearVelocity.y > 0f)
        {
            playerAnimator.SetBool("isJumping", true);
        }
        else
        {
            playerAnimator.SetBool("isJumping", false);
        }
    }

    // Función que verifica si el jugador cae según la velocidad en Y de su RigidBody2D
    private void checkFall()
    {
        if (playerRB.linearVelocity.y < 0f)
        {
            playerAnimator.SetBool("isFalling", true);
        }
        else
        {
            playerAnimator.SetBool("isFalling", false);
        }
    }
}
