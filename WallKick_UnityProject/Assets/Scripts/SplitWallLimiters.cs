using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitWallLimiters : MonoBehaviour {

    public static bool vsMode_HiteOtherSide = false;
    public bool isHit;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (vsMode_HiteOtherSide && collision.gameObject.tag == "SplitWall")
        {
            isHit = true;
        }
    }
}
