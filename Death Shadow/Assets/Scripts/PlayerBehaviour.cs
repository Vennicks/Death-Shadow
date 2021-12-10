using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    private int total_souls = 0;
    //Component of the player
    private Rigidbody2D rb2d;
    private Transform GroundCheckRight;
    private Transform GroundCheckLeft;
    private Collider2D hitbox;

    private Animator animator;

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
        animator = gameObject.GetComponent<Animator>();

        playerInputActions.PlayerInput.Jump.started += (ctx) => Jumping();
        playerInputActions.PlayerInput.Jump.canceled += (ctx) => UnlockJumping();
        playerInputActions.PlayerInput.Jump.Enable();

        playerInputActions.PlayerInput.Movement.performed += (ctx) => { floatMoveAction = ctx.ReadValue<float>(); };
        playerInputActions.PlayerInput.Movement.canceled += (ctx) => { floatMoveAction = ctx.ReadValue<float>(); };
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
        if (transform.position.y < -15)
            OutOfRange();

        if (transform.position.x >= 307)
            WinZone();

        if (floatJumpAction > 0.1f)
        {
            isJumping = true;
            rb2d.AddForce(new Vector2(0f, floatJumpAction * jumpForce), ForceMode2D.Impulse);
            floatJumpAction = 0f;
        }

        checkIfGrounded();
        animator.SetFloat("speed", Math.Abs(floatMoveAction));
    }

    private void FixedUpdate()
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
            //transform.Translate(new Vector2(Math.Abs(floatMoveAction) * moveSpeed * Time.deltaTime, 0f));
            rb2d.velocity += new Vector2(floatMoveAction * moveSpeed * Time.deltaTime, 0f);
        }
    }

    void TakeDamages()
    {
        SceneManager.LoadScene("Scenes/Menu/LoseScreen");
    }

    void OutOfRange()
    {
        SceneManager.LoadScene("Scenes/Menu/LoseScreen");
    }

    void WinZone()
    {
        SceneManager.LoadScene("Scenes/Menu/VictoryScreen");
    }

        //Increment the score when a soul has been catched
    public void IncrementScore()
    {
        GameObject canvaObject = GameObject.Find("Canva");
        TextMeshProUGUI canvas = canvaObject.GetComponent<TextMeshProUGUI>();
        total_souls += 1;
        canvas.text = "Total souls collected: " + total_souls;
    }

    //----------- HANDLING JUMP -----------------------------------------------------------
    //Check if the player is on the ground
    void checkIfGrounded()
    {
        var collider = Physics2D.OverlapArea(GroundCheckLeft.position, GroundCheckRight.position);
        if (collider)
        {
            if (collider.gameObject.tag == TAGS.PLATFORM)
            {
                countJump = 0;
                animator.SetInteger("JumpState", countJump);
                animator.SetBool("Grounded", true);
                isJumping = false;
            }
            else
            {
                animator.SetBool("Grounded", false);
            }
        }
    }
        //perform a jump
    void Jumping()
    {
        if (!isJumping)
        {
            countJump++;
            animator.SetInteger("JumpState", countJump);
            floatJumpAction = 0.2f;
        }
    }

        //unlock jumping if it can jump again
    void UnlockJumping()
    {
        if (isJumping && countJump < NbMaxJump-1)
        {
            isJumping = false;
        }
    }
    
    //----------------------- HANDLE COLLISIONS--------------------------------------------

        //Check if collide with enemy
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag == TAGS.ENEMY)
        {
            TakeDamages();
        }
    }

        //Check if collectible entered in the area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag == TAGS.SOUL)
        {
            obj.GetComponent<CollectibleBehaviour>().DefineTarget(this.gameObject);
        }
    }
    
    //---------------- HANDLING ABILITY-----------------

        //put the player out of the shadow
        private async Task delayedWork()
    {
        await Task.Delay((int)DurationFA * 1000);
        animator.SetBool("InShadow", false);
        gameObject.layer = 6;
    }

        //Use the first ability
    private void UseFirstAbility()
    {
        if (OnCooldownFA)
            return;
        gameObject.layer = 8;
        delayedWork();
        animator.SetBool("InShadow", true);
        StartCoroutine(CoolingDownFA());
        OnCooldownFA = true;
    }

        //handle the end of the cooldown of the first ability
    private IEnumerator CoolingDownFA()
    {
        yield return new WaitForSeconds(CooldownFA);
        OnCooldownFA = false;
    }

    //Use the second ability
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

        //handle the end of the cooldown of the Second ability
    private IEnumerator CoolingDownSA()
    {
        yield return new WaitForSeconds(CooldownSA);
        OnCooldownSA = false;
    }
}
