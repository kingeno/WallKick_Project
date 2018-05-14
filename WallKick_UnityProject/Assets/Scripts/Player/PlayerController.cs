using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    public InputDevice Device { get; set; }

    [Header("Grounded Movement")]
    public float groundedMaxVelocity;
    private float analogGroundedMaxVelocity;
    [Range(.2f, 5.0f)] public float groundedAcceleration;

    [Header("Airborn Movement")]
    public float airMaxVelocity;
    private float analogAirMaxVelocity;
    [Range(.2f, 20.0f)] public float airAcceleration;
    private float fallingVelocity;
    public float fallingMaxVelocity = 20.0f;

    [Header("Jump")]
    [Range(10, 30)]
    public float jumpVelocity; //recommended value : 20
    [Range(0, 100)] public float fallMultiplier; // recommended value : 6
    [Range(0, 100)] public float lowJumpMultiplier; // recommended value : 3
    public float landingLag;

    [Header("Wall Jump")]
    public float horizontalForce = 10.0f;
    public float verticalForce = 20.0f;
    public float maxVerticalVelocity; // todo : enter a recommended value

    [Header("Punch")]
    public int hitStrength;
    [HideInInspector] public int bonusStrength;
    public int bonusStrengthMultiplier;
    [HideInInspector] public int totalStrengh;

    [Header("Energy")]
    public float maximumEnergy = 100.0f;
    public float passiveEnergyLoss;
    public float groundedMoveEnergyCost = .1f;
    public float airMoveEnergyCost = .1f;
    public float jumpEnergyCost = 1.0f;
    public float wallJumpEnergyCost = .5f;
    public float punchEnergyCost = 2.0f;
    [HideInInspector] public float currentEnergy;

    [HideInInspector] public bool isLoosingEnergy;
    [HideInInspector] public bool isSpendingEnergy;
    [HideInInspector] public bool canCollideWithSource;
    [HideInInspector] public bool isPowered;
    [HideInInspector] public bool energyDecrease;
    [HideInInspector] public bool spendEnergy;

    [Header("Ground & Wall triggers")]
    public GameObject playerGroundCheck;
    private Collider2D playerGroundCheckCollider;
    public PlayerGroundCheck playerGroundCheckScript;

    public LeftDetectionBox leftDetectionBox;
    public RightDetectionBox rightDetectionBox;

    [Header("Punch Colliders")]
    public Transform leftPunchCollider;

    //-----------------------------------------------------------------------------------------

    [Header("Animations")]
    public Transform playerSkin;
    public Animator characterAnimator;
    public Animator straightPunchColliderAnimator;
    public Animator uppercutColliderAnimator;
    private bool isFacingRight;
    private bool isFacingLeft = false;
    private Quaternion facingRight = Quaternion.Euler(.0f, 90.0f, .0f);
    private Quaternion facingLeft = Quaternion.Euler(.0f, 270.0f, .0f);

    [Header("Visual Effects")]
    public GameObject jumpVFX;

    [Header("UI")]
    public Image energyGauge;

    //-----------------------------------------------------------------------------------------

    private Rigidbody2D playerRigidbody;
    private Collider2D playerCollider;
    private RaycastHit2D isHitingPlateform; // used to store collision with plateform when not grounded and holding joystick down
    private int oneWayPlateformMask;  // contains the layermask for OneWayPlateform;

    private GameObject gameManager;
    private GameManager gameManagerScript;

    //-----------------------------------------------------------------------------------------

    private bool inputLeft = false;
    private bool inputRight = false;
    private bool inputPunch = false;
    //private bool inputOvercut = false;
    private bool inputUppercut = false;
    private bool inputDownAir = false;
    private bool inputJump = false;
    private bool inputJumpHolded = false;
    private bool inputWallJump = false;
    public static bool inputPause = false;

    /*[HideInInspector]*/
    public bool isGrounded = false;
    [HideInInspector] public bool isSlidingOnWall = false;
    [HideInInspector] public bool canWallJumpToRight = false;
    [HideInInspector] public bool canWallJumpToLeft = false;

    [HideInInspector] public bool willPassThroughPlateform = false;
    [HideInInspector] public bool willPassThroughPlateformRaycast = false;

    //-----------------------------------------------------------------------------------------

    private float xPos;
    private float yPos;
    private float zPos;
    private float verticalVelocity;
    private float horizontalVelocity;

    private GUIStyle guiStyle = new GUIStyle();
    private Text debugText;
    private int debugDisplayedEnergy;


    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        playerGroundCheckCollider = playerGroundCheck.GetComponent<Collider2D>();

        gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManager>();

        oneWayPlateformMask = LayerMask.GetMask("OneWayPlateform");

        currentEnergy = maximumEnergy;
        energyGauge.fillAmount = maximumEnergy / maximumEnergy;

        guiStyle.normal.textColor = Color.white;
    }

    private void Update()
    {
        bonusStrength = (int)WallSplitMovement.horizontalVelocity;

        if (bonusStrength < 0)
            bonusStrength *= -1;

        if (bonusStrength > 0)
            totalStrengh = hitStrength + (bonusStrength * bonusStrengthMultiplier);
        else
            totalStrengh = hitStrength;


        if (!isGrounded && Device.LeftStickY < -0.3f)
            isGrounded = false;

        canCollideWithSource = false;

        debugDisplayedEnergy = (int)currentEnergy;
        energyGauge.fillAmount = currentEnergy / 100;

        // energy spend control
        if (Device.DPadUp.WasPressed || Input.GetKeyDown(KeyCode.S))
        {
            if (spendEnergy)
                spendEnergy = false;
            else if (!spendEnergy)
                spendEnergy = true;
        }
        // energy passive consumption
        if (Device.DPadDown.WasPressed || Input.GetKeyDown(KeyCode.D))
        {
            if (energyDecrease)
                energyDecrease = false;
            else if (!energyDecrease)
                energyDecrease = true;
        }

        // energy decrease condition
        if (currentEnergy > 0)
        {
            if (energyDecrease)
            {
                currentEnergy -= passiveEnergyLoss * Time.deltaTime;
            }
            isPowered = true;

            if (Device.Action4.WasPressed || Input.GetKeyDown(KeyCode.E))
                currentEnergy = maximumEnergy;

            if (currentEnergy > maximumEnergy)
                currentEnergy = maximumEnergy;
        }
        else if (currentEnergy <= 0 && isGrounded)
        {
            playerRigidbody.velocity += Vector2.zero;
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

        verticalVelocity = (int)playerRigidbody.velocity.y;
        horizontalVelocity = (int)playerRigidbody.velocity.x;

        if (playerRigidbody.velocity.y <= -maxVerticalVelocity)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -maxVerticalVelocity);
        }

        xPos = transform.position.x;
        yPos = transform.position.y;
        zPos = transform.position.z;

        var inputDevice = InputManager.ActiveDevice;
        float leftStickValueX = Device.LeftStickX;

        if (isPowered)
        {
            //----------------- LEFT ----------------------
            if (leftStickValueX < -0.3f)
            {
                analogGroundedMaxVelocity = -groundedMaxVelocity * leftStickValueX;
                analogAirMaxVelocity = -airMaxVelocity * leftStickValueX;
                inputLeft = true;

                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(groundedMoveEnergyCost));
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                analogGroundedMaxVelocity = groundedMaxVelocity;
                analogAirMaxVelocity = airMaxVelocity;
                inputLeft = true;
                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(groundedMoveEnergyCost));
            }
            else
                inputLeft = false;

            //----------------- RIGHT ----------------------
            if (leftStickValueX > .3f)
            {
                analogGroundedMaxVelocity = groundedMaxVelocity * leftStickValueX;
                analogAirMaxVelocity = airMaxVelocity * leftStickValueX;
                inputRight = true;

                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(groundedMoveEnergyCost));
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                analogGroundedMaxVelocity = groundedMaxVelocity;
                analogAirMaxVelocity = airMaxVelocity;
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
                willPassThroughPlateformRaycast = true;
            }
            else if (gameManagerScript.plateformName == "empty")
            {
                willPassThroughPlateform = false;
            }
            else
            {
                willPassThroughPlateformRaycast = false;
            }


            //----------------- PUNCH ----------------------
            // straight punch
            if (Device.Action3.WasPressed && Device.LeftStickY < 0.5f && Device.LeftStickY > -0.5f || Input.GetKeyDown(KeyCode.C))
            {
                inputPunch = true;
                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(punchEnergyCost));
            }
            else
                inputPunch = false;

            // overcut
            //if (Device.Action3.WasPressed && Device.LeftStickY < -0.5f || Input.GetKeyDown(KeyCode.C) && Input.GetKeyDown(KeyCode.DownArrow))
            //{
            //    inputOvercut = true;
            //    if (spendEnergy)
            //        StartCoroutine(EnergyConsumption(punchEnergyCost));
            //}
            //else
            //    inputOvercut = false;

            // uppercut
            if (Device.Action3.WasPressed && Device.LeftStickY > 0.5f || Input.GetKeyDown(KeyCode.C) && Input.GetKeyDown(KeyCode.UpArrow))
            {
                inputUppercut = true;
                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(punchEnergyCost));
            }
            else
                inputUppercut = false;

            // down air
            if (Device.Action2.WasPressed && verticalVelocity != 0f /*!isGrounded && Device.Action3.WasPressed && Device.LeftStickY < -0.5f*/)
            {
                inputDownAir = true;
                if (spendEnergy)
                    StartCoroutine(EnergyConsumption(punchEnergyCost));
            }
            else
                inputDownAir = false;


            //----------------- PAUSE ----------------------
            if (Device.CommandWasPressed && !inputPause || Input.GetKeyDown(KeyCode.Escape))
                inputPause = true;
            else
                inputPause = false;


            // check if grounded
            if (playerGroundCheckScript.isGrounded && playerRigidbody.velocity.y <= 0f)
            {
                isGrounded = true;
                isSlidingOnWall = false;
            }
            else
                isGrounded = false;

            // check if the LEFT detection box is triggering a gameobject with the "wall" tag
            if (leftDetectionBox.isTriggered && !rightDetectionBox.isTriggered)
            {
                if (leftStickValueX < -0.2f || Input.GetKey(KeyCode.LeftArrow))
                {
                    canWallJumpToRight = true;
                    isSlidingOnWall = true;
                }
            }
            else if (!leftDetectionBox.isTriggered && !rightDetectionBox.isTriggered || Input.GetKey(KeyCode.DownArrow) || Device.LeftStickY < -0.5)
            {
                canWallJumpToRight = false;
                isSlidingOnWall = false;
            }
            // check if the RIGHT detection box is triggering a gameobject with the "wall" tag
            if (rightDetectionBox.isTriggered && !leftDetectionBox.isTriggered)
            {
                if (leftStickValueX > .2f || Input.GetKey(KeyCode.RightArrow))
                {
                    canWallJumpToLeft = true;
                    isSlidingOnWall = true;
                }
            }
            else if (!rightDetectionBox.isTriggered && !leftDetectionBox.isTriggered || Input.GetKey(KeyCode.DownArrow) || Device.LeftStickY < -0.5)
            {
                canWallJumpToLeft = false;
                isSlidingOnWall = false;
            }


            float horizontalSpeed = playerRigidbody.velocity.x / groundedMaxVelocity;
            if (horizontalSpeed < .0f)
                horizontalSpeed *= -1;
            //Debug.Log("horizontal speed percent = " + horizontalSpeed);

            if (isGrounded)
            {
                characterAnimator.SetBool("isGrounded", true);
                characterAnimator.SetFloat("horizontalSpeed", horizontalSpeed);
            }
            else
                characterAnimator.SetBool("isGrounded", false);

            if (playerRigidbody.velocity.y > 0f)
            {
                characterAnimator.SetBool("isJumping", true);
            }
            else
            {
                characterAnimator.SetBool("isJumping", false);
            }

            if (!isGrounded && playerRigidbody.velocity.y < 7f)
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
                straightPunchColliderAnimator.SetBool("enableRightColliderAnimation", false);
                straightPunchColliderAnimator.SetBool("enableLeftColliderAnimation", false);
            }

            //if (inputOvercut)
            //{
            //    characterAnimator.SetBool("isOvercuting", true);
            //}
            //else
            //{
            //    characterAnimator.SetBool("isOvercuting", false);
            //    //overcutCollidersAnimator.SetBool("enableRightColliderAnimation", false);
            //    //overcutCollidersAnimator.SetBool("enableLeftColliderAnimation", false);
            //}

            if (inputUppercut)
            {
                characterAnimator.SetBool("isUppercuting", true);
            }
            else
            {
                characterAnimator.SetBool("isUppercuting", false);
                uppercutColliderAnimator.SetBool("enableRightColliderAnimation", false);
                uppercutColliderAnimator.SetBool("enableLeftColliderAnimation", false);
            }

            if (inputDownAir)
            {
                characterAnimator.SetBool("isDownAir", true);
            }
            else
            {
                characterAnimator.SetBool("isDownAir", false);
                uppercutColliderAnimator.SetBool("enableRightColliderAnimation", false);
                uppercutColliderAnimator.SetBool("enableLeftColliderAnimation", false);
            }

            if (isFacingRight && characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Punch_001") && characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < .5f)
                straightPunchColliderAnimator.SetBool("enableRightColliderAnimation", true);
            //if (isFacingRight && characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Punch_001") && characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < .5f)
                //downAirColliderAnimator.SetBool("enableRightColliderAnimation", true);
            if (isFacingRight && characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Uppercut") && characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < .5f)
                uppercutColliderAnimator.SetBool("enableRightColliderAnimation", true);


            if (isFacingLeft && characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Punch_001") && characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < .5f)
                straightPunchColliderAnimator.SetBool("enableLeftColliderAnimation", true);
            //if (isFacingLeft && characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("DownAir") && characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < .5f)
                //downAirCollidersAnimator.SetBool("enableLeftColliderAnimation", true);
            if (isFacingLeft && characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Uppercut") && characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < .5f)
                uppercutColliderAnimator.SetBool("enableLeftColliderAnimation", true);



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
    }


    void FixedUpdate()
    {
        if (playerRigidbody.velocity.y <= -fallingMaxVelocity)
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -fallingMaxVelocity);

        if (isPowered)
        {
            // pass through plateform when grounded
            if (willPassThroughPlateform)
            {
                foreach (Collider2D _collider in gameManagerScript.plateformColliders)
                {
                    if (_collider.name == gameManagerScript.plateformName)
                    {
                        Physics2D.IgnoreCollision(playerCollider, _collider, true);
                    }
                }
            }
            else if (!willPassThroughPlateform)
            {
                foreach (Collider2D _collider in gameManagerScript.plateformColliders)
                {
                    if (_collider.name == gameManagerScript.plateformName)
                    {
                        Physics2D.IgnoreCollision(playerCollider, _collider, false);
                    }
                }
            }

            // pass through plateform when not grounded/in the air
            Vector2 rayStart = new Vector2(xPos, yPos - transform.localScale.y / 2f);
            Vector2 rayEnd = new Vector2(xPos, yPos - transform.localScale.y / 1f);

            if (willPassThroughPlateformRaycast && !isGrounded)
            {
                isHitingPlateform = Physics2D.Raycast(rayStart, Vector2.down, rayStart.y - rayEnd.y, oneWayPlateformMask);
                //Debug.Log(rayStart.y - rayEnd.y);

                if (isHitingPlateform)
                {
                    Physics2D.IgnoreCollision(playerCollider, isHitingPlateform.transform.gameObject.GetComponent<Collider2D>(), true);
                    //Debug.DrawLine(rayStart, rayEnd, Color.green, .5f);
                }
                else
                {
                    //Debug.DrawLine(rayStart, rayEnd, Color.red, .5f);
                    willPassThroughPlateformRaycast = false;
                }
            }


            // horizontal grounded movement
            if (isGrounded && inputRight && playerRigidbody.velocity.x < analogGroundedMaxVelocity)
            {
                isFacingLeft = false;
                isFacingRight = true;
                float _maxContribution = Mathf.Max(.0f, analogGroundedMaxVelocity - playerRigidbody.velocity.x);
                float _acceleration = Mathf.Min(_maxContribution, groundedAcceleration);
                playerRigidbody.velocity += new Vector2(_acceleration, .0f);
            }
            else if (isGrounded && inputLeft && playerRigidbody.velocity.x > -analogGroundedMaxVelocity)
            {
                isFacingRight = false;
                isFacingLeft = true;
                float _maxContribution = Mathf.Max(0, analogGroundedMaxVelocity + playerRigidbody.velocity.x);
                float _acceleration = Mathf.Min(_maxContribution, groundedAcceleration);
                playerRigidbody.velocity += new Vector2(-_acceleration, .0f);
            }
            if (isGrounded && !inputLeft && !inputRight)
            {
                playerRigidbody.velocity = new Vector2(0f, playerRigidbody.velocity.y);
            }

            // horizontal airborn movement
            if (!isGrounded && inputRight && playerRigidbody.velocity.x < analogAirMaxVelocity)
            {
                //isFacingLeft = false;
                //isFacingRight = true;
                float _maxContribution = Mathf.Max(0, analogAirMaxVelocity - playerRigidbody.velocity.x);
                float _acceleration = Mathf.Min(_maxContribution, airAcceleration);
                playerRigidbody.velocity += new Vector2(_acceleration, .0f);
            }
            else if (!isGrounded && inputLeft && playerRigidbody.velocity.x > -analogAirMaxVelocity)
            {
                //isFacingRight = false;
                //isFacingLeft = true;
                float _maxContribution = Mathf.Max(0, analogAirMaxVelocity + playerRigidbody.velocity.x);
                float _acceleration = Mathf.Min(_maxContribution, airAcceleration);
                playerRigidbody.velocity += new Vector2(-_acceleration, .0f);
            }

            // horizontal deceleration when joystick not pushed
            if (!isGrounded && !inputLeft && !inputRight && playerRigidbody.velocity.x < .0f)
            {
                float _deceleration = .3f;
                playerRigidbody.velocity += new Vector2(_deceleration, .0f);
            }
            else if (!isGrounded && !inputLeft && !inputRight && playerRigidbody.velocity.x > .0f)
            {
                float _deceleration = .3f;
                playerRigidbody.velocity += new Vector2(-_deceleration, .0f);
            }

            // jump
            if (inputJump && playerRigidbody.velocity.y == .0f)
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, .0f);
                playerRigidbody.AddForce(transform.up * jumpVelocity, ForceMode2D.Impulse);

                Instantiate(jumpVFX, new Vector3(xPos, yPos - this.transform.localScale.y / 2.0f, transform.position.z), Quaternion.identity);
                inputJump = false;
            }
            if (playerRigidbody.velocity.y < .0f)
            {
                playerRigidbody.gravityScale = fallMultiplier;
            }
            else if (playerRigidbody.velocity.y > .0f)
            {
                if (inputJumpHolded)
                    playerRigidbody.gravityScale = lowJumpMultiplier;
                else
                    playerRigidbody.gravityScale = fallMultiplier;
            }
            else if (isGrounded)
            {
                playerRigidbody.gravityScale = 1.0f;
            }
            else
            {
                inputJumpHolded = false;
            }

            // wall jump using side boxes (trigger) detection
            if (isSlidingOnWall)
            {
                maxVerticalVelocity = 8.0f;

                if (!isGrounded && canWallJumpToRight)
                {
                    isFacingLeft = false;
                    isFacingRight = true;

                    if (inputWallJump)
                    {
                        playerRigidbody.velocity = new Vector2(.0f, .0f);
                        Vector2 _wallJumpForce = new Vector2(horizontalForce, verticalForce);
                        playerRigidbody.AddForce(_wallJumpForce, ForceMode2D.Impulse);
                        isSlidingOnWall = false;
                    }
                }
                if (!isGrounded && canWallJumpToLeft)
                {
                    isFacingRight = false;
                    isFacingLeft = true;

                    if (inputWallJump)
                    {
                        playerRigidbody.velocity = new Vector2(.0f, .0f);
                        Vector2 _wallJumpForce = new Vector2(-horizontalForce, verticalForce);
                        playerRigidbody.AddForce(_wallJumpForce, ForceMode2D.Impulse);
                        isSlidingOnWall = false;
                    }
                }
                inputWallJump = false;
            }
            else
                maxVerticalVelocity = 20.0f;
        }
        else
            playerRigidbody.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Plateform")
        {
            StartCoroutine(LandingLag(landingLag));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        canCollideWithSource = true;

        if (canCollideWithSource && other.tag == "EnergySource")
        {
            currentEnergy += 30.0f;
            canCollideWithSource = false;
        }
    }

    IEnumerator LandingLag(float time)
    {
        float i = .0f;
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
            //yield return new WaitForSeconds(time / 5.0f);
        }
    }

    IEnumerator EnergyConsumption(float actionCost)
    {
        float i = .0f;
        while (i <= 1.0f)
        {
            i += 2.0f;
            currentEnergy -= actionCost;
            if (actionCost < .2f)
            {
                yield return null;/*new WaitForSeconds(1.0f);*/
            }
            else
            {
                yield return null;
            }
        }
    }

    //void OnGUI()
    //{
    //    guiStyle.fontSize = 14;
    //    Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
    //    float x = screenPos.x;
    //    float y = Screen.height - screenPos.y;

    //    GUI.Label(new Rect(x - 50.0f, y - 100.0f, 20.0f, 50.0f),
    //        //"spend energy = " + spendEnergy.ToString() + " (S)"
    //        //+ "\n" + "energy decrease = " + energyDecrease.ToString() + " (D)"
    //        /*+*/ "\n" + "energy = " + debugDisplayedEnergy.ToString() + " (E)"
    //        //+ "\n" + "vertical velocity = " + verticalVelocity.ToString()
    //        //+ "\n" + "horizontal velocity = " + horizontalVelocity.ToString()
    //        , guiStyle);
    //}
}
