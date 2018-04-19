using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCenter : MonoBehaviour
{

    public GameObject splitWall;
    public Rigidbody2D splitWallRb;
    public WallSplitMovement splitWallMovement;

    private Collider2D buttonCollider;

    public float hitStrengh;

    private void Start()
    {
        buttonCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        //Set the horizontal position of the button on the wall
        transform.position = new Vector2(splitWall.transform.position.x, transform.position.y);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Punch1")
        {
            Debug.Log("Player 1 hit the button!");
            transform.position = new Vector3(transform.position.x, RandomPos(), transform.position.y);
            splitWallRb.AddForce(transform.right * hitStrengh, ForceMode2D.Impulse);
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
}