using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchesCollisionManagement : MonoBehaviour {

    private Collider2D _collider;
    private Animator _animator;

    private string straigtPunch_AnimationName;
    private string uppercut_AnimationName;
    private string downAir_AnimationName;

    void Start () {

        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();

        straigtPunch_AnimationName = "Straight_ColliderAnim";
        uppercut_AnimationName = "Uppercut_ColliderAnim";
        downAir_AnimationName = "DownAir_ColliderAnim";
    }
	

	void Update () {

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Straight_ColliderAnim") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .9f)
        {
            Debug.Log("SRAIGHT PUNCH - collision enabled");
            Physics2D.IgnoreCollision(_collider, ButtonCenter.buttonCollider, false);
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Uppercut_ColliderAnim") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .9f)
        {
            Debug.Log("UPPERCUT - collision enabled");
            Physics2D.IgnoreCollision(_collider, ButtonCenter.buttonCollider, false);
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("DownAir_ColliderAnim") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .9f)
        {
            Debug.Log("DOWN AIR - collision enabled");
            Physics2D.IgnoreCollision(_collider, ButtonCenter.buttonCollider, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Physics2D.IgnoreCollision(_collider, ButtonCenter.buttonCollider, true);
    }
}
