using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitWallLimiters : MonoBehaviour {

    public static bool vsMode_HiteOtherSide = false;
    public int playerSide;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (vsMode_HiteOtherSide && collision.gameObject.tag == "SplitWall")
        {
            if (playerSide == 2)
            {
                Debug.Log("player 1 gain score");
                AddPointToScore(VsModeHitOtherSide.static_p1Score, VsModeHitOtherSide.static_pointsToAdd);
            }

            else if (playerSide == 1)
            {
                Debug.Log("player 2 gain score");
                AddPointToScore(VsModeHitOtherSide.static_p2Score, VsModeHitOtherSide.static_pointsToAdd);
            }
        }
    }

    public void AddPointToScore(int playerScore, int pointsToAdd)
    {
        Debug.Log("add points to score");
        playerScore += pointsToAdd;
    }
}
