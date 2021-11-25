using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    // Start is called before the first frame 
    private InputActions playerInputActions;

    private Rigidbody2D rb2d;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 150f;
    [SerializeField] private int NbMaxJump = 2;

    private int countJump = 0;
    private bool isJumping = false;


    float floatMoveAction = 0;
    float floatJumpAction = 0;

    int TotalDamages = 0;

    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        playerInputActions = new InputActions();

        playerInputActions.PlayerInput.Jump.started += (ctx) => Jumping();
        playerInputActions.PlayerInput.Jump.canceled += (ctx) => UnlockJumping();
        playerInputActions.PlayerInput.Jump.Enable();

        playerInputActions.PlayerInput.Movement.performed += Walking;
        playerInputActions.PlayerInput.Movement.canceled += Walking;
        playerInputActions.PlayerInput.Movement.Enable();

        playerInputActions.PlayerInput.Ability1.started += (ctx) => UseFirstAbility();
        playerInputActions.PlayerInput.Ability1.Enable();
        playerInputActions.PlayerInput.Ability2.started += (ctx) => UseSecondAbility();
        playerInputActions.PlayerInput.Ability2.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (floatMoveAction < 0)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (floatMoveAction > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (floatMoveAction > 0.1f || floatMoveAction < -0.1f)
        {
            transform.Translate(new Vector2(Math.Abs(floatMoveAction) * moveSpeed * Time.deltaTime, 0f));
        }
    }

    private void FixedUpdate()
    {
        if (floatJumpAction > 0.1f)
        {
            isJumping = true;
            rb2d.AddForce(new Vector2(0f, floatJumpAction * jumpForce), ForceMode2D.Impulse);
            floatJumpAction = 0f;
        }
    }

    void TakeDamages()
    {
        TotalDamages++;
    }

    void Walking(InputAction.CallbackContext ctx)
    {
        floatMoveAction = ctx.ReadValue<float>();
    }

    void Jumping()
    {
        if (!isJumping)
        {
            countJump++;
            floatJumpAction = 0.2f;
        }
    }

    void UnlockJumping()
    {
        if (isJumping && countJump < NbMaxJump)
        {
            isJumping = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            countJump = 0;
            isJumping = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag == "Ennemy")
        {
            TakeDamages();
        }
    }

    private void UseFirstAbility()
    {
        Debug.Log("Using First Ability");
    }

    private void UseSecondAbility()
    {
        Debug.Log("Using Second Ability");
    }
}
