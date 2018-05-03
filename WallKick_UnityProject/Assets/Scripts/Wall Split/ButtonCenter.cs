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
            StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit));
            transform.position = new Vector3(transform.position.x, RandomPos(), transform.position.z);

            if (xVelocity <= .0f)
            {
                splitWallRb.velocity = Vector2.zero;
                ReturnPoint.isEnable = false;
                splitWallRb.AddForce(transform.right * (hitStrengh + (-bonusStrengh * 20)), ForceMode2D.Impulse);
                Debug.Log("hit strengh = " + hitStrengh + " + " + -bonusStrengh);
            }
            else if (xVelocity >= .1f)
            {
                ReturnPoint.isEnable = false;
                splitWallRb.AddForce(transform.right * hitStrengh, ForceMode2D.Impulse);
                Debug.Log("hit strengh = " + hitStrengh);
            }
        }

        if (other.collider.tag == "Punch2")
        {
            //Debug.Log("Player 2 hit the button!");
            StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit));
            transform.position = new Vector3(transform.position.x, RandomPos(), transform.position.z);

            if (xVelocity >= .0f)
            {
                splitWallRb.velocity = Vector2.zero;
                ReturnPoint.isEnable = false;
                splitWallRb.AddForce(transform.right * (-hitStrengh + (-bonusStrengh * 20)), ForceMode2D.Impulse);
                Debug.Log("hit strengh = " + hitStrengh + " + " + bonusStrengh);
            }
            else if (xVelocity <= -.1f)
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
}