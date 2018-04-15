using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

    Animator animator;
    Rigidbody2D rb2D;

	// Use this for initialization
	void Start () {
        animator = GetComponentInChildren<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        float speedPercent = rb2D.velocity.x;
        animator.SetFloat("speedPercent", speedPercent, .1f, Time.deltaTime);
	}
}
