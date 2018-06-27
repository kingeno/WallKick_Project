using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchesCollisionManagement : MonoBehaviour
{

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Straight_ColliderAnim") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .96f)
        {
            Physics2D.IgnoreLayerCollision(9, 11, false);
            //Debug.Log("SRAIGHT PUNCH - collision ignored = " + Physics2D.GetIgnoreLayerCollision(9, 11));
            ButtonCenter.hasHit = false;
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Uppercut_ColliderAnim") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .96f)
        {
            Physics2D.IgnoreLayerCollision(9, 11, false);
            //Debug.Log("UPPERCUT - collision ignored = " + Physics2D.GetIgnoreLayerCollision(9, 11));
            ButtonCenter.hasHit = false;
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("DownAir_ColliderAnim") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .96f)
        {
            Physics2D.IgnoreLayerCollision(9, 11, false);
            //Debug.Log("DOWN AIR - collision ignored = " + Physics2D.GetIgnoreLayerCollision(9, 11));
            ButtonCenter.hasHit = false;
        }
    }
}
