using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchesCollisionManagement : MonoBehaviour
{

    private Animator _animator;

    public GameObject player1;
    public GameObject player2;

    public PlayerController player1Controller;
    public PlayerController player2Controller;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }


    void Update()
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

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Straight_ColliderAnim") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .96f)
        {
            Physics2D.IgnoreLayerCollision(9, 11, false);
            //Debug.Log("SRAIGHT PUNCH - collision ignored = " + Physics2D.GetIgnoreLayerCollision(9, 11));
            if (tag == "P1_StraightPunch")
                player1Controller.hasJustHitButton = false;
            if (tag == "P2_StraightPunch")
                player2Controller.hasJustHitButton = false;
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Uppercut_ColliderAnim") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .96f)
        {
            Physics2D.IgnoreLayerCollision(9, 11, false);
            //Debug.Log("UPPERCUT - collision ignored = " + Physics2D.GetIgnoreLayerCollision(9, 11));
            if (tag == "P1_Uppercut")
                player1Controller.hasJustHitButton = false;
            if (tag == "P2_Uppercut")
                player2Controller.hasJustHitButton = false;
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("DownAir_ColliderAnim") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .96f)
        {
            Physics2D.IgnoreLayerCollision(9, 11, false);
            //Debug.Log("DOWN AIR - collision ignored = " + Physics2D.GetIgnoreLayerCollision(9, 11));
            if (tag == "P1_DownAir")
                player1Controller.hasJustHitButton = false;
            if (tag == "P2_DownAir")
                player2Controller.hasJustHitButton = false;
        }
    }
}
