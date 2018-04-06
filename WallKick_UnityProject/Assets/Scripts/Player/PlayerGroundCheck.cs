using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour {

    public PlayerController _playerController;
    public string plateformName;
    public bool isGrounded = false;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ground" || other.tag == "Plateform")
        {
            isGrounded = true;
            if(other.tag == "Plateform" && !_playerController.passThroughPlateformCoroutine)
            {
                plateformName = other.name;
            }
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
