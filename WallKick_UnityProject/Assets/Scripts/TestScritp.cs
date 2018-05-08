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
            //StartCoroutine(ButtonMovement(startVelocity, velocityDecreaseRate));
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("down");
            isPushedDown = true;
            //StartCoroutine(ButtonMovement(startVelocity, velocityDecreaseRate));
        }
    }

    private void FixedUpdate()
    {
        if (isPushedUp)
        {
            rb.velocity = new Vector2(.0f, currentVelocity);
            Debug.Log("current velocity = " + currentVelocity);

            if (currentVelocity > 1f)
            {
                float _maxDesceleration = Mathf.Max(0, 0 + currentVelocity);
                float _desceleration = Mathf.Min(_maxDesceleration, velocityDecreaseRate);
                currentVelocity -= _desceleration * Time.deltaTime;
                //rb.velocity = new Vector2(transform.position.x, currentVelocity);
            }
            else if (currentVelocity <= 1f)
            {
                Debug.Log("end of coroutine");
                isPushedUp = false;
                rb.velocity = Vector2.zero;
                currentVelocity = startVelocity;
            }
        }
    }

    public IEnumerator ButtonMovement(float startVelocity, float velocityDecreaseRate)
    {
        float currentVelocity = startVelocity;
        float decreaseRate = velocityDecreaseRate;
        float i = 0f;
        while (i < 1)
        {
            if (isPushedUp)
            {
                rb.velocity = new Vector2(.0f, currentVelocity);
                Debug.Log("current velocity = " + currentVelocity);

                if (currentVelocity > 1f)
                {
                    float _maxDesceleration = Mathf.Max(0, 0 + currentVelocity);
                    float _desceleration = Mathf.Min(_maxDesceleration * Time.deltaTime, decreaseRate * Time.deltaTime);
                    currentVelocity -= _desceleration;
                    //rb.velocity = new Vector2(transform.position.x, currentVelocity);
                    yield return null;
                }
                else if (currentVelocity <= 1f)
                {
                    Debug.Log("end of coroutine");
                    i += 2;
                    isPushedUp = false;
                    rb.velocity = Vector2.zero;
                    yield return null;
                }
            }
            //if (isPushedDown && currentVelocity < .0f)
            //{
            //    float _maxContribution = Mathf.Max(0, 0 + currentVelocity);
            //    float _acceleration = Mathf.Min(_maxContribution, decreaseRate);
            //    rb.velocity += new Vector2(.0f, -_acceleration);
            //    //rb.velocity = new Vector2(transform.position.x, currentVelocity);
            //    currentVelocity += decreaseRate * Time.deltaTime;
            //    yield return null;
            //}
            //else if (isPushedDown && currentVelocity >= 0)
            //{
            //    Debug.Log("end of coroutine");
            //    i += 2;
            //    isPushedDown = false;
            //    rb.velocity = Vector2.zero;
            //    yield return null;
            //}
            yield return null;
        }
    }
}
