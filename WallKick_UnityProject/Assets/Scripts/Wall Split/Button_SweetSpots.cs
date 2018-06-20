using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_SweetSpots : MonoBehaviour
{

    public bool topSweetSpot;
    public bool middleSweetSpot;
    public bool bottomSweetSpot;

    public GameObject button;
    public ButtonCenter buttonCenterScript;

    public GameObject player1;
    public GameObject player2;

    public PlayerController player1Controller;
    public PlayerController player2Controller;

    public GameObject hitSweetSpotVFX;


    private void Update()
    {
        if (buttonCenterScript == null)
        {
            button = GameObject.FindGameObjectWithTag("Button");
            buttonCenterScript = button.GetComponent<ButtonCenter>();
        }

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
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (topSweetSpot)
        {
            if (collision.tag == "P1_DownAir")
            {
                Debug.Log("Dair SS !");
                buttonCenterScript.isPushedDown_SS = true;
                buttonCenterScript.Punch(1, player1Controller.sweetSpotHitStrength, player1Controller.sweetSpotTotalStrength);
                Instantiate(hitSweetSpotVFX, collision.transform.position, Quaternion.identity);
                StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit_sweetSpot));
            }

            if (collision.tag == "P2_DownAir")
            {
                Debug.Log("Dair SS !");
                buttonCenterScript.isPushedDown_SS = true;
                buttonCenterScript.Punch(2, player2Controller.sweetSpotHitStrength, player2Controller.sweetSpotTotalStrength);
                Instantiate(hitSweetSpotVFX, collision.transform.position, Quaternion.identity);
                StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit_sweetSpot));
            }
        }

        // ---------------------------------

        else if (middleSweetSpot)
        {
            if (collision.tag == "P1_StraightPunch")
            {
                Debug.Log("Fair SS !");
                buttonCenterScript.isNotPushed_SS = true;
                buttonCenterScript.Punch(1, player1Controller.sweetSpotHitStrength, player1Controller.sweetSpotTotalStrength);
                Instantiate(hitSweetSpotVFX, collision.transform.position, Quaternion.identity);
                StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit_sweetSpot));
            }

            if (collision.tag == "P2_StraightPunch")
            {
                Debug.Log("Fair SS !");
                buttonCenterScript.isNotPushed_SS = true;
                buttonCenterScript.Punch(2, player2Controller.sweetSpotHitStrength, player2Controller.sweetSpotTotalStrength);
                Instantiate(hitSweetSpotVFX, collision.transform.position, Quaternion.identity);
                StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit_sweetSpot));
            }
        }

        // ---------------------------------

        else if (bottomSweetSpot)
        {
            if (collision.tag == "P1_Uppercut")
            {
                //Debug.Log("Uppercut SS !");
                buttonCenterScript.isPushedUp_SS = true;
                buttonCenterScript.Punch(1, player1Controller.sweetSpotHitStrength, player1Controller.sweetSpotTotalStrength);
                Instantiate(hitSweetSpotVFX, collision.transform.position, Quaternion.identity);
                StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit_sweetSpot));
            }

            if (collision.tag == "P2_Uppercut")
            {
                Debug.Log("Uppercut SS !");
                buttonCenterScript.isPushedUp_SS = true;
                buttonCenterScript.Punch(2, player2Controller.sweetSpotHitStrength, player2Controller.sweetSpotTotalStrength);
                Instantiate(hitSweetSpotVFX, collision.transform.position, Quaternion.identity);
                StartCoroutine(GameManager.FreezeFrame(GameManager.freezeDurationWhenButtonHit_sweetSpot));
            }
        }
    }
}
