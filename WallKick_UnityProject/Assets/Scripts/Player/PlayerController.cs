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

    private int displayedEnergy;
    public float currentEnergy;
    public float maximumEnergy = 100f;
    public float energyDecreaseRate;

    private bool isLoosingEnergy;
    private bool isSpendingEnergy;

    public bool isPowered = true;
    public bool energyDecrease = true;
    public bool spendEnergy = true;

    public float groundedMoveEnergyCost = 0.1f;
    public float airMoveEnergyCost = 0.1f;
    public float jumpEnergyCost = 1f;
    public float wallJumpEnergyCost = 0.5f;
    public float punchEnergyCost = 2f;

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
    public float landingLag;
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
    public Transform leftPunchCollider;
    private Rigidbody2D rb;
    private Collider2D _collider2D;
    private RaycastHit2D isHitingPlateform; // used to store collision with plateform when not grounded and holding joystick down
    private int oneWayPlateformMask;  // contains the layermask for OneWayPlateform;

    //-----------------------------------------------------------------------------------------

    [Header("Animations")]
    public Transform playerSkin;
    public Animator characterAnimator;
    public Animator punchCollidersAnimator;
    private bool isFacingRight = true;
    private bool isFacingLeft = false;
    private Quaternion facingRight = Quaternion.Euler(0f, 90f, 0f);
    private Quaternion facingLeft = Quaternion.Euler(0f, 270f, 0f);

    [Header("Visual Effects")]
    public GameObject jumpVFX;

    //-----------------------------------------------------------------------------------------


    void Start()
    {

        guiStyle.normal.textColor = Color.white;

        rb = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();

        oneWayPlateformMask = LayerMask.GetMask("OneWayPlateform");

        currentEnergy = maximumEnergy;
    }

    private void Update()
    {
        //Time.timeScale = 0.12f;

        displayedEnergy = (int)currentEnergy;

        if (Device.DPadUp.WasPressed || Input.GetKeyDown(KeyCode.S))
        {
            if (spendEnergy)
                spendEnergy = false;
            else if (!spendEnergy)
                spendEnergy = true;
        }
        if (Device.DPadDown.WasPressed || Input.GetKeyDown(KeyCode.D))
        {
            if (energyDecrease)
                energyDecrease = false;
            else if (!energyDecrease)
                energyDecrease = true;
        }

        if (currentEnergy > 0)
        {
            if (energyDecrease)
            {
                currentEnergy -= energyDecreaseRate * Time.deltaTime;
            }
            isPowered = true;

            if (Device.Action4.WasPressed || Input.GetKeyDown(KeyCode.E))
                currentEnergy = maximumEnergy;
        }
        else
        {
            rb.velocity += Vector2.zero;
            currentEnergy = 0f;
            isPowered = false;
            characterAnimator.SetBool("poweredOff", true);
            if (Device.Action4.WasPressed || Input.GetKeyDown(KeyCode.E))
            {
                currentEnergy = maximumEnergy;
                isPowered = true;
                characterAnimator.SetBool("poweredOff", false);
                characterAnimator.SetBool("poweredOn", true);
            }
        }

        verticalVelocity = (int)rb.velocity.y;
        horizontalVelocity = (int)rb.velocity.x;

        if (rb.velocity.y <= -maxVerticalVelocity)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxVerticalVelocity);
        }

        xPos = transform.position.x;
        yPos = transform.position.y;
        zPos = transform.position.z;

        var inputDevice = InputManager.ActiveDevice;
        float leftStickValueX = Device.LeftStickX;

        if (isPowered)
        {
            //----------------- LEFT ----------------------
            if (leftStickValueX < -0.2f)
            {
                analogGroundedMaxVelocity = -groundedMaxVelocity * leftStickValueX;
                analogAirMaxVelocity = -airMaxVelocity * leftStickValueX;
                inputLeft = true;

                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(groundedMoveEnergyCost));
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                analogGroundedMaxVelocity = 15f;
                analogAirMaxVelocity = 12f;
                inputLeft = true;

                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(groundedMoveEnergyCost));
            }
            else
                inputLeft = false;

            //----------------- RIGHT ----------------------
            if (leftStickValueX > 0.2f)
            {
                analogGroundedMaxVelocity = groundedMaxVelocity * leftStickValueX;
                analogAirMaxVelocity = airMaxVelocity * leftStickValueX;
                inputRight = true;

                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(groundedMoveEnergyCost));
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                analogGroundedMaxVelocity = 15f;
                analogAirMaxVelocity = 12f;
                inputRight = true;

                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(groundedMoveEnergyCost));
            }
            else
                inputRight = false;

            //----------------- JUMP -----------------------
            if (Device.Action1.WasPressed && isGrounded || Input.GetKeyDown(KeyCode.Space) && isGrounded || Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            {
                inputJump = true;

                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(jumpEnergyCost));
            }

            if (Device.Action1.IsPressed && !isGrounded || Input.GetKey(KeyCode.Space) && !isGrounded || Input.GetKey(KeyCode.UpArrow) && !isGrounded)
                inputJumpHolded = true;
            else
                inputJumpHolded = false;

            //----------------- WALL JUMP -----------------------

            if (Device.Action1.WasPressed && isSlidingOnWall || Input.GetKeyDown(KeyCode.Space) && isSlidingOnWall || Input.GetKeyDown(KeyCode.UpArrow) && isSlidingOnWall)
            {
                inputWallJump = true;

                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(wallJumpEnergyCost));
            }

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
            {
                willPassThroughPlateform_Raycast = false;
            }


            //----------------- PUNCH ----------------------
            if (Device.Action3.WasPressed || Input.GetKeyDown(KeyCode.C))
            {
                inputPunch = true;

                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(punchEnergyCost));
            }
            else
                inputPunch = false;
        }

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
            if (leftStickValueX < -0.2f || Input.GetKey(KeyCode.LeftArrow))
            {
                canWallJumpToRight = true;
                isSlidingOnWall = true;
            }
        }
        else if (!LeftDetectionBox.isTriggered && !RightDetectionBox.isTriggered || Input.GetKey(KeyCode.DownArrow) || Device.LeftStickY < -0.5)
        {
            canWallJumpToRight = false;
            isSlidingOnWall = false;
        }
        // check if the RIGHT detection box is triggering a gameobject with the "wall" tag
        if (RightDetectionBox.isTriggered && !LeftDetectionBox.isTriggered)
        {
            if (leftStickValueX > 0.2f || Input.GetKey(KeyCode.RightArrow))
            {
                canWallJumpToLeft = true;
                isSlidingOnWall = true;
            }
        }
        else if (!RightDetectionBox.isTriggered && !LeftDetectionBox.isTriggered || Input.GetKey(KeyCode.DownArrow) || Device.LeftStickY < -0.5)
        {
            canWallJumpToLeft = false;
            isSlidingOnWall = false;
        }


        if (isGrounded && rb.velocity == new Vector2(0f, 0f))
        {
            characterAnimator.SetBool("isIdling", true);
        }
        else
        {
            characterAnimator.SetBool("isIdling", false);
        }

        if (isGrounded && rb.velocity.x < 0f && rb.velocity.x > -7f || isGrounded && rb.velocity.x > 0f && rb.velocity.x < 7f)
        {
            characterAnimator.SetBool("isWalking", true);
        }
        else
        {
            characterAnimator.SetBool("isWalking", false);
        }

        if (isGrounded && rb.velocity.x >= 7f || isGrounded && rb.velocity.x <= -7f)
        {
            characterAnimator.SetBool("isRunning", true);
        }
        else
        {
            characterAnimator.SetBool("isRunning", false);
        }

        if (rb.velocity.y > 0f)
        {
            characterAnimator.SetBool("isJumping", true);
        }
        else
        {
            characterAnimator.SetBool("isJumping", false);
        }

        if (!isGrounded && rb.velocity.y < 7f)
        {
            characterAnimator.SetBool("isFalling", true);
        }
        else
        {
            characterAnimator.SetBool("isFalling", false);
        }

        if (!isGrounded && isSlidingOnWall)
        {
            characterAnimator.SetBool("isSlidingOnWall", true);
        }
        else
        {
            characterAnimator.SetBool("isSlidingOnWall", false);
        }

        if (inputPunch)
        {
            characterAnimator.SetBool("isPunching", true);
        }
        else
        {
            characterAnimator.SetBool("isPunching", false);
            punchCollidersAnimator.SetBool("enableRightColliderAnimation", false);
            punchCollidersAnimator.SetBool("enableLeftColliderAnimation", false);
        }

        if (isFacingRight && characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Punch_001") && characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
            punchCollidersAnimator.SetBool("enableRightColliderAnimation", true);
        if (isFacingLeft && characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Punch_001") && characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
            punchCollidersAnimator.SetBool("enableLeftColliderAnimation", true);


        // facing animation (left & right)
        if (isFacingRight && !isFacingLeft)
        {
            playerSkin.transform.rotation = facingRight;
        }
        if (isFacingLeft && !isFacingRight)
        {
            playerSkin.transform.rotation = facingLeft;
        }
    }


    void FixedUpdate()
    {
        if (isPowered)
        {
            // pass through plateform when grounded
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

            // pass through plateform when not grounded / in the air
            Vector2 rayStart = new Vector2(xPos, yPos - transform.localScale.y / 2f);
            Vector2 rayEnd = new Vector2(xPos, yPos - transform.localScale.y / 1f);

            if (willPassThroughPlateform_Raycast && !isGrounded)
            {
                isHitingPlateform = Physics2D.Raycast(rayStart, Vector2.down, rayStart.y - rayEnd.y, oneWayPlateformMask);
                //Debug.Log(rayStart.y - rayEnd.y);

                if (isHitingPlateform == true)
                {
                    Physics2D.IgnoreCollision(_collider2D, isHitingPlateform.transform.gameObject.GetComponent<Collider2D>(), true);
                    //Debug.DrawLine(rayStart, rayEnd, Color.green, 0.5f);
                }
                else
                {
                    //Debug.DrawLine(rayStart, rayEnd, Color.red, 0.5f);
                    willPassThroughPlateform_Raycast = false;
                }
            }


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
            if (inputJump && rb.velocity.y == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(transform.up * jumpVelocity, ForceMode2D.Impulse);

                Instantiate(jumpVFX, new Vector3(xPos, yPos - this.transform.localScale.y / 2, transform.position.z), Quaternion.identity);
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
        else
            rb.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Plateform")
        {
            StartCoroutine(LandingLag(landingLag));
        }
    }

    IEnumerator LandingLag(float time)
    {
        float i = 0;
        while (i <= time)
        {
            i += Time.deltaTime;
            if (i <= time/* && isGrounded*/)
            {
                characterAnimator.SetBool("isLanding", true);
            }
            else
                characterAnimator.SetBool("isLanding", false);
            yield return null;
            //yield return new WaitForSeconds(time / 5);
        }
    }

    IEnumerator EnergyConsumption(float actionCost)
    {
        float i = 0;
        while (i <= 1)
        {
            i += 2;
            currentEnergy -= actionCost;
            if (actionCost < 0.2f)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return null;
            }
        }
    }

    void OnGUI()
    {
        guiStyle.fontSize = 12;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        float x = screenPos.x;
        float y = Screen.height - screenPos.y;

        GUI.Label(new Rect(x - 50f, y - 100f, 20f, 50f),
            "spend energy = " + spendEnergy.ToString()
            + "\n" + "energy decrease = " + energyDecrease.ToString()
            + "\n" + "energy = " + displayedEnergy.ToString()
            //"no collision = " + noCollisionState.ToString()
            //+ "\n" + "vertical velocity = " + verticalVelocity.ToString()
            //+ "\n" + "horizontal velocity = " + horizontalVelocity.ToString()
            , guiStyle);
    }
}
