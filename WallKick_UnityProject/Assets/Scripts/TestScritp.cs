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
            rb.velocity = new Vector2(0f, startVelocity);
            currentVelocity = startVelocity;
            isPushedUp = false;
        }
        if (!isPushedUp && rb.velocity.y > 2.0f)
        {
            float _maxDesceleration = Mathf.Max(0, 0 + currentVelocity);
            float _desceleration = Mathf.Min(_maxDesceleration, velocityDecreaseRate) * descelerationAmplifier;
            currentVelocity -= _desceleration * Time.deltaTime;
            rb.velocity = new Vector2(0f, currentVelocity);
        }
        else if (rb.velocity.y <= 2f && rb.velocity.y >= 0f)
        {
            rb.velocity = Vector2.zero;
        }



        if (isPushedDown)
        {
            rb.velocity = Vector2.zero;
            rb.velocity = new Vector2(0f, -startVelocity);
            currentVelocity = -startVelocity;
            isPushedDown = false;
        }
        if (!isPushedDown && rb.velocity.y < -2.0f)
        {
            float _maxDesceleration = Mathf.Min(0, 0 + currentVelocity);
            float _desceleration = Mathf.Max(-_maxDesceleration, -velocityDecreaseRate) * descelerationAmplifier;
            currentVelocity += _desceleration * Time.deltaTime;
            rb.velocity = new Vector2(0f, currentVelocity);
        }
        else if (rb.velocity.y >= -2f && rb.velocity.y <= 0f)
        {
            rb.velocity = Vector2.zero;
        }
    }
}
