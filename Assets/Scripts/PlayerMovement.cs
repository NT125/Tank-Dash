using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float jumpForce = 21f;
    [SerializeField] private float movementSpeed = 3f;

    private Rigidbody2D playerRB;
    private bool isOnGround;
    public AudioSource[] playerAS;

    private PlayerControls controls;
    private Vector2 moveInput;
    private bool jumpPressed;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerAS = GetComponents<AudioSource>();
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Gameplay.Jump.performed += ctx => jumpPressed = true;
    }

    private void OnDisable()
    {
        controls.Gameplay.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled -= ctx => moveInput = Vector2.zero;
        controls.Gameplay.Jump.performed -= ctx => jumpPressed = true;
        controls.Gameplay.Disable();
    }

    private void Update()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        float horizontal = moveInput.x * movementSpeed * Time.deltaTime;
        transform.Translate(new Vector2(horizontal, 0));
    }

    private void Jump()
    {
        if (jumpPressed && isOnGround)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpForce);
            if (playerAS.Length > 0) playerAS[0].Play();
            isOnGround = false;
        }
        jumpPressed = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Map")) isOnGround = true;

        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Spike"))
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, 11f);
            if (playerAS.Length > 1) playerAS[1].Play();
        }
    }

    // M�todos para los botones de UI
    public void OnJumpButton() => jumpPressed = true;
    public void OnMoveLeftButton(bool pressed) => moveInput.x = pressed ? -1f : 0f;
    public void OnMoveRightButton(bool pressed) => moveInput.x = pressed ? 1f : 0f;
}
