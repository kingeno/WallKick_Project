using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCenter : MonoBehaviour
{

    public GameObject splitWall;
    public Rigidbody2D splitWallRb;
    public WallSplitMovement splitWallMovement;
    private Rigidbody2D rb;

    private Collider2D buttonCollider;

    public int hitStrengh;
    private int bonusStrengh;
    public float freezeFrameTime;

    private void Start()
    {
        buttonCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //Set the horizontal position of the button on the wall
        transform.position = new Vector2(splitWall.transform.position.x, transform.position.y);
        bonusStrengh = (int)splitWallRb.velocity.x;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        float xVelocity = splitWallRb.velocity.x;

        

        if (other.collider.tag == "Punch1")
        {
            //Debug.Log("Player 1 hit the button!");

            transform.position = new Vector3(transform.position.x, RandomPos(), transform.position.z);
            //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, RandomPos(), transform.position.z), 1f * Time.deltaTime);
            StartCoroutine(FreezeFrame(freezeFrameTime));
            //transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, RandomPos(), transform.position.z), 1.2f * Time.deltaTime);

            if (xVelocity <= 0.0f)
            {
                splitWallRb.velocity = new Vector2(0f, 0f);
                ReturnPoint.isEnable = false;
                splitWallRb.AddForce(transform.right * (hitStrengh + (-bonusStrengh * 20)), ForceMode2D.Impulse);
                Debug.Log("hit strengh = " + hitStrengh + " + " + -bonusStrengh);
            }
            else if (xVelocity >= 0.1f)
            {
                ReturnPoint.isEnable = false;
                splitWallRb.AddForce(transform.right * hitStrengh, ForceMode2D.Impulse);
                Debug.Log("hit strengh = " + hitStrengh);
            }
        }

        if (other.collider.tag == "Punch2")
        {
            //Debug.Log("Player 2 hit the button!");
            transform.position = new Vector3(transform.position.x, RandomPos(), transform.position.z);
            //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, RandomPos(), transform.position.z), 0.1f);
            StartCoroutine(FreezeFrame(freezeFrameTime));
            //transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, RandomPos(), transform.position.z), 1.2f * Time.deltaTime);

            if (xVelocity >= 0.0f)
            {
                splitWallRb.velocity = new Vector2(0f, 0f);
                ReturnPoint.isEnable = false;
                splitWallRb.AddForce(transform.right * (-hitStrengh + (-bonusStrengh * 20)), ForceMode2D.Impulse);
                Debug.Log("hit strengh = " + hitStrengh + " + " + bonusStrengh);
            }
            else if (xVelocity <= -0.1f)
            {
                ReturnPoint.isEnable = false;
                splitWallRb.AddForce(transform.right * -hitStrengh, ForceMode2D.Impulse);
                Debug.Log("hit strengh = " + hitStrengh);
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

    IEnumerator FreezeFrame(float time)
    {
        float i = .0f;
        while (i <= time)
        {
            i++;
            if (i < time)
            {
                //Debug.Log(i);
                Time.timeScale = .0f;
            }
            else if (i < time + 1)
            {
                //Debug.Log("end of coroutine");
                ScreenShake.shakeDuration = .1f;
                Time.timeScale = 1.0f;
            }
            yield return null;
        }
    }
}