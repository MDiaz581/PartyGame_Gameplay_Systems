using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerBehaviorRewire : MonoBehaviour
{

    private Rigidbody2D RB;

    public int playerNumber = 0;

    public Transform modelTransform;

    [SerializeField]
    private LayerMask hitMask;

    [SerializeField]
    private LayerMask wallJumpMask;

    [SerializeField]
    private Player player;

    [Header("Walking")]

    [Tooltip("This is the speed of which the player will initially have when moving, this allows for the player to speed up left to right much faster, giving snappier movement")]
    public float startSpeed;

    [Tooltip("This is the speed of which the player will continuously have when moving, this is more of how the player will accelerate")]
    public float speed;

    [Tooltip("This is the max speed the player can move at")]
    public float maxSpeed;

    [Tooltip("This is the rate of which the player will decelerate when not moving, this changes the amount of momentum continues to have after letting go of the move button")]
    public float decelerationRate;

    [Header("Running")]
    [Tooltip("This is the multiplier for running, a higher number means more max speed and more acceleration rate")]
    public float runMultiplier;


    private float runSpeed;
    private bool runRight;
    private bool runLeft;

    [Header("Jumping")]

    [Tooltip("This is the speed of which the player can jump, affects the height of each jump")]
    public float jumpSpeed;

    [Tooltip("This is the rate of which the player can change direction during a jump, the higher the value the less control")]
    public float airControl;

    [Tooltip("The amount of speed before preventing you from jumping any higher, the higher the value the higher the jump. Allows for short hopping")]
    public float jumpLimitValue;

    public float fastFallSpeed;

    public float wallJumpSpeed;

    public float wallJumpHorizontalSpeed;

    public bool jumpReset;

    public bool jumping;

    public bool jumpLimit;

    private bool canWallJumpRight;

    private bool canWallJumpLeft;

    public bool crouching;

    public bool isDead;

    //Used to make wall jumps more responsive
    private bool onWall;

    public bool grounded;

    private Vector3 offset;

    private Vector3 wallJumpOffset;

    //This is the amount of time you have in the dodge state
    public float dodgeTime;

    private float dodgeTimer;

    public float dodgeCoolDown;

    public float dodgeCoolDownTimer;

    private bool dodgeEnded;

    public bool dodging;

    public GameObject dodgeSphere;

    public Animator animator;

    private AudioSource AS;

    public AudioClip sfx_jump;

    private float phaseTime = 0f;

    private float phaseTimer = 0.275f;

    private bool phasing;

    private bool aboveFallthrough;

    private GameManager gameManager;

    private CamFindPlayers CFP;

    private int deathFailsafe = 1;

    private PlayerFeedbackSystem PFS;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        CFP = GameObject.Find("TargetGroup1").GetComponent<CamFindPlayers>();

        gameManager.numPlayers += 1;

        RB = GetComponent<Rigidbody2D>();

        offset = new Vector3(0, -0.53f);
        wallJumpOffset = new Vector3(0.05f, 0);

        player = ReInput.players.GetPlayer(playerNumber);

        animator = modelTransform.GetComponent<Animator>();

        jumpReset = true;

        AS = GameObject.Find("Audio Source").GetComponent<AudioSource>();

        PFS = GetComponent<PlayerFeedbackSystem>();

    }

    // Start is called before the first frame update
    void Start()
    {

    }


    private void Update()
    {
        if (!isDead)
        {
            AnimatorControllerFunction();
            StartWalking();
            WallJumpCheck();
            FloorChecker();
            PhaseThrough();
            WallJump();
            Dead();
            NavigateMenu();
        }

        

        if (player.GetButtonDown("Pause"))
        {
            Pause();
        }
        if (player.GetButtonDown("Unpause"))
        {
            Unpause();
        }
        

        if (phasing)
        {
            phaseTime += Time.deltaTime;

            if(phaseTime >= phaseTimer && this.gameObject.layer != LayerMask.NameToLayer("Dead"))
            {
                this.gameObject.layer = 8;
                phasing = false;
            }
        }

        if (player.GetButtonUp("Jump"))
        {
            jumpReset = true;
        }

        if (AS != null && player.GetButtonDown("Jump") && !crouching && !jumpLimit && !isDead)
        {
            AS.pitch = Random.Range(0.8f, 1.1f);
            AS.PlayOneShot(sfx_jump);
        }

        if (player.GetButtonDown("Dodge") && !dodging && dodgeCoolDownTimer <= 0f && !isDead)
        {
            dodgeEnded = false;
            dodgeCoolDownTimer = dodgeCoolDown;
            dodgeTimer = 0f;
            dodging = true;
        }

        if (dodgeEnded)
        {
            if (dodgeCoolDownTimer > 0f)
            {
                dodgeCoolDownTimer -= Time.deltaTime;
            }

        }

        if (dodging)
        {
            Dodge();
        }

        if(grounded && runLeft)
        {
            modelTransform.transform.rotation = Quaternion.Euler(0, 270, 0);
        }


        if (grounded && runRight)
        {
            modelTransform.transform.rotation = Quaternion.Euler(0, 90, 0);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {
            Walking();
            Stopping();
            Jumping();
            Crouch();
        }


    }

    void StartWalking()
    {

        //Left movement
        //This is the initial force to give snappier movement
        if (player.GetButtonDown("LeftMove"))
        {
            modelTransform.transform.rotation = Quaternion.Euler(0, 270, 0);
            if (!jumping)
            {
                if (RB.velocity.x > -maxSpeed)
                {
                    
                    RB.velocity = Vector2.zero;

                    RB.AddForce(-Vector2.right * startSpeed, ForceMode2D.Impulse);
                }
            }
        }

        //Right Movement
        //This is the initial force to give snappier movement
        if (player.GetButtonDown("RightMove"))
        {
            modelTransform.transform.rotation = Quaternion.Euler(0, 90, 0);
            if (!jumping)
            {

                if (RB.velocity.x < maxSpeed)
                {
                    RB.velocity = Vector2.zero;

                    RB.AddForce(Vector2.right * startSpeed, ForceMode2D.Impulse);
                }
            }
        }

    }

    void Walking()
    {

        animator.SetFloat("Speed", Mathf.Abs(player.GetAxis("Horizontal")));


        if(Mathf.Abs(player.GetAxis("Horizontal")) <= 0.15f)
        {
            if (Mathf.Abs(RB.velocity.x) > 1.25f)
            {

                animator.SetBool("StopBool", true);

            }
            else
            {
                animator.SetBool("StopBool", false);

            }
        }


        //Left movement

        //This is the constant force
        if (player.GetAxis("Horizontal") < 0)
        {
            if (RB.velocity.x > -maxSpeed * Mathf.Abs(player.GetAxis("Horizontal")))
            {
                if (jumping)
                {
                    RB.AddForce((Vector2.right * speed / airControl) * (player.GetAxis("Horizontal") * 2.5f), ForceMode2D.Force);
                }
                else
                {
                    RB.AddForce((Vector2.right * speed) * (player.GetAxis("Horizontal") * 2.5f), ForceMode2D.Force);
                }

            }
            runLeft = true;
        }
        else
        {
            runLeft = false;
        }

        //Right Movement

        //This is the constant force
        if (player.GetAxis("Horizontal") > 0)
        {
            if (RB.velocity.x < maxSpeed * Mathf.Abs(player.GetAxis("Horizontal")))
            {
                if (jumping)
                {
                    RB.AddForce((Vector2.right * speed  / airControl) * (player.GetAxis("Horizontal") * 2.5f), ForceMode2D.Force);
                }
                else
                {
                    RB.AddForce((Vector2.right * speed) * (player.GetAxis("Horizontal") * 2.5f), ForceMode2D.Force);
                }

            }
            runRight = true;
        }
        else
        {
            runRight = false;
        }
    }

    void Running()
    {
        if (player.GetButton("Run"))
        {
            runSpeed = runMultiplier;
        }
        else
        {
            runSpeed = 1f;
        }

    }

    void Stopping()
    {
        if (!jumping)
        {
            if (!runRight)
            {
                if (RB.velocity.x > 1)
                {

                    RB.AddForce(-Vector2.right * (speed / decelerationRate));
                }
            }
            if (!runLeft)
            {
                if (RB.velocity.x < -1)
                {
                    RB.AddForce(Vector2.right * (speed / decelerationRate));
                }
            }
        }
    }

    void Jumping()
    {


        if (player.GetButton("Jump") && !jumpLimit && jumpReset && !crouching)
        {

            if (!canWallJumpLeft || !canWallJumpRight)
            {
                RB.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            }


            if (RB.velocity.y > jumpLimitValue)
            {
                jumpReset = false;
                jumpLimit = true;
            }
            grounded = false;
            jumping = true;
        }

        if (player.GetButtonUp("Jump"))
        {
            jumpReset = true;
            if (jumping)
            {
                jumpLimit = true;
            }

        }

    }

    void PhaseThrough()
    {
        if (player.GetButtonDoublePressDown("Down", 0.5f) && aboveFallthrough)
        {
            Debug.Log("doown doubles");
            this.gameObject.layer = 15;
            phaseTime = 0f;
            phasing = true;
            
        }
        /*
        if (player.GetButtonDoublePressUp("Down", 1f))
        {
            Debug.Log("doown doubles");
            this.gameObject.layer = 8;
        }
        */
        if (player.GetButton("Down") && player.GetButton("Jump") && aboveFallthrough)
        {

            this.gameObject.layer = 15;
            phaseTime = 0f;
            phasing = true;

        }
        /*
        if(player.GetButtonUp("Down") || player.GetButtonUp("Jump"))
        {
            this.gameObject.layer = 8;
        }
        */
    }

    void Crouch()
    {

        if (player.GetAxis("Crouch") < -0.8f)
        {
            crouching = true;
        }
        else
        {
            crouching = false;
        }


        if (jumping)
        {
            if (player.GetAxis("Crouch") < -0.8f && RB.velocity.y < maxSpeed / 2)
            {
                RB.AddForce(-Vector2.up * fastFallSpeed, ForceMode2D.Impulse);           
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        RaycastHit2D groundChecker = Physics2D.CircleCast(transform.position + offset, 0.1f, -Vector2.up, 100f, hitMask);

        if (groundChecker.collider != null)
        {
            if (Mathf.Abs(groundChecker.point.y - transform.position.y) < 0.6f)
            {
                jumpLimit = false;
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RaycastHit2D groundChecker = Physics2D.CircleCast(transform.position + offset, 0.1f, -Vector2.up, 100f, hitMask);

        if (groundChecker.collider != null)
        {
            if (Mathf.Abs(groundChecker.point.y - transform.position.y) < 0.6f)
            {
                if (jumpLimit)
                {
                    RB.velocity = new Vector2(RB.velocity.x, 0f);
                }
                jumping = false;
                jumpLimit = false;
            }
        }
    }


    private void FloorChecker()
    {
        RaycastHit2D floorChecker = Physics2D.CircleCast(transform.position + offset, 0.1f, -Vector2.up, 100f, hitMask);

        if (floorChecker.collider != null)
        {

            if (Mathf.Abs(floorChecker.point.y - transform.position.y) > 0.6f)
            {
                if (!jumping)
                {
                    jumping = true;
                    jumpLimit = true;
                    grounded = false;

                    aboveFallthrough = false;
                }

            }
            else
            {               
                grounded = true;
                canWallJumpLeft = false;
                canWallJumpRight = false;
                if(floorChecker.collider.gameObject.tag == "FallThrough")
                {
                    aboveFallthrough = true;
                } else
                {
                    aboveFallthrough = false;
                }
            }
        }

    }

    private void WallJumpCheck()
    {
        if (!grounded)
        {
            RaycastHit2D wallCheckerRight = Physics2D.CircleCast(transform.position + wallJumpOffset, 0.25f, Vector2.right, 100f, wallJumpMask);

            if (wallCheckerRight.collider != null)
            {
                if (Mathf.Abs(wallCheckerRight.point.x - transform.position.x) < 0.35f)
                {
                    
                    modelTransform.transform.rotation = Quaternion.Euler(0, 270, 0);
                    canWallJumpRight = true;

                    //This makes wall jumping more responsive by slowing you down as you hit the wall
                    if (!player.GetButton("Jump"))
                    {
                        onWall = true;
                    }
                    else
                    {
                        onWall = false;
                    }
                    if (onWall)
                    {
                        RB.velocity = new Vector2(RB.velocity.x / 2f, RB.velocity.y);
                    }

                }
                else
                {
                    canWallJumpRight = false;
                }

            }

            RaycastHit2D wallCheckerLeft = Physics2D.CircleCast(transform.position - wallJumpOffset, 0.25f, -Vector2.right, 100f,wallJumpMask);

            if (wallCheckerLeft.collider != null)
            {
                if (Mathf.Abs(wallCheckerLeft.point.x - transform.position.x) < 0.35f)
                {
                    

                    modelTransform.transform.rotation = Quaternion.Euler(0, 90, 0);
                    canWallJumpLeft = true;

                    if (!player.GetButton("Jump"))
                    {
                        onWall = true;
                    } else
                    {
                        onWall = false;
                    }
                    

                    if (onWall)
                    {
                       RB.velocity = new Vector2(RB.velocity.x/2f, RB.velocity.y);
                    }

                }
                else
                {
                    canWallJumpLeft = false;
                }
            }
        }
    }

    private void WallJump()
    {
        if (canWallJumpRight && player.GetButtonDown("Jump") && !grounded)
        {
            if (AS != null)
            {
                AS.pitch = Random.Range(0.8f, 1.1f);
                AS.PlayOneShot(sfx_jump);
            }

            RB.velocity = Vector2.zero;
            RB.AddForce(Vector2.up * wallJumpSpeed, ForceMode2D.Impulse);
            RB.AddForce(-Vector2.right * wallJumpHorizontalSpeed, ForceMode2D.Impulse);
            animator.SetTrigger("WallJumped");
            jumpReset = false;
            Debug.Log("WallJumping");
            canWallJumpRight = false;



        }

        if (canWallJumpLeft && player.GetButtonDown("Jump") && !grounded)
        {
            if (AS != null)
            {
                AS.pitch = Random.Range(0.8f, 1.1f);
                AS.PlayOneShot(sfx_jump);
            }

            RB.velocity = Vector2.zero;
            RB.AddForce(Vector2.up * wallJumpSpeed, ForceMode2D.Impulse);
            RB.AddForce(Vector2.right * wallJumpHorizontalSpeed, ForceMode2D.Impulse);
            jumpReset = false;
            animator.SetTrigger("WallJumped");

            Debug.Log("WallJumping");
            canWallJumpLeft = false;

        }
    }

    void Dodge()
    {
        dodgeTimer += Time.deltaTime;

        transform.gameObject.layer = LayerMask.NameToLayer("Dodge");

        //dodgeSphere.SetActive(true);
        animator.SetBool("Dodging", true);

        if (dodgeTimer >= dodgeTime)
        {
            animator.SetBool("Dodging", false);           
            //dodgeSphere.SetActive(false);
            dodgeEnded = true;
            transform.gameObject.layer = LayerMask.NameToLayer("Player");
            dodging = false;

        }
    }

    void Dead()
    {
        if(this.gameObject.layer == LayerMask.NameToLayer("Dead"))
        {
            //animator.SetTrigger("Dead");
            //this.enabled = false;
        }
    }

    public void Die()
    {
        PFS.ScreenShake();

        this.gameObject.layer = LayerMask.NameToLayer("Dead");
        animator.SetTrigger("Dead");
        if(deathFailsafe == 1)
        {
            gameManager.numPlayers -= 1;
            deathFailsafe -= 1;
        }
        
        isDead = true;
        CFP.RemovePlayer();
        gameManager.WinCheck();
        Debug.Log(gameManager.numPlayers);
    }


    public void Pause()
    {
        gameManager.Pause();

        gameManager.pauseScreen.GetComponent<PauseMenu>().OnPause(playerNumber);
    }

    public void Unpause()
    {

        gameManager.Unpause();
            
    }

    public void NavigateMenu()
    {
        if (player.GetNegativeButtonDown("Vertical Move"))
        {

            gameManager.pauseScreen.GetComponent<PauseMenu>().ChangeState(-1);
        }

        if (player.GetButtonDown("Vertical Move"))
        {

            gameManager.pauseScreen.GetComponent<PauseMenu>().ChangeState(1);
        }

    }

    public void EnableMenuControls()
    {
        player.controllers.maps.SetAllMapsEnabled(false);
        player.controllers.maps.SetMapsEnabled(true, "Menu");
        gameManager.pauseScreen.GetComponent<PauseMenu>().enabled = true;
    }

    public void EnablePlayControls()
    {
        player.controllers.maps.SetAllMapsEnabled(false);
        player.controllers.maps.SetMapsEnabled(true, "Default");
        gameManager.pauseScreen.GetComponent<PauseMenu>().enabled = false;
    }


    void AnimatorControllerFunction()
    {
        if (!isDead)
        {
            if (grounded)
            {
                animator.SetBool("Grounded", true);
            }
            else
            {
                animator.SetBool("Grounded", false);
            }
            if (jumping && !grounded)
            {
                animator.SetBool("Jumping", true);
            }
            else
            {
                animator.SetBool("Jumping", false);
            }
            if (canWallJumpLeft || canWallJumpRight)
            {
                animator.SetBool("CanWallJump", true);
            }
            else
            {
                animator.SetBool("CanWallJump", false);
            }
        } else
        {
            animator.SetBool("Jumping", false);
        }

    }
}
