using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public struct TAGS
{
    public static string PLATFORM = "Platform";
    public static string ENEMY = "Enemy";
    public static string SOUL = "Soul";
    public static string PLAYER = "Player";
}

public class PlayerBehaviour : MonoBehaviour
{
    //Input manager
    private InputActions playerInputActions;

    //Component of the player
    private Rigidbody2D rb2d;
    private Transform GroundCheckRight;
    private Transform GroundCheckLeft;
    private Collider2D hitbox;
    
    //Physic of the player
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 150f;
    [SerializeField] private int NbMaxJump = 2;

    //platform to spawn
    [SerializeField] private GameObject Platform;
    [SerializeField] private Vector2 OffsetSpawnPlatform;


    //Handling duration and cool down of abilities
    [SerializeField] private float DurationFA;
    [SerializeField] private float CooldownFA;
    [SerializeField] private float CooldownSA;
    private bool OnCooldownFA = false;
    private bool OnCooldownSA = false;

    private int countJump = 0;
    private bool isJumping = false;

    float floatMoveAction = 0;
    float floatJumpAction = 0;

    

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

        GroundCheckRight = transform.Find("GroundCheckRight");
        GroundCheckLeft = transform.Find("GroundCheckLeft");

        hitbox = gameObject.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (floatMoveAction < 0)
        {
            Vector3 current_pos = transform.position;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            transform.position = current_pos;
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

        checkIfGrounded();
    }

    void checkIfGrounded()
    {
        var collider = Physics2D.OverlapArea(GroundCheckLeft.position, GroundCheckRight.position);
        if (collider)
            Grounded(collider);
    }

    void TakeDamages()
    {
        Debug.Log("DEATH");
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
        if (isJumping && countJump < NbMaxJump-1)
        {
            isJumping = false;
        }
    }
    
    private void Grounded(Collider2D collision)
    {
        if (collision.gameObject.tag == TAGS.PLATFORM)
        {
            countJump = 0;
            isJumping = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag == TAGS.ENEMY)
        {
            TakeDamages();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag == TAGS.SOUL)
        {
            obj.GetComponent<CollectibleBehaviour>().DefineTarget(this.gameObject);
        }
    }

    private static void StopAbility1(GameObject o)
    {
        o.layer = 6;
    }

    private async Task delayedWork()
    {
        await Task.Delay((int)DurationFA * 1000);
        gameObject.layer = 6;
    }

    private void UseFirstAbility()
    {
        if (OnCooldownFA)
            return;
        gameObject.layer = 8;
        delayedWork();
        StartCoroutine(CoolingDownFA());
        OnCooldownFA = true;
    }

    private IEnumerator CoolingDownFA()
    {
        yield return new WaitForSeconds(CooldownFA);
        OnCooldownFA = false;
    }

    private void UseSecondAbility()
    {
        if (OnCooldownSA)
            return;
        if (transform.localRotation.eulerAngles.y > 0)
            Instantiate(Platform, transform.position + new Vector3(-OffsetSpawnPlatform.x, OffsetSpawnPlatform.y),
                Quaternion.identity);
        else
            Instantiate(Platform, transform.position + new Vector3(OffsetSpawnPlatform.x, OffsetSpawnPlatform.y),
                Quaternion.identity);
        StartCoroutine(CoolingDownSA());
        OnCooldownSA = true;
    }

    private IEnumerator CoolingDownSA()
    {
        yield return new WaitForSeconds(CooldownSA);
        OnCooldownSA = false;
    }
}
