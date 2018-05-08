using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCenter : MonoBehaviour
{
    private Rigidbody2D rb;

    public GameObject splitWall;
    public Rigidbody2D splitWallRb;
    public WallSplitMovement splitWallMovement;

    public GameObject player1;
    public GameObject player2;
    public PlayerController player1Controller;
    public PlayerController player2Controller;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (splitWall == null)
            splitWall = GameObject.Find("Split Wall");
        if (splitWall != null && splitWallMovement == null)
            splitWallMovement = splitWall.GetComponent<WallSplitMovement>();
    }

    private void Update()
    {
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
            player2Controller.bonusStrength = (int)WallSplitMovement.horizontalVelocity;

        //Set the horizontal position of the button on the wall
        transform.position = new Vector2(splitWall.transform.position.x, transform.position.y);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Punch1")
        {
            //Debug.Log("Player 1 hit the button!");
            StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit));
            transform.position = new Vector3(transform.position.x, RandomPos(), transform.position.z);

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

        if (other.collider.tag == "Punch2")
        {
            //Debug.Log("Player 2 hit the button!");
            StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit));
            transform.position = new Vector3(transform.position.x, RandomPos(), transform.position.z);

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
    }

    private int RandomPos()
    {
        int newRandomPos;

        int[] arrayOfPos = new int[] { 4, 6, 8, 10, 12, 14, 16, 18 };
        newRandomPos = arrayOfPos[Random.Range(0, 7)];

        return newRandomPos;
    }
}