using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCenter : MonoBehaviour
{

    public GameObject splitWall;
    public Rigidbody2D splitWallRb;
    public WallSplitMovement splitWallMovement;

    private Collider2D buttonCollider;

    public int hitStrengh;
    public int bonusStrengh;
    public float superHitStrengh;
    public float freezeTime;
    public float verticalSpeed;

    private void Start()
    {
        buttonCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        //Set the horizontal position of the button on the wall
        transform.position = new Vector2(splitWall.transform.position.x, transform.position.y);
        bonusStrengh = (int)splitWallRb.velocity.x;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Punch1")
        {
            //Debug.Log("Player 1 hit the button!");
            StartCoroutine(FreezeFrame(freezeTime));
            //transform.Translate(new Vector2(transform.position.x, RandomPos()));
            //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, RandomPos(), transform.position.z), verticalSpeed * Time.deltaTime);

            float xVelocity = splitWallRb.velocity.x;

            splitWallRb.velocity = new Vector2(0f, 0f);


            if (xVelocity < 0f)
            {
                splitWallRb.AddForce(transform.right * (hitStrengh + -bonusStrengh), ForceMode2D.Impulse);
            }
            else
                splitWallRb.AddForce(transform.right * hitStrengh, ForceMode2D.Impulse);

            Debug.Log(hitStrengh);
        }

        if (other.collider.tag == "Punch2")
        {
            Debug.Log("Player 2 hit the button!");
            splitWallRb.AddForce(transform.right * -hitStrengh, ForceMode2D.Impulse);
        }
    }

    private int RandomPos()
    {
        int newRandomPos;

        int[] arrayOfPos = new int[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        newRandomPos = arrayOfPos[Random.Range(0, 10)];

        return newRandomPos;
    }

    IEnumerator FreezeFrame(float time)
    {
        float i = 0;
        while (i <= time)
        {
            i++;
            if (i < time)
            {
                Debug.Log(i);
                Time.timeScale = 0f;
            }
            else if (i < time + 1)
            {
                Debug.Log("end of coroutine");
                ScreenShake.shakeDuration = 0.1f;
                Time.timeScale = 1f;
            }
            yield return null;
        }
    }
}