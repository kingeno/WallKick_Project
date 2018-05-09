using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCenter : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D _collider;
    public Transform splitWallTransform;

    public GameObject splitWall;
    public Rigidbody2D splitWallRb;
    public WallSplitMovement splitWallMovement;

    public GameObject player1;
    public GameObject player2;
    public PlayerController player1Controller;
    public PlayerController player2Controller;
    public GameObject player1Punch;
    public Collider2D player1PunchCollider;


    public float startVelocity;
    public float velocityDecreaseRate;
    private float currentVelocity;

    public float descelerationAmplifier;

    public bool isPushedUp = false;
    public bool isPushedDown = false;

    public float limitersBounceForce;
    public bool hasHitUpperLimiter = false;
    public bool hasHitBottomLimiter = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        splitWallTransform = splitWallTransform.GetComponent<Transform>();

        if (splitWall == null)
            splitWall = GameObject.Find("Split Wall");
        if (splitWall != null && splitWallMovement == null)
            splitWallMovement = splitWall.GetComponent<WallSplitMovement>();

        currentVelocity = startVelocity;
    }

    private void Update()
    {
        //Set the horizontal position of the button on the wall
        transform.position = new Vector2(splitWallTransform.transform.position.x, transform.position.y);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("up");
            isPushedUp = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("down");
            isPushedDown = true;
        }

        if (GameManager.isPlayer1Active && player2 == null)
        {
            player1 = GameObject.FindGameObjectWithTag("Player1");
            player1Controller = player1.GetComponent<PlayerController>();
        }
        if (GameManager.isPlayer2Active && player2 == null)
        {
            player2 = GameObject.FindGameObjectWithTag("Player2");
            player2Controller = player2.GetComponent<PlayerController>();
        }
        if (GameManager.isPlayer1Active && player1PunchCollider == null)
        {
            player1Punch = GameObject.FindGameObjectWithTag("Punch1");
            player1PunchCollider = player1Punch.GetComponent<Collider2D>();
        }

        if (player1 != null)
            player1Controller.bonusStrength = (int)WallSplitMovement.horizontalVelocity;
        if (player2 != null)
            player2Controller.bonusStrength = (int)WallSplitMovement.horizontalVelocity;
    }


    private void FixedUpdate()
    {
        if (isPushedUp)
        {
            rb.velocity = Vector2.zero;
            if (hasHitBottomLimiter)
            {
                rb.velocity = new Vector2(0f, limitersBounceForce);
                currentVelocity = limitersBounceForce;
            }
            else
            {
                rb.velocity = new Vector2(0f, startVelocity);
                currentVelocity = startVelocity;
            }
            isPushedUp = false;
            hasHitBottomLimiter = false;
        }
        if (!isPushedUp && rb.velocity.y > 2.0f)
        {
            float _maxDesceleration = Mathf.Max(0, 0 + currentVelocity);
            float _desceleration = Mathf.Min(_maxDesceleration, velocityDecreaseRate) * descelerationAmplifier;
            currentVelocity -= _desceleration * Time.deltaTime;
            rb.velocity = new Vector2(0f, currentVelocity);
            limitersBounceForce = -currentVelocity;
        }
        else if (rb.velocity.y <= 2f && rb.velocity.y >= 0f)
        {
            rb.velocity = Vector2.zero;
        }


        if (isPushedDown)
        {
            rb.velocity = Vector2.zero;
            if (hasHitUpperLimiter)
            {
                rb.velocity = new Vector2(0f, limitersBounceForce);
                currentVelocity = limitersBounceForce;
            }
            else
            {
                rb.velocity = new Vector2(0f, -startVelocity);
                currentVelocity = -startVelocity;
            }
            isPushedDown = false;
            hasHitUpperLimiter = false;
        }
        if (!isPushedDown && rb.velocity.y < -2.0f)
        {
            float _maxDesceleration = Mathf.Max(0, 0 + -currentVelocity);
            float _desceleration = Mathf.Min(_maxDesceleration, velocityDecreaseRate) * descelerationAmplifier;
            Debug.Log("max desceleration = " + _maxDesceleration);
            Debug.Log("velocity decrease rate = " + velocityDecreaseRate);
            currentVelocity += _desceleration * Time.deltaTime;
            rb.velocity = new Vector2(0f, currentVelocity);
            limitersBounceForce = -currentVelocity;
        }
        else if (rb.velocity.y >= -2f && rb.velocity.y <= 0f)
        {
            rb.velocity = Vector2.zero;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Punch1")
        {
            //Debug.Log("Player 1 hit the button!");
            StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit));
            //transform.position = new Vector3(transform.position.x, RandomPos(), transform.position.z);
            Debug.Log("down");
            isPushedDown = true;
            if (WallSplitMovement.horizontalVelocity < .0f)
            {
                splitWallMovement.ApplyHorizontalForce(player1Controller.totalStrengh);
                Debug.Log("hit strengh = " + player1Controller.hitStrength + " + " + (player1Controller.bonusStrength * player1Controller.bonusStrengthMultiplier) + " = " + player1Controller.totalStrengh);
            }
            else if (WallSplitMovement.horizontalVelocity >= .0f)
            {
                splitWallMovement.ApplyHorizontalForce(player1Controller.hitStrength);
                Debug.Log("hit strengh = " + player1Controller.hitStrength);
            }
        }

        if (collision.gameObject.tag == "Punch2")
        {
            //Debug.Log("Player 2 hit the button!");
            StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit));
            //transform.position = new Vector3(transform.position.x, RandomPos(), transform.position.z);
            Debug.Log("up");
            isPushedUp = true;

            if (WallSplitMovement.horizontalVelocity > .0f)
            {
                splitWallMovement.ApplyHorizontalForce(-player2Controller.totalStrengh);
                Debug.Log("hit strengh = " + player2Controller.hitStrength + " + " + (player2Controller.bonusStrength * player1Controller.bonusStrengthMultiplier) + " = " + player2Controller.totalStrengh);
            }
            else if (WallSplitMovement.horizontalVelocity <= .0f)
            {
                splitWallMovement.ApplyHorizontalForce(-player2Controller.hitStrength);
                Debug.Log("hit strengh = " + -player2Controller.hitStrength);
            }
            //ReturnPoint.isEnable = true;
        }

        if (collision.gameObject.tag == "Button_UpperLimiter")
        {
            Debug.Log("Button hit upper limiter");
            hasHitUpperLimiter = true;
            isPushedDown = true;
        }
        else if (collision.gameObject.tag == "Button_BottomLimiter")
        {
            Debug.Log("Button hit bottom limiter");
            hasHitBottomLimiter = true;
            isPushedUp = true;
        }
    }

    private int RandomPos()
    {
        int newRandomPos;

        int[] arrayOfPos = new int[] { 4, 6, 8, 10, 12, 14, 16, 18 };
        newRandomPos = arrayOfPos[Random.Range(0, 7)];

        return newRandomPos;
    }
}