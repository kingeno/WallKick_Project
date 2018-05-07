using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSplitMovement : MonoBehaviour {

    private GUIStyle guiStyle = new GUIStyle();

    private Rigidbody2D rb;
    private Collider2D col;
    public Transform bottomGear;
    public Transform forceField;
    public Transform returnPoint;

    public float maxVelocity;

    public static float horizontalVelocity;
    public float returnPointAttractionForce;

    public bool isGeneratingEnergy = false;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        guiStyle.normal.textColor = Color.white;


        if (maxVelocity < 5f)
            Debug.LogError("Check the WallSplit \"maxVelocity\" value in the Inspector, it must be at 15 minimum");
    }

    private void Update()
    {
        forceField.transform.position = transform.position;

        if (horizontalVelocity != 0)
        {
            if (ReturnPoint.isInReturnPoint && horizontalVelocity <= 1.9f && horizontalVelocity >= -1.9f)
            {
                transform.position = Vector3.MoveTowards(transform.position, returnPoint.position, 1.2f * Time.deltaTime);
                isGeneratingEnergy = false;
            }
            else
                isGeneratingEnergy = true;
        }
    }

    private void FixedUpdate()
    {
        horizontalVelocity = rb.velocity.x;

        if (rb.velocity.x >= maxVelocity)
            rb.velocity = new Vector2(maxVelocity, 0f);
        if (rb.velocity.x <= -maxVelocity)
            rb.velocity = new Vector2(-maxVelocity, 0f);

        if (!ReturnPoint.isInReturnPoint && ReturnPoint.isEnable)
            rb.AddForce((returnPoint.transform.position - transform.position).normalized * returnPointAttractionForce * Time.deltaTime);
        if (transform.position.x == returnPoint.transform.position.x && ReturnPoint.isEnable)
            rb.velocity = new Vector2(0f, transform.position.y);
    }

    public void ApplyHorizontalForce(int forceAmount)
    {
        ReturnPoint.isEnable = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(transform.right * forceAmount, ForceMode2D.Impulse);
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
