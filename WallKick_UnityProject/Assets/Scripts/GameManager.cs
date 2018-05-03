using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject pauseMenuUI;
    public int frameRate;
    public bool forcedFrameRate;
    public bool vSyncEnabled;

    private int numberOfPlateforms;
    [HideInInspector] public GameObject[] plateforms;
    [HideInInspector] public List<Collider2D> plateformColliders;
    [HideInInspector] public string plateformName;

    public static float timeSpeed = 1.0f;

    public float _freezeDurationWhenButtonHit;
    public static float freezeDurationWhenButtonHit;

    private void Awake()
    {
        freezeDurationWhenButtonHit = _freezeDurationWhenButtonHit;
        Time.timeScale = timeSpeed;

        forcedFrameRate = false;
        vSyncEnabled = false;

        pauseMenuUI.SetActive(false);

        plateformColliders = new List<Collider2D>();

        plateforms = GameObject.FindGameObjectsWithTag("Plateform");

        for (int i = 0; i < plateforms.Length; i++)
        {
            plateformColliders.Add(plateforms[i].GetComponent<Collider2D>());
            plateformColliders[i].name = "PlateformCol_" + i.ToString();
            Debug.Log(plateformColliders[i].name);
        }
    }

    private void Update()
    {
        if (forcedFrameRate)
        {
            vSyncEnabled = false;
            QualitySettings.vSyncCount = 0;  // VSync must be disabled to allow target framerate
            Application.targetFrameRate = frameRate;
        }
        else if (vSyncEnabled)
        {
            forcedFrameRate = false;
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public static IEnumerator FreezeFrame(float freezeDuration)
    {
        float i = .0f;
        while (i <= freezeDuration)
        {
            i++;
            if (i < freezeDuration)
            {
                //Debug.Log(i);
                Time.timeScale = .0f;
            }
            else if (i < freezeDuration + 1)
            {
                //Debug.Log("end of coroutine");
                ScreenShake.shakeDuration = .1f;
                Time.timeScale = timeSpeed;
            }
            yield return null;
        }
    }
}
