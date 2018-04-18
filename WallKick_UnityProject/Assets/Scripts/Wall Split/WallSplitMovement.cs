using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSplitMovement : MonoBehaviour {


    public Transform bottomGear;

    // ----- SPLIT WALL MOVEMENT USING LERP -----
    public bool isMoving { get; private set; }
    public bool checkIfMoving;
    public float distanceToMove;
    [Range(0f, 2f)]
    public float timeToMove;
    
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }   

    private void Update()
    {
        checkIfMoving = isMoving;
    }

    public void MoveRightCoroutine()
    {
        float _currentXPos = transform.position.x;
        StartCoroutine(MoveToPosition(new Vector2(_currentXPos += distanceToMove, transform.position.y), timeToMove));
    }

    public void MoveLeftCoroutine()
    {
        float _currentXPos = transform.position.x;
        StartCoroutine(MoveToPosition(new Vector2(_currentXPos -= distanceToMove, transform.position.y), timeToMove));
    }

    public IEnumerator MoveToPosition(Vector2 targetPosition, float timeToMove)
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        if (currentPos == targetPosition)
            Debug.LogError("Check in the inspector if the values of the \"WallSplitMovement\" script are different from 0");
        float t = 0f;
        while (t < 2)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector2.Lerp(currentPos, targetPosition, t);

            if (t < 2)
            {
                //isMoving = true;
            }
            else
            {
                //isMoving = false;
                stopHitAnimation(); 
            }

            yield return null;
        }
    }

    public void startHitAnimation()
    {
        animator.SetBool("noDirectionShake", true);
        //animator.SetBool("clockwiseShake", true);
        //animator.SetBool("counterClockwiseShake", true);
    }

    public void stopHitAnimation()
    {
        animator.SetBool("noDirectionShake", false);
        //animator.SetBool("clockwiseShake", false);
        //animator.SetBool("counterClockwiseShake", false);
    }


    /*========== SPLIT WALL MOVEMENT USING ADDFORCE (PHYSICS2D) ==========*/

    //   Rigidbody2D _Rigidbody2D;

    //void Start () {
    //       _Rigidbody2D = GetComponent<Rigidbody2D>();
    //}


    //   public void OnButtonHit(float hitStrengh)
    //   {
    //       _Rigidbody2D.velocity = new Vector2(0f, 0f);
    //       _Rigidbody2D.AddForce(transform.right * hitStrengh, ForceMode2D.Impulse);
    //       transform.Translate(Vector2.right * Time.deltaTime);
    //   }
}
