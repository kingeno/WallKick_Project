using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchesCollisionManagement : MonoBehaviour {

    private Collider2D _collider;
    private Animator _animator;

    private string straigtPunch_L_AnimationName;
    private string straigtPunch_R_AnimationName;
    private string uppercut_L_AnimationName;
    private string uppercut_R_AnimationName;

    void Start () {

        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();

        straigtPunch_L_AnimationName = "PunchColliderAnimationToLeft";
        straigtPunch_R_AnimationName = "PunchColliderAnimationToRight";
        uppercut_L_AnimationName = "Uppercut_ColliderAnimation_L";
        uppercut_R_AnimationName = "Uppercut_ColliderAnimation_R";
	}
	

	void Update () {

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(straigtPunch_L_AnimationName) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .9f)
        {
            Debug.Log("Retablit les collision (Left)");
            Physics2D.IgnoreCollision(_collider, ButtonCenter.buttonCollider, false);
        }
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(straigtPunch_R_AnimationName) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .9f)
        {
            Debug.Log("Retablit les collision (Right)");
            Physics2D.IgnoreCollision(_collider, ButtonCenter.buttonCollider, false);
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(uppercut_L_AnimationName) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .8f)
        {
            Debug.Log("Retablit les collision (Left)");
            Physics2D.IgnoreCollision(_collider, ButtonCenter.buttonCollider, false);
        }
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(uppercut_R_AnimationName) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .8f)
        {
            Debug.Log("Retablit les collision (Right)");
            Physics2D.IgnoreCollision(_collider, ButtonCenter.buttonCollider, false);
        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Physics2D.IgnoreCollision(_collider, ButtonCenter.buttonCollider, true);
    }
}
