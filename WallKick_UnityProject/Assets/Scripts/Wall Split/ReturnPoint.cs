using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnPoint : MonoBehaviour {

    public static bool isInReturnPoint = false;
    public static bool isEnable;

    private void Awake()
    {
        isEnable = true;
    }

    private void FixedUpdate()
    {
        if (WallSplitMovement.horizontalVelocity != 0f)
        {
            isEnable = true;
        }
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.gameObject.tag == "SplitWall" && isEnable)
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
