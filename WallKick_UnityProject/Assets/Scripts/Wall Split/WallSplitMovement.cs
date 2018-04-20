using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSplitMovement : MonoBehaviour {

    private GUIStyle guiStyle = new GUIStyle();

    private Rigidbody2D rb;
    public Transform bottomGear;
    public Transform forceField;
    public Transform returnPoint;
    
    public float horizontalVelocity;
    public float forceAmount;
    public float speed;

    public bool isGeneratingEnergy = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        guiStyle.normal.textColor = Color.white;
    }

    private void Update()
    {
        forceField.transform.position = transform.position;

        if (horizontalVelocity != 0)
        {
            if (ReturnPoint.isInReturnPoint && horizontalVelocity <= 0.5f && horizontalVelocity >= -0.5f)
            {
                transform.position = returnPoint.transform.position;
                isGeneratingEnergy = false;
            }
            else
                isGeneratingEnergy = true;
        }
    }

    private void FixedUpdate()
    {
        horizontalVelocity = rb.velocity.x;



        if (!ReturnPoint.isInReturnPoint)
            rb.AddForce((returnPoint.transform.position - transform.position).normalized * forceAmount * Time.deltaTime);
        if (transform.position.x == returnPoint.transform.position.x)
            rb.velocity = new Vector2(0f, transform.position.y);

    }

    void OnGUI()
    {
        guiStyle.fontSize = 14;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        float x = screenPos.x;
        float y = Screen.height - screenPos.y;

        GUI.Label(new Rect(x - 50f, y - 100f, 20f, 50f),
            "x velocity = " + horizontalVelocity.ToString()
            //+ "\n" + "energy decrease = " + energyDecrease.ToString()
            , guiStyle);
    }
}
