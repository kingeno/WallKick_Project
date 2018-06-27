using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSplitMovement : MonoBehaviour {

    private GUIStyle guiStyle = new GUIStyle();

    public static Rigidbody2D _rb;
    public static Transform _transform;
    public Transform bottomGear;
    public Transform forceField;
    public Transform returnPoint;

    public float maxVelocity;
    public static float horizontalVelocity;
    public static float normalizedHorizontalVelocity;
    private float GUI_normalizedHorizontalVelocity;

    public float returnPointAttractionForce;

    public bool isGeneratingEnergy = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();

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
            }
        }
    }

    private void FixedUpdate()
    {
        horizontalVelocity = _rb.velocity.x;
        normalizedHorizontalVelocity = horizontalVelocity / maxVelocity;
        GUI_normalizedHorizontalVelocity = normalizedHorizontalVelocity * 100;
        GUI_normalizedHorizontalVelocity = (int)GUI_normalizedHorizontalVelocity;

        if (_rb.velocity.x >= maxVelocity)
            _rb.velocity = new Vector2(maxVelocity, 0f);
        if (_rb.velocity.x <= -maxVelocity)
            _rb.velocity = new Vector2(-maxVelocity, 0f);

        if (!ReturnPoint.isInReturnPoint && ReturnPoint.isEnable)
            _rb.AddForce((returnPoint.transform.position - transform.position).normalized * returnPointAttractionForce * Time.deltaTime);
        if (transform.position.x == returnPoint.transform.position.x && ReturnPoint.isEnable)
            _rb.velocity = new Vector2(0f, transform.position.y);

        if (normalizedHorizontalVelocity > 0.1)
        {
            isGeneratingEnergy = true;
        }
        else
            isGeneratingEnergy = false;
    }

    public static void ApplyHorizontalForce(int forceAmount)
    {
        ReturnPoint.isEnable = false;
        _rb.velocity = Vector2.zero;
        _rb.AddForce(_transform.right * forceAmount, ForceMode2D.Impulse);
    }

    //void OnGUI()
    //{
    //    guiStyle.fontSize = 14;
    //    Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
    //    float x = screenPos.x;
    //    float y = Screen.height - screenPos.y;

    //    GUI.Label(new Rect(x - 50f, y - 100f, 20f, 50f),
    //        "% x velocity = " + GUI_normalizedHorizontalVelocity.ToString()
    //        //+ "\n" + "energy decrease = " + energyDecrease.ToString()
    //        , guiStyle);
    //}
}
