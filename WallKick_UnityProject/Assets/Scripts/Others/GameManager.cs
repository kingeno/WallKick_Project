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
    public GameObject[] plateforms;
    public List<Collider2D> plateformColliders;
    public string plateformName;


    private void Awake()
    {
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
}
