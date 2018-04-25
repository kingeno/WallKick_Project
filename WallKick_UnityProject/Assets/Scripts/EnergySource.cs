using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySource : MonoBehaviour {


    private GUIStyle guiStyle = new GUIStyle();
    public bool debugDisplay;

    private BoxCollider2D boxCol;

    public static float maxLoad = 30f;

    public float timeToCharge;
    public bool isCharged = true;

    private void Awake()
    {
        debugDisplay = true;
        guiStyle.normal.textColor = Color.white;

        boxCol = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player1" || other.tag == "Player2")
        {
            boxCol.enabled = false;
            if (isCharged)
            {
                isCharged = false;
                StartCoroutine(chargingSource(timeToCharge));
            }
        }
    }

    IEnumerator chargingSource(float time)
    {
        float i = 0f;
        while (i <= time)
        {
            i += Time.deltaTime;
            if (i < time)
            {
                yield return null;
            }
            else
            {
                isCharged = true;
                boxCol.enabled = true;
                yield return null;
            }
        }
    }

    void OnGUI()
    {
        if (debugDisplay)
        {
            guiStyle.fontSize = 8;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            float x = screenPos.x;
            float y = Screen.height - screenPos.y;

            GUI.Label(new Rect(x - 50f, y - 100f, 20f, 50f),
                "is charged = " + isCharged.ToString()
                //+ "\n" + "energy decrease = " + energyDecrease.ToString()
                , guiStyle);
        }
    }
}
