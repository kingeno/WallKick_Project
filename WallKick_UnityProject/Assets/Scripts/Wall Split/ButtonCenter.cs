using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCenter : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D _collider;
    public static Collider2D buttonCollider;
    public Transform splitWallTransform;

    public GameObject splitWall;
    public Rigidbody2D splitWallRb;
    public WallSplitMovement splitWallMovement;

    public GameObject player1;
    public GameObject player2;
    public PlayerController player1Controller;
    public PlayerController player2Controller;

    public float startVelocity; // default value : 50
    public float velocityDecreaseRate; // default value : 20
    private float currentVelocity;

    public float descelerationAmplifier; // default value : 10

    public bool isNotPushed = false;
    public bool isPushedUp = false;
    public bool isPushedDown = false;

    public float limitersBounceForce;
    public bool hasHitUpperLimiter = false;
    public bool hasHitBottomLimiter = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        buttonCollider = GetComponent<Collider2D>();

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

        if (player1 != null)
            player1Controller.bonusStrength = (int)WallSplitMovement.horizontalVelocity;
        if (player2 != null)
            player2Controller.bonusStrength = -(int)WallSplitMovement.horizontalVelocity;
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
            //Debug.Log("max desceleration = " + _maxDesceleration);
            //Debug.Log("velocity decrease rate = " + velocityDecreaseRate);
            currentVelocity += _desceleration * Time.deltaTime;
            rb.velocity = new Vector2(0f, currentVelocity);
            limitersBounceForce = -currentVelocity;
        }
        else if (rb.velocity.y >= -2f && rb.velocity.y <= 0f)
        {
            rb.velocity = Vector2.zero;
        }

        if (isNotPushed)
        {
            rb.velocity = Vector2.zero;
            isNotPushed = false;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "P1_StraightPunch")
        {
            isNotPushed = true;
            Punch(1, player1Controller.hitStrength, player1Controller.totalStrengh);
        }

        if (collision.tag == "P2_StraightPunch")
        {
            isNotPushed = true;
            Punch(2, player2Controller.hitStrength, player2Controller.totalStrengh);
        }

        if (collision.tag == "P1_Uppercut")
        {
            isPushedUp = true;
            Punch(1, player1Controller.hitStrength, player1Controller.totalStrengh);
        }
        if (collision.tag == "P2_Uppercut")
        {
            isPushedUp = true;
            Punch(2, player2Controller.hitStrength, player2Controller.totalStrengh);
        }

        if (collision.tag == "P1_DownAir")
        {
            isPushedDown = true;
            Punch(1, player1Controller.hitStrength, player1Controller.totalStrengh);
        }
        if (collision.tag == "P2_DownAir")
        {
            isPushedDown = true;
            Punch(2, player2Controller.hitStrength, player2Controller.totalStrengh);
        }
    }

    private void Punch (int playerNumber, int playerStrenght, int playerTotalStrenght)
    {
        StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit));

        if (playerNumber == 1)
        {
            //Debug.Log("Player Number = " + playerNumber);

            if (WallSplitMovement.horizontalVelocity >= .0f)
            {
                splitWallMovement.ApplyHorizontalForce(playerStrenght);
                //Debug.Log("hit strengh = " + player1Controller.hitStrength);
            }
            else if (WallSplitMovement.horizontalVelocity < .0f)
            {
                splitWallMovement.ApplyHorizontalForce(playerTotalStrenght);
                //Debug.Log("hit strengh = " + player1Controller.hitStrength + " + " + (player1Controller.bonusStrength * player1Controller.bonusStrengthMultiplier) + " = " + player1Controller.totalStrengh);
            }
        }
        if (playerNumber == 2)
        {
            Debug.Log("Player Number = " + playerNumber);

            if (WallSplitMovement.horizontalVelocity <= .0f)
            {
                splitWallMovement.ApplyHorizontalForce(-playerStrenght);
                Debug.Log("hit strengh = " + player2Controller.hitStrength);
            }
            else if (WallSplitMovement.horizontalVelocity > .0f)
            {
                splitWallMovement.ApplyHorizontalForce(-playerTotalStrenght);
                Debug.Log("hit strengh = " + player2Controller.hitStrength + " + " + (player2Controller.bonusStrength * player2Controller.bonusStrengthMultiplier) + " = " + player2Controller.totalStrengh);
            }
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