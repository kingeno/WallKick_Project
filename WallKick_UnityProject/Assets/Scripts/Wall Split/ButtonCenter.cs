using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCenter : MonoBehaviour {

    
    public GameObject splitWall;
    public WallSplitMovement splitWallMovement;

    private Collider2D buttonCollider;

    public float hitStrengh;


    private void Start()
    {
        buttonCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (EndZone.gameIsOver)
        {
            buttonCollider.enabled = !buttonCollider;
        }

        //Set the horizontal position of the button on the wall
        transform.position = new Vector2(splitWall.transform.position.x, transform.position.y);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!splitWallMovement.isMoving)
        {
            if (other.collider.tag == "Punch1")
            {
                Debug.Log("Player 1 hit the button!");

                int _newRandPos = RandomPos();
                transform.position = new Vector2(splitWall.transform.position.x, _newRandPos);

                splitWallMovement.startHitAnimation();
                splitWallMovement.MoveRightCoroutine();

                //splitWallMovement.OnButtonHit(hitStrengh);
            }

            if (other.collider.tag == "Punch2")
            {
                Debug.Log("Player 2 hit the button!");

                int _newRandPos = RandomPos();
                transform.position = new Vector2(splitWall.transform.position.x, _newRandPos);

                splitWallMovement.startHitAnimation();
                splitWallMovement.MoveLeftCoroutine();

                //splitWallMovement.OnButtonHit(-hitStrengh);
            }
        }
    }

    private int RandomPos()
    {
        int newRandomPos;

        int[] arrayOfPos = new int[] {4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        newRandomPos = arrayOfPos[Random.Range(0, 10)];

        return newRandomPos;
    }

    //IEnumerator MoveSplitWall(float time)
    //{
    //    float i = 0;
    //    while (i <= 2)
    //    {
    //        float currentXPos = transform.position.x;
    //        i += 0.1f;
    //        Vector2 newPos = new Vector2(currentXPos = i, transform.position.y);
    //        splitWall.transform.position = newPos;
    //        print("i = " + i);
    //        yield return new WaitForSeconds(time / 5);
    //    }
    //}
}