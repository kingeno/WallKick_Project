using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour {

    public PlayerController playerController;
    public bool isGrounded = false;
    private Rigidbody2D _Rigidbody;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ground" || other.tag == "Plateform")
        {
            isGrounded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ground" || other.tag == "Plateform")
        {
            isGrounded = false;
        }
    }
}
