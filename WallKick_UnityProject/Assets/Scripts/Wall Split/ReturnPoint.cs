using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnPoint : MonoBehaviour {

    public static bool isInReturnPoint = false;

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.gameObject.tag == "SplitWall")
        {
            //Debug.Log(isInReturnPoint);
            isInReturnPoint = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "SplitWall")
        {
            //Debug.Log(isInReturnPoint);
            isInReturnPoint = false;
        }
    }

}
