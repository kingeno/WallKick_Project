using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoopModeHighScore : MonoBehaviour
{
    // MODE RULES
    // "Reach It" is a mode where the players have to cooperate in order to reach an "energy score" in a given time.
    // The score increase when the wall split velocity is higher than 10% of it maximum speed
    // exemple : players have to reach 1000 "energy points" whithin 2 minutes.

    public WallSplitMovement splitWallMovement;

    public float maxCapacity;
    public float minCapacity;

    private float generatedEnergy;
    private float currentEnergy;

    private int displayedEnergy;

    private float splitWallVelocity;

    public float timeLeft;

    [Header("UI")]
    public Image energyGauge;

    private float fillAmount;

    [SerializeField]
    private float lerpSpeed;

    private GUIStyle guiStyle = new GUIStyle();

    private void OnEnable()
    {
        currentEnergy = 0;
    }

    void Start()
    {
        energyGauge.fillAmount = minCapacity;

        guiStyle.normal.textColor = Color.black;

        InvokeRepeating("AddEnergy", 0f, 0.5f);
    }

    void Update()
    {
        splitWallVelocity = WallSplitMovement.normalizedHorizontalVelocity;
        splitWallVelocity = Mathf.Round(splitWallVelocity * 100f) / 100f;

        HandleBar();

        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
        }
        else
            timeLeft = 0f;
    }

    public void AddEnergy()
    {
        generatedEnergy = splitWallVelocity;

        if (generatedEnergy < 0)
            generatedEnergy *= -1;

        currentEnergy += generatedEnergy;
    }


    private void HandleBar()
    {
        //energyGauge.fillAmount = generatedEnergy;

        //if (fillAmount != energyGauge.fillAmount)
        //{
            energyGauge.fillAmount = Mathf.Lerp(energyGauge.fillAmount, generatedEnergy, Time.deltaTime * lerpSpeed);
        //}
    }


    void OnGUI()
    {
        guiStyle.fontSize = 14;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        float x = screenPos.x;
        float y = Screen.height - screenPos.y;

        GUI.Label(new Rect(x - 150f, y - 1f, 20f, 20f),
            "current energy = " + displayedEnergy.ToString("F2")
            + "\n" + "generated energy= " + generatedEnergy.ToString("F2")
            , guiStyle);

        GUI.Label(new Rect(x - 150f, y -= 30f, 20f, 20f),
            "Timer : " + timeLeft.ToString("F2")
            , guiStyle);
    }
}
