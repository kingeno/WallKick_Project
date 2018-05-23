using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VsModeHitOtherSide : MonoBehaviour {

    public SplitWallLimiters rightLimiter;
    public SplitWallLimiters leftLimiter;

    public static int static_p1Score;
    public static int static_p2Score;

    private int p1Score;
    private int p2Score;

    public int pointsToAdd;
    public static int static_pointsToAdd;

    private GUIStyle guiStyle = new GUIStyle();

    void Start () {

        SplitWallLimiters.vsMode_HiteOtherSide = true;

        static_pointsToAdd = pointsToAdd;

        guiStyle.normal.textColor = Color.white;
    }
	
	void Update () {
        p1Score = static_p1Score;
        p2Score = static_p2Score;
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
