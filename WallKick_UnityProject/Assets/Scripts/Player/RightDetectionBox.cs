using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightDetectionBox : MonoBehaviour {

    public bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Wall")
        {
            isTriggered = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Wall")
        {
            isTriggered = false;
        }
    }
}
