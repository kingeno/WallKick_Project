using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;

// TODO 
// 1. adding a trigger collider to the player. The collider must have the same size (or a little but taller on the vertical axis) as the collider that already exists
// 2. if the player is pressing down and the trigger is currently colliding(check if OnTriggerEnter works for this) with a plateform,
// store the plateform index and disable the collisions between the player collider and the plateform
// 3. use OnTriggerExit to set the collision back to true and empty the variable which stored the plateform index (for the variable to be up to store the next plateform index)

public class PlayerController : MonoBehaviour
{

    private GUIStyle guiStyle = new GUIStyle();

    public InputDevice Device { get; set; }

    private Text debugText;

    private float xPos;
    private float yPos;
    private float zPos;

    private float verticalVelocity;
    private float horizontalVelocity;

    public bool inputPunch = false;
    public bool inputJump = false;
    public bool inputJumpHolded = false;
    public bool inputWallJump = false;
    public bool isFastFalling = false;
    public bool willPassThroughPlateform = false;
    public bool inputLeft = false;
    public bool inputRight = false;
    public static bool inputPause = false;

    public bool willPassThroughPlateform_Raycast = false;

    [Header("Jump")]
    [Range(10, 30)]
    public float jumpVelocity; //recommended value : 20
    [Range(0, 100)]
    public float fallMultiplier; // recommended value : 6
    [Range(0, 100)]
    public float lowJumpMultiplier; // recommended value : 3
    [Header("Wall Jump")]
    public float horizontalForce = 10;
    public float verticalForce = 20;
    public float maxVerticalVelocity; // todo : enter a recommended value

    [Header("Grounded Movement")]
    public float groundedMaxVelocity;
    private float analogGroundedMaxVelocity;
    [Range(0.2f, 5f)]
    public float groundedAcceleration;

    [Header("Air Movement")]
    public float airMaxVelocity;
    private float analogAirMaxVelocity;
    private float fallingVelocity;
    public float fallingMaxVelocity = 20f;
    [Range(0.2f, 20f)]
    public float airAcceleration;
    [Range(0.05f, 0.1f)]
    public float disabledCollisionDuration;
    private float noCollisionState;  // use for debug display

    [Header("Ground & Wall triggers")]
    public bool isGrounded = false;
    public bool isSlidingOnWall = false;
    public bool canWallJumpToRight = false;
    public bool canWallJumpToLeft = false;

    public PlayerGroundCheck PlayerGroundCheck;
    public LeftDetectionBox LeftDetectionBox;
    public RightDetectionBox RightDetectionBox;
    Rigidbody2D rb;
    Collider2D _collider2D;

    //-----------------------------------------------------------------------------------------

    [Header("Animations")]
    public Transform playerSkin;
    public Animator animator;
    private bool isFacingRight = true;
    private bool isFacingLeft = false;
    private Quaternion facingRight = Quaternion.Euler(0f, 0f, 0f);
    private Quaternion facingLeft = Quaternion.Euler(0f, 180f, 0f);

    [Header("Visual Effects")]
    public GameObject jumpVFX;

    //-----------------------------------------------------------------------------------------

    void Start()
    {
        guiStyle.normal.textColor = Color.black;

        rb = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
    }

