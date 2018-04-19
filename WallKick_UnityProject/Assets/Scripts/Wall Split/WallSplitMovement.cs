using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSplitMovement : MonoBehaviour {

    private Rigidbody2D rb;
    public Transform bottomGear;

    public float horizontalVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        horizontalVelocity = rb.velocity.x;
    }
}
