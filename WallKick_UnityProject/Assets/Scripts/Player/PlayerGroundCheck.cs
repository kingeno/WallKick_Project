using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour {

    public bool isGrounded = false;

    private GameObject gameManager;
    private GameManager gameManagerScript;


    private void Awake()
    {
        gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ground" || other.tag == "Plateform")
        {
            if (other.tag == "Plateform")
            {
                gameManagerScript.plateformName = other.name;
                //Debug.Log("plateform name = " + plateformName);
            }

            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ground" || other.tag == "Plateform")
        {
            isGrounded = false;
            gameManagerScript.plateformName = "empty";
            //Debug.Log("plateform name = " + plateformName);
        }
    }
}