    private void Update()
    {
        Time.timeScale = 0.2f;

        verticalVelocity = (int)rb.velocity.y;

        if (rb.velocity.y <= -maxVerticalVelocity)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxVerticalVelocity);
        }

        xPos = transform.position.x;
        yPos = transform.position.y;
        zPos = transform.position.z;

        var inputDevice = InputManager.ActiveDevice;
        float leftStickValueX = Device.LeftStickX;

        //----------------- LEFT ----------------------
        if (leftStickValueX < -0.2f)
        {
            analogGroundedMaxVelocity = -groundedMaxVelocity * leftStickValueX;
            analogAirMaxVelocity = -airMaxVelocity * leftStickValueX;
            inputLeft = true;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            analogGroundedMaxVelocity = 15f;
            analogAirMaxVelocity = 12f;
            inputLeft = true;
        }
        else
            inputLeft = false;

        //----------------- RIGHT ----------------------
        if (leftStickValueX > 0.2f)
        {
            analogGroundedMaxVelocity = groundedMaxVelocity * leftStickValueX;
            analogAirMaxVelocity = airMaxVelocity * leftStickValueX;
            inputRight = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            analogGroundedMaxVelocity = 15f;
            analogAirMaxVelocity = 12f;
            inputRight = true;
        }
        else
            inputRight = false;

        //----------------- JUMP -----------------------
        if (Device.Action1.WasPressed && isGrounded || Input.GetKeyDown(KeyCode.Space) && isGrounded || Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            inputJump = true;
            /*jumpVFX = (GameObject) */
            Instantiate(jumpVFX, new Vector3(xPos, yPos - this.transform.localScale.y / 2, transform.position.z), Quaternion.identity);
            //Destroy(jumpVFX, 1f);
        }

        if (Device.Action1.IsPressed && !isGrounded || Input.GetKey(KeyCode.Space) && !isGrounded || Input.GetKey(KeyCode.UpArrow) && !isGrounded)
            inputJumpHolded = true;
        else
            inputJumpHolded = false;

        //----------------- WALL JUMP -----------------------
        if (Device.Action1.WasPressed && isSlidingOnWall || Input.GetKeyDown(KeyCode.Space) && isSlidingOnWall || Input.GetKeyDown(KeyCode.UpArrow) && isSlidingOnWall)
            inputWallJump = true;

        //----------- PASS THROUGH PLATEFORM ------------

        if (Device.LeftStickY < -0.5 || Input.GetKey(KeyCode.DownArrow))
        {
            willPassThroughPlateform = true;
            willPassThroughPlateform_Raycast = true;
        }
        else if (PlayerGroundCheck.plateformName == "empty")
        {
            willPassThroughPlateform = false;
        }
        else
            willPassThroughPlateform_Raycast = false;

        // --------------- FAST FALL --------------------
        // if (Device.LeftStickY < -0.8)
        //    Debug.Log("FastFall");

        //----------------- PUNCH ----------------------
        if (Device.Action3.WasPressed || Input.GetKeyDown(KeyCode.C))
        {
            inputPunch = true;

        }
        else
            inputPunch = false;

        //----------------- PAUSE ----------------------
        if (Device.CommandWasPressed && !inputPause || Input.GetKeyDown(KeyCode.Escape))
            inputPause = true;
        else
            inputPause = false;


        // check if grounded
        if (PlayerGroundCheck.isGrounded && rb.velocity.y <= 0f)
        {
            isGrounded = true;
            isSlidingOnWall = false;
        }
        else
            isGrounded = false;

        // check if the LEFT detection box is triggering a gameobject with the "wall" tag
        if (LeftDetectionBox.isTriggered && !RightDetectionBox.isTriggered)
        {
            canWallJumpToRight = true;
            isSlidingOnWall = true;
        }
        else if (!LeftDetectionBox.isTriggered && !RightDetectionBox.isTriggered)
        {
            canWallJumpToRight = false;
            isSlidingOnWall = false;
        }
        // check if the RIGHT detection box is triggering a gameobject with the "wall" tag
        if (RightDetectionBox.isTriggered && !LeftDetectionBox.isTriggered)
        {
            canWallJumpToLeft = true;
            isSlidingOnWall = true;
        }
        else if (!RightDetectionBox.isTriggered && !LeftDetectionBox.isTriggered)
        {
            canWallJumpToLeft = false;
            isSlidingOnWall = false;
        }



        if (inputPunch)
        {
            animator.SetBool("isPunching", true);
        }
        else
        {
            animator.SetBool("isPunching", false);
        }

        if (rb.velocity.x != 0 && isGrounded)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (!isGrounded)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

        // facing animation (left & right)
        if (isFacingRight && !isFacingLeft)
        {
            playerSkin.transform.rotation = facingRight;
        }
        if (isFacingLeft && !isFacingRight)
        {
            playerSkin.transform.rotation = facingLeft;
        }

        // pass through plateform
        if (willPassThroughPlateform)
        {
            foreach (Collider2D _collider in PlayerGroundCheck.plateformColliders)
            {
                if (_collider.name == PlayerGroundCheck.plateformName)
                    Physics2D.IgnoreCollision(_collider2D, _collider, true);
            }
        }
        else if (!willPassThroughPlateform)
        {
            foreach (Collider2D _collider in PlayerGroundCheck.plateformColliders)
            {
                if (_collider.name == PlayerGroundCheck.plateformName)
                {
                    Physics2D.IgnoreCollision(_collider2D, _collider, false);
                    //Debug.Log("Collision enabled between " + _collider2D.name + " & " + _collider.name);
                }
            }
        }

        // Does the ray intersect any objects excluding the player layer
        if (willPassThroughPlateform_Raycast)
        {
            if (Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.forward), 5f))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right), Color.yellow);
                Debug.Log("Did Hit");
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right), Color.white);
                Debug.Log("Did not Hit");
            }
        }
    }

    void FixedUpdate()
    {
        // horizontal isGrounded movement
        if (isGrounded && inputRight && rb.velocity.x < analogGroundedMaxVelocity)
        {
            isFacingLeft = false;
            isFacingRight = true;
            float _maxContribution = Mathf.Max(0, analogGroundedMaxVelocity - rb.velocity.x);
            float _acceleration = Mathf.Min(_maxContribution, groundedAcceleration);
            rb.velocity += new Vector2(_acceleration, 0f);
        }
        else if (isGrounded && inputLeft && rb.velocity.x > -analogGroundedMaxVelocity)
        {
            isFacingRight = false;
            isFacingLeft = true;
            float _maxContribution = Mathf.Max(0, analogGroundedMaxVelocity + rb.velocity.x);
            float _acceleration = Mathf.Min(_maxContribution, groundedAcceleration);
            rb.velocity += new Vector2(-_acceleration, 0f);
        }
        if (isGrounded && !inputLeft && !inputRight)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        // horizontal airborn movement
        if (!isGrounded && inputRight && rb.velocity.x < analogAirMaxVelocity)
        {
            isFacingLeft = false;
            isFacingRight = true;
            float _maxContribution = Mathf.Max(0, analogAirMaxVelocity - rb.velocity.x);
            float _acceleration = Mathf.Min(_maxContribution, airAcceleration);
            rb.velocity += new Vector2(_acceleration, 0f);
        }
        else if (!isGrounded && inputLeft && rb.velocity.x > -analogAirMaxVelocity)
        {
            isFacingRight = false;
            isFacingLeft = true;
            float _maxContribution = Mathf.Max(0, analogAirMaxVelocity + rb.velocity.x);
            float _acceleration = Mathf.Min(_maxContribution, airAcceleration);
            rb.velocity += new Vector2(-_acceleration, 0f);
        }

        // horizontal deceleration when joystick not pushed
        if (!isGrounded && !inputLeft && !inputRight && rb.velocity.x < 0)
        {
            float _deceleration = 0.3f;
            rb.velocity += new Vector2(_deceleration, 0f);
        }
        else if (!isGrounded && !inputLeft && !inputRight && rb.velocity.x > 0)
        {
            float _deceleration = 0.3f;
            rb.velocity += new Vector2(-_deceleration, 0f);
        }

        // jump
        if (inputJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(transform.up * jumpVelocity, ForceMode2D.Impulse);
            inputJump = false;
        }
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0)
        {
            if (inputJumpHolded)
                rb.gravityScale = lowJumpMultiplier;
            else
                rb.gravityScale = fallMultiplier;
        }
        else if (isGrounded)
        {
            rb.gravityScale = 1f;
        }
        else
        {
            inputJumpHolded = false;
        }

        // wall jump using side boxes (trigger) detection
        if (isSlidingOnWall)
        {
            maxVerticalVelocity = 8f;

            if (!isGrounded && canWallJumpToRight)
            {
                isFacingLeft = false;
                isFacingRight = true;

                if (inputWallJump)
                {
                    rb.velocity = new Vector2(0f, 0f);
                    Vector2 _wallJumpForce = new Vector2(horizontalForce, verticalForce);
                    rb.AddForce(_wallJumpForce, ForceMode2D.Impulse);
                    isSlidingOnWall = false;
                }
            }
            if (!isGrounded && canWallJumpToLeft)
            {
                isFacingRight = false;
                isFacingLeft = true;

                if (inputWallJump)
                {
                    rb.velocity = new Vector2(0f, 0f);
                    Vector2 _wallJumpForce = new Vector2(-horizontalForce, verticalForce);
                    rb.AddForce(_wallJumpForce, ForceMode2D.Impulse);
                    isSlidingOnWall = false;
                }
            }
            inputWallJump = false;
        }
        else
            maxVerticalVelocity = 20f;
    }

    void OnGUI()
    {
        guiStyle.fontSize = 20;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        float x = screenPos.x;
        float y = Screen.height - screenPos.y;

        GUI.Label(new Rect(x - 30f, y - 100f, 20f, 50f),
            "no collision = " + noCollisionState.ToString() + "\n" +
            "vertical velocity = " + verticalVelocity.ToString(),
            guiStyle);
    }
}
