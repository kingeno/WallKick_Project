using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScritp : MonoBehaviour
{

    public Transform splitWall;

    Rigidbody2D rb;

    public float startVelocity;
    public float velocityDecreaseRate;
    private float currentVelocity;

    public float descelerationAmplifier;

    public bool isPushedUp = false;
    public bool isPushedDown = false;

    public float limitersBounceForce;
    public bool hasHitUpperLimiter = false;
    public bool hasHitBottomLimiter = false;

    void Start()
    {
        splitWall = splitWall.GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();

        currentVelocity = startVelocity;
    }

    void Update()
    {
        transform.position = new Vector2(splitWall.position.x, transform.position.y);

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
}
