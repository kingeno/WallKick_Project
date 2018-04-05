using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndZone : MonoBehaviour {

    public bool EndZoneRight;
    public bool EndZoneLeft;
    public static bool gameIsOver;

    public GameObject endGameUI;
    public TextMeshProUGUI scoreText;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "SplitWall")
        {
            //Time.timeScale = 0f;
            gameObject.SetActive(true);
            gameIsOver = true;
            if (!EndZoneLeft && EndZoneRight)
            {
                Debug.Log("Player 1 wins");
                scoreText.text = "Player 1 Wins";
            }
            else if (!EndZoneRight && EndZoneLeft)
            {
                Debug.Log("Player 2 wins");
                scoreText.text = "Player 2 Wins";
            }
        }
    }
}
