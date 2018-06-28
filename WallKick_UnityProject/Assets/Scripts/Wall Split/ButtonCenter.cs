using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCenter : MonoBehaviour
{

    public bool hasHit;

    private Rigidbody2D rb;
    private Collider2D _collider;
    public static Collider2D buttonCollider;
    public Transform splitWallTransform;

    public GameObject splitWall;
    public WallSplitMovement splitWallMovement;

    public GameObject player1;
    public GameObject player2;
    public PlayerController player1Controller;
    public PlayerController player2Controller;

    public GameObject hitVFX;

    public float startVelocity; // default value : 50
    public float SS_startVelocity; // default value : 80
    public float velocityDecreaseRate; // default value : 20
    private float currentVelocity;

    public float descelerationAmplifier; // default value : 10

    // if it hits the button
    public bool isNotPushed = false;
    public bool isPushedUp = false;
    public bool isPushedDown = false;

    // if it hits one of the sweet spots (SS) of the button
    public bool isNotPushed_SS = false;
    public bool isPushedUp_SS = false;
    public bool isPushedDown_SS = false;

    public float limitersBounceForce;
    public bool hasHitUpperLimiter = false;
    public bool hasHitBottomLimiter = false;

    private void Start()
    {
        hasHit = false;

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
        if (isPushedUp || isPushedUp_SS) //pushed up function
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
                if (isPushedUp_SS)
                    currentVelocity = SS_startVelocity;
                else
                    currentVelocity = startVelocity;
            }
            isPushedUp = false;
            isPushedUp_SS = false;
            hasHitBottomLimiter = false;
        }
        if (!isPushedUp && rb.velocity.y > 2.0f || !isPushedUp_SS && rb.velocity.y > 2.0f) //progressive vertical desceleration of the button when pushed up
        {
            float _maxDesceleration = Mathf.Max(0, 0 + currentVelocity);
            float _desceleration = Mathf.Min(_maxDesceleration, velocityDecreaseRate) * descelerationAmplifier;
            currentVelocity -= _desceleration * Time.deltaTime;
            rb.velocity = new Vector2(0f, currentVelocity);
            limitersBounceForce = -currentVelocity;
        }
        else if (rb.velocity.y <= 2f && rb.velocity.y >= 0f) //stops the button when its velocity < 2 && >= 0
        {
            rb.velocity = Vector2.zero;
        }


        if (isPushedDown || isPushedDown_SS) //pushed down function
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
                if (isPushedDown_SS)
                    currentVelocity = -SS_startVelocity;
                else
                    currentVelocity = -startVelocity;
            }
            isPushedDown = false;
            isPushedDown_SS = false;
            hasHitUpperLimiter = false;
        }
        if (!isPushedDown && rb.velocity.y < -2.0f || !isPushedDown_SS && rb.velocity.y < -2.0f) //progressive vertical desceleration of the button when pushed down
        {
            float _maxDesceleration = Mathf.Max(0, 0 + -currentVelocity);
            float _desceleration = Mathf.Min(_maxDesceleration, velocityDecreaseRate) * descelerationAmplifier;
            currentVelocity += _desceleration * Time.deltaTime;
            rb.velocity = new Vector2(0f, currentVelocity);
            limitersBounceForce = -currentVelocity;
        }
        else if (rb.velocity.y >= -2f && rb.velocity.y <= 0f) //stops the button when its velocity > -2 && <= 0
        {
            rb.velocity = Vector2.zero;
        }


        // reset the button vertical velocity and booleans
        if (isNotPushed || isNotPushed_SS)
        {
            rb.velocity = Vector2.zero;
            isNotPushed = false;
            isNotPushed_SS = false;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Button_UpperLimiter") //when the button hit the upper limiter
        {
            hasHitUpperLimiter = true;
            isPushedDown = true;
        }
        else if (collision.gameObject.tag == "Button_BottomLimiter") //when the button hit the bottom limiter
        {
            hasHitBottomLimiter = true;
            isPushedUp = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // PLAYER 1
        if (player1Controller != null)
        {
            if (!player1Controller.hasJustHitButton && collision.tag == "P1_StraightPunch" && !isNotPushed_SS)
            {
                isNotPushed = true;
                player1Controller.Punch(1, player1Controller.hitStrength, player1Controller.totalStrengh);
                Instantiate(hitVFX, collision.transform.position, Quaternion.identity);
                StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit, 0.1f));
            }
            if (!player1Controller.hasJustHitButton && collision.tag == "P1_Uppercut" && !isPushedUp_SS)
            {
                isPushedUp = true;
                player1Controller.Punch(1, player1Controller.hitStrength, player1Controller.totalStrengh);
                Instantiate(hitVFX, collision.transform.position, Quaternion.identity);
                StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit, 0.1f));
            }
            if (!player1Controller.hasJustHitButton && collision.tag == "P1_DownAir" && !isPushedDown_SS)
            {
                isPushedDown = true;
                player1Controller.Punch(1, player1Controller.hitStrength, player1Controller.totalStrengh);
                Instantiate(hitVFX, collision.transform.position, Quaternion.identity);
                StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit, 0.1f));
            }
        }

        // PLAYER 2
        if (player2Controller != null)
        {
            if (!player2Controller.hasJustHitButton && collision.tag == "P2_StraightPunch" && !isNotPushed_SS)
            {
                isNotPushed = true;
                player2Controller.Punch(2, player2Controller.hitStrength, player2Controller.totalStrengh);
                Instantiate(hitVFX, collision.transform.position, Quaternion.identity);
                StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit, 0.1f));
            }
            if (!player2Controller.hasJustHitButton && collision.tag == "P2_Uppercut" && !isPushedUp_SS)
            {
                isPushedUp = true;
                player2Controller.Punch(2, player2Controller.hitStrength, player2Controller.totalStrengh);
                Instantiate(hitVFX, collision.transform.position, Quaternion.identity);
                StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit, 0.1f));
            }
            if (!player2Controller.hasJustHitButton && collision.tag == "P2_DownAir" && !isPushedDown_SS)
            {
                isPushedDown = true;
                player2Controller.Punch(2, player2Controller.hitStrength, player2Controller.totalStrengh);
                Instantiate(hitVFX, collision.transform.position, Quaternion.identity);
                StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit, 0.1f));
            }
        }
    }

    //private int RandomPos()
    //{
    //    int newRandomPos;

    //    int[] arrayOfPos = new int[] { 4, 6, 8, 10, 12, 14, 16, 18 };
    //    newRandomPos = arrayOfPos[Random.Range(0, 7)];

    //    return newRandomPos;
    //}
}