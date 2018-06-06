using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VsModeHitOtherSide : MonoBehaviour {

    public SplitWallLimiters rightLimiter;
    public SplitWallLimiters leftLimiter;

    public int pointsToAdd;

    private int p1Score;
    private int p2Score;

    private GUIStyle guiStyle = new GUIStyle();

    private void OnEnable()
    {
        p1Score = 0;
        p2Score = 0;
    }

    void Start () {

        SplitWallLimiters.vsMode_HiteOtherSide = true;

        guiStyle.normal.textColor = Color.white;
    }
	
	void Update () {
        if (rightLimiter.isHit == true)
        {
            p1Score += pointsToAdd;
            rightLimiter.isHit = false;
        }
        if (leftLimiter.isHit == true)
        {
            p2Score += pointsToAdd;
            leftLimiter.isHit = false;
        }
    }

    void OnGUI()
    {
        guiStyle.fontSize = 18;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        float x = screenPos.x;
        float y = Screen.height - screenPos.y;

        GUI.Label(new Rect(x - 400f, y, 20f, 50f),
            "P1 Score = " + p1Score.ToString()
            //+ "\n" + "energy decrease = " + energyDecrease.ToString()
            , guiStyle);

        GUI.Label(new Rect(x + 330f, y, 20f, 50f),
            "P2 Score = " + p2Score.ToString()
            //+ "\n" + "energy decrease = " + energyDecrease.ToString()
            , guiStyle);
    }
}
