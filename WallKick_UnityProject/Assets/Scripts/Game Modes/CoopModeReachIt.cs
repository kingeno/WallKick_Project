using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoopModeReachIt : MonoBehaviour {

    // MODE RULES
    // "Reach It" is a mode where the players have to cooperate in order to reach an "energy score" in a given time.
    // The score increase when the wall split velocity is higher than 10% of it maximum speed
    // exemple : players have to reach 1000 "energy points" whithin 2 minutes.

    public float maxCapacity;
    public float minCapacity;

    private float generatedEnergy;
    private float currentEnergy;

    private int displayedEnergy;

    private float splitWallVelocity;

    [Header("UI")]
    public Image energyGauge;

    private GUIStyle guiStyle = new GUIStyle();

    void Start () {

        energyGauge.fillAmount = minCapacity;

        guiStyle.normal.textColor = Color.black;
    }
	
	void Update () {

        splitWallVelocity = WallSplitMovement.normalizedHorizontalVelocity;

        if (splitWallVelocity >= 0.2f || splitWallVelocity <= -0.2f)
            generatedEnergy = splitWallVelocity;
        else
            generatedEnergy = 0f;

        if (generatedEnergy < 0)
            generatedEnergy *= -1;

        currentEnergy += generatedEnergy;

        energyGauge.fillAmount = currentEnergy / maxCapacity;

        displayedEnergy = (int)currentEnergy;

        if (currentEnergy > maxCapacity)
        {
            currentEnergy = maxCapacity;
        }
    }

    //IEnumerator EnergyConsumption(float generatedEnergy)
    //{
    //    float i = .0f;
    //    while (i <= 1.0f)
    //    {
    //        i += 2.0f;
    //        currentEnergy += generatedEnergy;
    //        if (currentEnergy < maxCapacity)
    //        {
    //            yield return null;/*new WaitForSeconds(1.0f);*/
    //        }
    //        else
    //        {
    //            currentEnergy = maxCapacity;
    //            yield return null;
    //        }
    //    }
    //}

    void OnGUI()
    {
        guiStyle.fontSize = 14;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        float x = screenPos.x;
        float y = Screen.height - screenPos.y;

        GUI.Label(new Rect(x - 150f, y - 1f, 20f, 20f),
            "current energy = " + displayedEnergy.ToString()
            + "\n" + "generated energy= " + generatedEnergy.ToString()
            , guiStyle);
    }
}
